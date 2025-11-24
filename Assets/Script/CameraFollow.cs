using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Rigidbody2D targetRb;      
    public float smoothSpeed = 5f;    
    public Vector3 offset = new Vector3(0, 0, -10);

    void FixedUpdate()
    {
        if (targetRb == null) return;

        Vector3 targetPos = targetRb.position + new Vector2(offset.x, offset.y);
        targetPos.z = offset.z;

        
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.fixedDeltaTime);
    }
}
