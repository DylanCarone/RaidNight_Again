using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WorldMapPath : MonoBehaviour
{
    public WorldMapNode fromNode;
    public WorldMapNode toNode;

    [Header("Waypoints")] public List<Transform> waypoints = new List<Transform>();

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        BuildPath();
    }

    public void BuildPath()
    {
        var points = new List<Vector3> { fromNode.transform.position };

        foreach (var wp in waypoints)
        {
            points.Add(wp.position);
        }

        points.Add(toNode.transform.position);

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    public List<Vector3> GetTravelPoints()
    {
        var points = new List<Vector3> { fromNode.transform.position };
        foreach (var wp in waypoints)
        {
            points.Add(wp.position);
        }
        points.Add(toNode.transform.position);
        return points;
    }

}
