using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;

    // Movement parameters
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float movementDampening = 0.9f;
    [SerializeField] private float verticalDriftAmount = 0.5f;

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

        if (firePoint == null)
        {
            firePoint = transform;
        }

        movementHandler = new ShipMovementHandler(rb, moveSpeed, 0f, verticalDriftAmount);
        weaponHandler = new WeaponHandler(laserPrefab, missilePrefab, firePoint, laserFireRate, missileFireRate);

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
        movementHandler.Move();
        movementHandler.ApplyDampening(movementDampening);
    }
}