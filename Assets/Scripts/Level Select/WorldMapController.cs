using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class WorldMapController : MonoBehaviour
{
    [SerializeField] private WorldMapPlayer mapPlayer;
    [SerializeField] private WorldMapNode startNode;


    private Dictionary<(WorldMapNode, WorldMapNode), WorldMapPath> pathLookup =
        new SerializedDictionary<(WorldMapNode, WorldMapNode), WorldMapPath>();

    private WorldMapNode selectedNode;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    
    
    private void Start()
    {
        // Auto-register all paths in the scene
        foreach (var path in FindObjectsByType<WorldMapPath>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            pathLookup[(path.FromNode, path.ToNode)] = path;
            pathLookup[(path.ToNode, path.FromNode)] = path; // bidirectional
        }

        mapPlayer.CurrentNode = startNode;
        SetSelectedNode(startNode);
        mapPlayer.OnArrivedAtNode += OnPlayerArrived;
    }
    
    private void Update()
    {
        if (mapPlayer.IsMoving) return;
        
        HandleInput();
    }
    
    private void HandleInput()
    {
        Vector2 input = playerInput.actions.FindAction("Navigate").ReadValue<Vector2>();
        
        if (playerInput.actions.FindAction("Submit").WasPressedThisFrame())
        {
            EnterSelectedLevel();
            return;
        }

        // Only respond to a fresh directional press
        if (!playerInput.actions.FindAction("Navigate").WasPressedThisFrame()) return;

        WorldMapNode best = GetBestNeighborInDirection(input);
        if (best != null)
            MoveToNode(best);
    }

    
    private WorldMapNode GetBestNeighborInDirection(Vector2 input)
    {
        if (input.sqrMagnitude < 0.5f) return null;

        Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;
        WorldMapNode best = null;
        float bestDot = -1f;

        foreach (var neighbor in mapPlayer.CurrentNode.GetAccessibleNeighbors())
        {
            Vector3 toNeighbor = (neighbor.transform.position - mapPlayer.CurrentNode.transform.position).normalized;
            float dot = Vector3.Dot(inputDir, toNeighbor);
            if (dot > bestDot)
            {
                bestDot = dot;
                best = neighbor;
            }
        }

        // Threshold — require the input to be roughly pointing at the node
        return bestDot > 0.5f ? best : null;
    }


    private void MoveToNode(WorldMapNode destination)
    {
        if (!pathLookup.TryGetValue((mapPlayer.CurrentNode, destination), out WorldMapPath path))
        {
            Debug.LogWarning($"No path found between {mapPlayer.CurrentNode.name} and {destination.name}");
            return;
        }
        SetSelectedNode(destination);
        mapPlayer.TravelTo(destination, path);
    }

    private void OnPlayerArrived(WorldMapNode node)
    {
        SetSelectedNode(node);
        // this is where I can pull up UI info of the level, etc
        // maybe a title, icon and star count
    }

    private void SetSelectedNode(WorldMapNode node)
    {
        selectedNode?.SetSelected(false);
        selectedNode = node;
        selectedNode?.SetSelected(true);
    }

    private void EnterSelectedLevel()
    {
        if (selectedNode == null || !selectedNode.nodeData.isUnlocked) return;
        SceneManager.LoadScene(selectedNode.nodeData.scene.name);
    }
    
}
