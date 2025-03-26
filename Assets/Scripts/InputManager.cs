using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    // Movement events
    public UnityEvent<Vector2> OnMove = new UnityEvent<Vector2>();

    // Weapon firing events
    public UnityEvent OnFireLaser = new UnityEvent();
    public UnityEvent OnFireMissile = new UnityEvent();
    public UnityEvent OnFireEnergyWeapon = new UnityEvent(); // New energy weapon event

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
        if (Input.GetKey(KeyCode.D))
        {
            input += Vector2.right;
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

        // Energy weapon firing with E key
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnFireEnergyWeapon?.Invoke();
        }

        // Handle reset/restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnResetPressed?.Invoke();
        }
    }
}