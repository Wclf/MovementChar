using UnityEngine;

public class KeyboardOrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;        // Nhân vật

    [Header("Khoảng cách & góc")]
    public float distance = 6f;
    public float yawSpeed = 60f;        // tốc độ xoay bằng phím mũi tên
    public float pitchSpeed = 60f;      // tốc độ ngẩng lên / cúi xuống
    public float minPitch = -20f;
    public float maxPitch = 60f;

    [Header("Phím điều khiển camera")]
    public KeyCode lookLeftKey = KeyCode.LeftArrow;
    public KeyCode lookRightKey = KeyCode.RightArrow;
    public KeyCode lookUpKey = KeyCode.UpArrow;
    public KeyCode lookDownKey = KeyCode.DownArrow;

    [Header("Xoay theo A / D (MoveAxis.x)")]
    public MonoBehaviour moveInputSource;   // gán KeyboardInput vào đây
    public float moveYawSpeed = 30f;        // tốc độ xoay camera khi giữ A/D

    IMobileInput moveInput;

    float yaw;   // quay ngang (Y)
    float pitch; // quay dọc (X)

    void Awake()
    {
        if (moveInputSource != null)
        {
            moveInput = moveInputSource as IMobileInput;
            if (moveInput == null)
                Debug.LogError("moveInputSource không implement IMobileInput!");
        }
    }

    void Start()
    {
        if (target == null) return;

        // Lấy khoảng cách & góc ban đầu từ vị trí camera hiện tại
        Vector3 dir = transform.position - target.position;
        distance = dir.magnitude;

        if (distance > 0.001f)
        {
            dir.Normalize();
            pitch = Mathf.Asin(dir.y) * Mathf.Rad2Deg;
            yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        float dt = Time.deltaTime;

        // 1) Input từ phím mũi tên
        float horizontalFromKeys = 0f;
        if (Input.GetKey(lookLeftKey)) horizontalFromKeys -= 1f;
        if (Input.GetKey(lookRightKey)) horizontalFromKeys += 1f;

        float verticalFromKeys = 0f;
        if (Input.GetKey(lookUpKey)) verticalFromKeys += 1f;
        if (Input.GetKey(lookDownKey)) verticalFromKeys -= 1f;

        // 2) Input xoay thêm từ A/D (MoveAxis.x)
        float horizontalFromMove = 0f;
        if (moveInput != null)
        {
            Vector2 axis = moveInput.MoveAxis;   // A/D, W/S
            horizontalFromMove = axis.x;         // chỉ lấy A/D
        }

        // Cộng dồn vào yaw/pitch
        yaw += horizontalFromKeys * yawSpeed * dt;
        yaw += horizontalFromMove * moveYawSpeed * dt;   // xoay theo A/D

        pitch += verticalFromKeys * pitchSpeed * dt;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Tính rotation & vị trí camera
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rot * new Vector3(0, 0, -distance);

        transform.position = target.position + offset;
        transform.LookAt(target.position);
    }
}
