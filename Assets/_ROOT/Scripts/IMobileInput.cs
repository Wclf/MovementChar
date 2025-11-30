using UnityEngine;

public interface IMobileInput
{
    Vector2 MoveAxis { get; }   // X: trái/phải, Y: tiến/lùi
    bool IsRunning { get; }     // chạy
    bool IsJumpPressed { get; } // nhảy (nhấn một lần)

}
