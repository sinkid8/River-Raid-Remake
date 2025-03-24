using UnityEngine;

public class ShipMovementHandler
{
    private Rigidbody2D rb;
    private float moveSpeed;
    private float rotationSpeed;
    private Vector2 inputDirection;

    public ShipMovementHandler(Rigidbody2D rb, float moveSpeed, float rotationSpeed)
    {
        this.rb = rb;
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;
    }

    public void UpdateInput(Vector2 newInputDirection)
    {
        inputDirection = newInputDirection;
    }

    public void Move()
    {
        // Apply movement based on input direction
        rb.linearVelocity = inputDirection * moveSpeed;

        // Rotate ship to face movement direction if there is movement
        if (inputDirection.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            rb.rotation = Mathf.LerpAngle(rb.rotation, angle, Time.deltaTime * rotationSpeed);
        }
    }

    public void ApplyDampening(float dampeningValue)
    {
        // Apply dampening when no input is given
        if (inputDirection.magnitude < 0.1f)
        {
            rb.linearVelocity *= dampeningValue;
        }
    }
}