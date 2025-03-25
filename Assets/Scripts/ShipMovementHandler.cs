using UnityEngine;

public class ShipMovementHandler
{
    private Rigidbody2D rb;
    private float moveSpeed;
    private float rotationSpeed;
    private Vector2 inputDirection;
    private float verticalDrift;

    public ShipMovementHandler(Rigidbody2D rb, float moveSpeed, float rotationSpeed, float verticalDrift = 0.5f)
    {
        this.rb = rb;
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;
        this.verticalDrift = verticalDrift;
    }

    public void UpdateInput(Vector2 newInputDirection)
    {
        inputDirection = newInputDirection;
    }

    public void Move()
    {
        Vector2 movement = inputDirection * moveSpeed;
        movement.y += verticalDrift;
        rb.linearVelocity = movement;
    }

    public void ApplyDampening(float dampeningValue)
    {
        if (inputDirection.magnitude < 0.1f)
        {
            rb.linearVelocity *= dampeningValue;
        }
    }
}