using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    // Movement events
    public UnityEvent<Vector2> OnMove = new UnityEvent<Vector2>();

    // Weapon firing events
    public UnityEvent OnFireLaser = new UnityEvent();
    public UnityEvent OnFireMissile = new UnityEvent();
    public UnityEvent OnFireEnergyWeapon = new UnityEvent();

    void Update()
    {
        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
        {
            input += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            input += Vector2.right;
        }

        if (input.magnitude > 1f)
        {
            input.Normalize();
        }

        OnMove?.Invoke(input);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnFireLaser?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            OnFireMissile?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnFireEnergyWeapon?.Invoke();
        }
    }
}