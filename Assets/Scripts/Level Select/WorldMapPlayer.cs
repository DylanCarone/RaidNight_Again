using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class WorldMapPlayer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float travelSpeed = 4f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Grounding")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundRaycastLength = 5f;
    [SerializeField] private float groundOffset = 0f;
    [SerializeField] private float normalAlignSpeed = 6f;
    
    public WorldMapNode CurrentNode { get; set; }

    private bool isMoving = false;

    public bool IsMoving => isMoving;

    public event Action<WorldMapNode> OnArrivedAtNode;

    private Vector3 groundedPosition;
    private Quaternion groundedRotation;

    private void Start()
    {
        if (CurrentNode != null)
        {
            SnapToGround(CurrentNode.transform.position);
            //transform.position = CurrentNode.transform.position;
        }
    }

    #region Public API

    public void SetStartNode(WorldMapNode node)
    {
        CurrentNode = node;
        SnapToGround(node.transform.position);
    }
    

    public void TravelTo(WorldMapNode destination, WorldMapPath path)
    {
        if (isMoving) return;
        StartCoroutine(TravelCoroutine(destination, path));

    }
    #endregion

    #region  Travel Coroutine
    
    private IEnumerator TravelCoroutine(WorldMapNode destination, WorldMapPath path)
    {
        isMoving = true;

        List<Vector3> points = path.GetTravelPoints();

        if (path.ToNode != destination)
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

        CurrentNode = destination;
        isMoving = false;
        OnArrivedAtNode?.Invoke(destination);
    }
    #endregion

    #region Grounding

    /// <summary>
    /// Returns the world position snapped to the ground directly below xzPosition
    /// Falls back to the input position if nothing hits
    /// </summary>

    private Vector3 GetGroundedPosition(Vector3 xzPosition)
    {
        Vector3 rayOrigin = new Vector3(xzPosition.x, xzPosition.y + groundRaycastLength * 0.5f, xzPosition.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundRaycastLength, groundMask))
        {
            return hit.point + Vector3.up * groundOffset;
        }

        return xzPosition;
    }

    private void SnapToGround(Vector3 position)
    {
        transform.position = GetGroundedPosition(position);
        AlignToGroundImmediate();
    }

    #endregion

    #region Normal Alignment
    /// <summary>
    /// Smoothly tilts the character to match the slope beneath them.
    /// Call every frame while moving.
    /// </summary>
    private void AlignToGround(Vector3 position)
    {
        Vector3 rayOrigin = position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundRaycastLength, groundMask))
        {
            // Build a target rotation that:
            //   - Keeps our current facing direction (forward)
            //   - Tilts our up axis to match the terrain normal
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal)
                                        * Quaternion.Euler(0, transform.eulerAngles.y, 0);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                normalAlignSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// Instant version of AlignToGround — no lerp. Used on start/teleport.
    /// </summary>
    private void AlignToGroundImmediate()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundRaycastLength, groundMask))
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal)
                                 * Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
    }
    

    #endregion

    #region Rotation

    /// <summary>
    /// Rotates the character to face a target position, only on the XZ plane.
    /// The slope tilt from AlignToGround is applied separately so they don't fight.
    /// </summary>
    private void RotateTowardTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetYRotation = Quaternion.LookRotation(direction);

        // Only rotate the Y axis here — AlignToGround handles XZ tilt
        float currentY = transform.eulerAngles.y;
        float targetY = targetYRotation.eulerAngles.y;
        float newY = Mathf.LerpAngle(currentY, targetY, rotationSpeed * Time.deltaTime);

        // Preserve the slope tilt, replace only the Y
        Vector3 euler = transform.eulerAngles;
        euler.y = newY;
        transform.eulerAngles = euler;
    }


    #endregion
}
