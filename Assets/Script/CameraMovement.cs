using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // The player's transform
    public float smoothSpeed = 0.125f;  // How smoothly the camera catches up
    public Vector3 offset;  // Camera position relative to player
    void Start()
    {
        if (target != null && target.TryGetComponent<Rigidbody2D>(out var rb))
        {
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }
    void LateUpdate()
    {
        if (target != null)
        {
            float speed = Time.deltaTime * smoothSpeed; 

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, speed);
            transform.position = smoothedPosition;
        }
    }
}