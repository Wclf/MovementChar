using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GroundMover : MonoBehaviour
{
    [Header("Chỉ số di chuyển")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float gravity = -9.81f;
    public float jumpForce = 5f;

    [Header("Input")]
    public MonoBehaviour inputSource;

    public Transform cameraTransform;

    CharacterController controller;
    IMobileInput input;

    float verticalVelocity;
    bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = inputSource as IMobileInput;

        if (input == null)
            Debug.LogError("inputSource không implement IMobileInput!");

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (input == null) return;

        HandleGroundCheck();
        HandleMovement();
        ApplyGravity();
        HandleJump();
    }

    void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f; // giữ nhân vật dính đất
    }

    void HandleMovement()
    {
        Vector2 axis = input.MoveAxis;

        // Nếu có camera → di chuyển theo hướng camera
        Vector3 moveDir;

        if (cameraTransform != null)
        {
            // Lấy forward/right của camera nhưng bỏ trục Y (không bay lên trời)
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = cameraTransform.right;
            camRight.y = 0;
            camRight.Normalize();

            // W/S đi theo camForward, A/D đi theo camRight
            moveDir = camForward * axis.y + camRight * axis.x;
        }
        else
        {
            // Nếu chưa gán camera thì fallback về trục thế giới
            moveDir = new Vector3(axis.x, 0, axis.y);
        }

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        float speed = input.IsRunning ? runSpeed : moveSpeed;

        Vector3 finalVelocity = moveDir * speed;
        finalVelocity.y = verticalVelocity;

        controller.Move(finalVelocity * Time.deltaTime);

        // Xoay nhân vật theo hướng đang di chuyển
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        if (isGrounded && input.IsJumpPressed)
        {
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        verticalVelocity += gravity * Time.deltaTime;
    }
}
