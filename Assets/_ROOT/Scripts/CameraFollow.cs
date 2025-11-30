using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, -10);
    public float followSpeed = 10f;

    void LateUpdate()
    {
        if (target == null) return;
            
        // Camera chỉ follow vị trí
        Vector3 desiredPos = target.position + offset;

        // Follow vị trí mượt
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            followSpeed * Time.deltaTime
        );

        // KHÔNG động vào rotation
        // Camera giữ nguyên góc nhìn ban đầu
    }
}

