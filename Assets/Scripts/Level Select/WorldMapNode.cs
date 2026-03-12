using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldMapNode : MonoBehaviour
{
    [Header("Data")] 
    [SerializeField] public LevelNodeData nodeData;
    
    [Header("Connections")]
    [SerializeField] public List<WorldMapNode> neighbors = new List<WorldMapNode>();

    [Header("Visuals")] 
    [SerializeField] private GameObject lockedVisual;
    [SerializeField] private GameObject unlockedVisual;
    [SerializeField] private GameObject completedVisual;
    [SerializeField] private GameObject selectedIndicator;

    private void Start()
    {
        RefreshVisuals();
    }

    public void RefreshVisuals()
    {
        bool locked = !nodeData.isUnlocked;
        bool complete = nodeData.isCompleted;
        
        lockedVisual?.SetActive(locked);
        unlockedVisual?.SetActive(!locked && !complete);
        completedVisual?.SetActive(complete);
    }

    public void SetSelected(bool selected)
    {
        selectedIndicator?.SetActive(selected);
    }


    public List<WorldMapNode> GetAccessibleNeighbors()
    {
        return neighbors.Where(n => n.nodeData.isUnlocked).ToList();
    }

}
