using UnityEngine;

public class JoystickInput : MonoBehaviour, IMobileInput
{
    [Header("Tham chiếu joystick của Unity Joystick Pack")]
    public Joystick joystick;   // <-- Kéo Fixed Joystick vào đây

    [Header("Chạy khi đẩy mạnh joystick?")]
    public bool enableRun = true;
    public float runThreshold = 0.9f;

    bool jumpPressed;
        
    public Vector2 MoveAxis
    {
        get
        {
            if (joystick == null) return Vector2.zero;
            return new Vector2(joystick.Horizontal, joystick.Vertical);
        }
    }

    public bool IsRunning => false;

    public bool IsJumpPressed
    {
        get
        {
            bool result = jumpPressed;
            jumpPressed = false;
            return result;
        }
    }

    // Gọi hàm này từ UI Button để nhảy
    public void PressJump()
    {
        jumpPressed = true;
    }
}
