using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;       // The player (Ajax) to follow
    public float smoothSpeed = 5f; // How smoothly the camera catches up
    public Vector3 offset = new Vector3(0f, 1f, -10f); // Camera position relative to player

    void LateUpdate()
    {
        // If there is no target assigned, do nothing
        if (target == null) return;

        // Calculate the desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera towards that position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Apply the new position
        transform.position = smoothedPosition;
    }
}