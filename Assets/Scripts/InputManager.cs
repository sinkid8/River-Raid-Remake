using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    // Movement events
    public UnityEvent<Vector2> OnMove = new UnityEvent<Vector2>();

    // Weapon firing events
    public UnityEvent OnFireLaser = new UnityEvent();
    public UnityEvent OnFireMissile = new UnityEvent();

    // Utility events
    public UnityEvent OnResetPressed = new UnityEvent();

    void Update()
    {
        // Handle movement input
        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
        {
            input += Vector2.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            input += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            input += Vector2.right;
        }
        if (Input.GetKey(KeyCode.W))
        {
            input += Vector2.up;
        }

        // Normalize if diagonal movement
        if (input.magnitude > 1f)
        {
            input.Normalize();
        }

        // Send movement input
        OnMove?.Invoke(input);

        // Handle weapon firing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnFireLaser?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            OnFireMissile?.Invoke();
        }

        // Handle reset/restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnResetPressed?.Invoke();
        }
    }
}