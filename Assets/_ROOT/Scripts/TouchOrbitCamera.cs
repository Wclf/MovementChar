using UnityEngine;

public class TouchOrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;        // Player

    [Header("Khoảng cách & góc")]
    public float distance = 6f;
    public float minPitch = -20f;
    public float maxPitch = 60f;

    [Header("Độ nhạy xoay")]
    public float rotateSensitivity = 0.2f;   // chỉnh cảm giác vuốt

    [Header("Zoom (tùy chọn, có thể để 0 bỏ qua)")]
    public float zoomSpeed = 2f;
    public float minDistance = 3f;
    public float maxDistance = 10f;

    IMobileInput moveInput;

    [Header("Ảnh hưởng từ hướng di chuyển")]
    public MonoBehaviour moveInputSource;   // gán JoystickInput (hoặc KeyboardInput) vào đây
    public float moveYawSpeed = 30f;        // tốc độ cam xoay theo trái/phải

    float yaw;    // quay ngang (Y)
    float pitch;  // quay dọc (X)

    Vector2 lastPointerPos;
    bool dragging = false;

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

        // Lấy yaw/pitch ban đầu từ vị trí camera hiện tại
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

        HandleRotation();
        HandleZoom();

        // 👇 thêm dòng này
        HandleMoveInfluence(Time.deltaTime);

        // phần tính pitch/distance + set vị trí/rotation giữ nguyên
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rot * new Vector3(0, 0, -distance);

        transform.position = target.position + offset;
        transform.LookAt(target.position);
    }

    void HandleMoveInfluence(float dt)
    {
        if (moveInput == null) return;

        Vector2 axis = moveInput.MoveAxis;

        // Chỉ xoay khi có di chuyển ngang (trái/phải)
        if (Mathf.Abs(axis.x) > 0.01f)
        {
            yaw += axis.x * moveYawSpeed * dt;
        }
    }

    void HandleRotation()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // PC: chỉ xoay nếu nhấn chuột TRÁI ở NỬA BÊN PHẢI màn hình
        if (Input.GetMouseButtonDown(0)) // CHUỘT TRÁI
        {
            if (Input.mousePosition.x > Screen.width * 0.5f)
            {
                dragging = true;
                lastPointerPos = Input.mousePosition;
            }
            else
            {
                dragging = false;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

        if (dragging)
        {
            Vector2 currentPos = Input.mousePosition;
            Vector2 delta = currentPos - lastPointerPos;
            lastPointerPos = currentPos;

            yaw += delta.x * rotateSensitivity;
            pitch -= delta.y * rotateSensitivity;
        }

#else
    // MOBILE: chỉ xoay khi có touch bên PHẢI
    if (Input.touchCount > 0)
    {
        Touch? rotateTouch = null;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.position.x > Screen.width * 0.5f)
            {
                rotateTouch = t;
                break;
            }
        }

        if (rotateTouch.HasValue)
        {
            Touch t = rotateTouch.Value;

            if (t.phase == TouchPhase.Began)
            {
                dragging = true;
                lastPointerPos = t.position;
            }
            else if (t.phase == TouchPhase.Moved && dragging)
            {
                Vector2 currentPos = t.position;
                Vector2 delta = currentPos - lastPointerPos;
                lastPointerPos = currentPos;

                yaw   += delta.x * rotateSensitivity;
                pitch -= delta.y * rotateSensitivity;
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                dragging = false;
            }
        }
        else
        {
            dragging = false;
        }
    }
#endif
    }



    void HandleZoom()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // Zoom bằng scroll chuột (cho tiện debug)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            distance -= scroll * zoomSpeed;
        }
#else
        // Pinch zoom trên mobile (2 ngón tay)
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prevPos0 = t0.position - t0.deltaPosition;
            Vector2 prevPos1 = t1.position - t1.deltaPosition;

            float prevDist = (prevPos0 - prevPos1).magnitude;
            float currentDist = (t0.position - t1.position).magnitude;
            float diff = currentDist - prevDist;

            distance -= diff * (zoomSpeed / 200f);
        }
#endif
    }
}
