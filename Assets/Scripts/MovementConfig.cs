using UnityEngine;

[CreateAssetMenu(fileName = "MovementConfig", menuName = "Game/MovementConfig")]
public class MovementConfig : ScriptableObject
{
    [Header("Tốc độ cơ bản")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    [Header("Gia tốc / Phanh")]
    public float acceleration = 10f;
    public float deceleration = 15f;

    [Header("Nhảy & Trọng lực")]
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("Mặt đất")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
}