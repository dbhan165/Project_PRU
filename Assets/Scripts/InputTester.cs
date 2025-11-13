using UnityEngine;
using UnityEngine.InputSystem;

public class InputTester : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current == null)
        {
            Debug.LogWarning("❌ Keyboard.current is NULL!");
            return;
        }

        if (Keyboard.current.escapeKey.isPressed)
        {
            Debug.Log("✅ ESC is being held down");
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("✅ ESC was pressed this frame");
        }

        if (Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
            Debug.Log("✅ ESC was released this frame");
        }
    }
}
