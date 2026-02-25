using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCamera;

    void Start()
    {
        mainCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Makes the UI look at the camera, then flips it to face the right way
        transform.LookAt(transform.position + mainCamera.forward);
    }
}