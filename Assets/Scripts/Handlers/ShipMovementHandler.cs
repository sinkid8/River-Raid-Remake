using UnityEngine;

public class ShipMovementHandler
{
    private Rigidbody2D rb;
    private float moveSpeed;
    private float rotationSpeed;
    private Vector2 inputDirection;
    private float verticalDrift;
    private bool gameStarted;

    public ShipMovementHandler(Rigidbody2D rb, float moveSpeed, float rotationSpeed, float verticalDrift = 0.5f)
    {
        this.rb = rb;
        this.moveSpeed = moveSpeed;
        this.rotationSpeed = rotationSpeed;
        this.verticalDrift = verticalDrift;
        this.gameStarted = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void UpdateInput(Vector2 newInputDirection)
    {
        inputDirection = newInputDirection;
    }

    public void StartGame()
    {
        gameStarted = true;
    }

    public void StopMovement()
    {
        gameStarted = false;
    }

    public void Move()
    {
        if (gameStarted)
        {
            Vector2 movement = inputDirection * moveSpeed;
            movement.y += verticalDrift;
            rb.linearVelocity = movement; 
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void ApplyDampening(float dampeningValue)
    {
        if (gameStarted && inputDirection.magnitude < 0.1f)
        {
            rb.linearVelocity *= dampeningValue;
        }
    }

    public void IncreaseVerticalDrift(float amount)
    {
        verticalDrift += amount;
        Debug.Log("Vertical drift increased by " + amount + ". New vertical drift: " + verticalDrift);
    }
}
