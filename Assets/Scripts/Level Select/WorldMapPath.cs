using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WorldMapPath : MonoBehaviour
{
    public WorldMapNode FromNode;
    public WorldMapNode ToNode;
    public List<Transform> waypoints = new List<Transform>();

    [Header("Gizmos")] 
    [SerializeField] private Color pathColor = Color.yellow;
    [SerializeField] private Color waypointColor = Color.cyan;
    [SerializeField] private float waypointSphereRadius = 0.3f;
    [SerializeField] private float arrowSize = 0.5f;

    public WorldMapNode GetFromNode() => FromNode;
    public WorldMapNode GetToNode() => ToNode;
    
    
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
        var points = GetTravelPoints();
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    public List<Vector3> GetTravelPoints()
    {
        var points = new List<Vector3> { FromNode.transform.position };
        foreach (var wp in waypoints)
        {
            points.Add(wp.position);
        }
        points.Add(ToNode.transform.position);
        return points;
    }

    private void OnDrawGizmos()
    {
        List<Vector3> points = GetTravelPoints();
        if (points.Count < 2) return;

        Gizmos.color = pathColor;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
            DrawArrow(points[i], points[i+1]);
        }

        Gizmos.color = waypointColor;
        foreach (var wp in waypoints)
        {
            if (wp == null) continue;
            Gizmos.DrawSphere(wp.position, waypointSphereRadius);
        }
        
        
        // ── Draw node labels ─────────────────────────────────────────
        #if UNITY_EDITOR
                UnityEditor.Handles.color = pathColor;
                if (FromNode != null)
                    UnityEditor.Handles.Label(FromNode.transform.position + Vector3.up * 1.5f,
                        $"FROM\n{FromNode.name}");
                if (ToNode != null)
                    UnityEditor.Handles.Label(ToNode.transform.position + Vector3.up * 1.5f,
                        $"TO\n{ToNode.name}");
        #endif
    }

    private void DrawArrow(Vector3 from, Vector3 to)
    {
        Vector3 midpoint = (from + to) * 0.5f;
        Vector3 direction = (to - from).normalized;

        if (direction == Vector3.zero) return;

        // The arrow head is two short lines branching back from the midpoint
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
        Vector3 left  = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150, 0) * Vector3.forward;

        Gizmos.DrawLine(midpoint, midpoint + right * arrowSize);
        Gizmos.DrawLine(midpoint, midpoint + left  * arrowSize);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Only show interactive handles when this path is selected
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;

            Vector3 newPos = UnityEditor.Handles.PositionHandle(
                waypoints[i].position, 
                Quaternion.identity
            );

            if (newPos != waypoints[i].position)
            {
                UnityEditor.Undo.RecordObject(waypoints[i], "Move Waypoint");
                waypoints[i].position = newPos;
            }
        }
    }
#endif

}
