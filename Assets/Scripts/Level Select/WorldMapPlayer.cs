using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapPlayer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float travelSpeed = 4f;

    public WorldMapNode currentNode;

    private bool isMoving = false;

    public bool IsMoving => isMoving;

    public event Action<WorldMapNode> OnArrivedAtNode;

    private void Start()
    {
        if (currentNode != null)
        {
            transform.position = currentNode.transform.position;
        }
    }

    public void TravelTo(WorldMapNode destination, WorldMapPath path)
    {
        if (isMoving) return;
        StartCoroutine(TravelCoroutine(destination, path));

    }

    private IEnumerator TravelCoroutine(WorldMapNode destination, WorldMapPath path)
    {
        isMoving = true;

        List<Vector3> points = path.GetTravelPoints();

        if (path.toNode != destination)
        {
            points.Reverse();
        }

        foreach (var point in points)
        {
            while (Vector3.Distance(transform.position, point) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, point, travelSpeed * Time.deltaTime);

                Vector3 dir = (point - transform.position);
                if (dir.sqrMagnitude > 0.001f)
                {
                    transform.forward = dir.normalized;
                }

                yield return null;
            }

            transform.position = point;

        }

        currentNode = destination;
        isMoving = false;
        OnArrivedAtNode?.Invoke(destination);
    }
}
