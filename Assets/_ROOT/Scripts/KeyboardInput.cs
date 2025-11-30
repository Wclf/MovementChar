using UnityEngine;

public class KeyboardInput : MonoBehaviour, IMobileInput
{
    [Header("Phím điều khiển")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

    bool jumpPressed;

    void Update()
    {
        // phát hiện nhảy 1 lần
        if (Input.GetKeyDown(jumpKey))
        {
            jumpPressed = true;
        }
    }

    public Vector2 MoveAxis
    {
        get
        {
            float h = 0f;
            if (Input.GetKey(leftKey)) h -= 1f;
            if (Input.GetKey(rightKey)) h += 1f;

            float v = 0f;
            if (Input.GetKey(forwardKey)) v += 1f;
            if (Input.GetKey(backwardKey)) v -= 1f;

            Vector2 axis = new Vector2(h, v);
            if (axis.sqrMagnitude > 1f)
                axis.Normalize(); // đi chéo không nhanh hơn

            return axis;
        }
    }

    public bool IsRunning => Input.GetKey(runKey);

    public bool IsJumpPressed
    {
        get
        {
            bool result = jumpPressed;
            jumpPressed = false;  // chỉ dùng 1 frame
            return result;
        }
    }

}
