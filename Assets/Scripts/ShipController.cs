using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;

    // Movement parameters
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float movementDampening = 0.9f;

    // Weapon parameters
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float laserFireRate = 0.25f;
    [SerializeField] private float missileFireRate = 1f;

    private Rigidbody2D rb;
    private ShipMovementHandler movementHandler;
    private WeaponHandler weaponHandler;
    private Vector2 currentInputDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // If no firePoint is assigned, use the ship's transform
        if (firePoint == null)
        {
            firePoint = transform;
        }

        // Initialize handlers
        movementHandler = new ShipMovementHandler(rb, moveSpeed, rotationSpeed);
        weaponHandler = new WeaponHandler(laserPrefab, missilePrefab, firePoint, laserFireRate, missileFireRate);

        // Setup input event listeners
        inputManager.OnMove.AddListener(HandleMoveInput);
        inputManager.OnFireLaser.AddListener(HandleFireLaser);
        inputManager.OnFireMissile.AddListener(HandleFireMissile);
    }

    private void HandleMoveInput(Vector2 input)
    {
        currentInputDirection = input;
        movementHandler.UpdateInput(input);
    }

    private void HandleFireLaser()
    {
        weaponHandler.FireLaser();
    }

    private void HandleFireMissile()
    {
        weaponHandler.FireMissile();
    }

    private void FixedUpdate()
    {
        // Handle movement in FixedUpdate for physics consistency
        movementHandler.Move();
        movementHandler.ApplyDampening(movementDampening);
    }
}