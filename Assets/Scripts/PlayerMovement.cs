using UnityEngine;
using UnityEngine.InputSystem;

public class MobilePlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;
    public float moveSpeed = 7f;
    public float rotationSpeed = 12f;
    public float jumpForce = 7f;
    public float groundCheckDistance = 1.2f;
    public LayerMask groundLayer;
    public float acceleration = 20f;
    public float deceleration = 8f;

    Rigidbody rb;
    GameInputc input;
    bool grounded;
    bool jumpRequested;
    private float gravityMultiplier = 3f;
    public float groundAcceleration = 60f;
    public float airAcceleration = 15f;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = new GameInputc();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        if (!GameManager.Instance.IsInState(GameState.Running))
            return;

        grounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.IsInState(GameState.Running))
            return;

        Vector2 inputValue = input.Gameplay.Move.ReadValue<Vector2>();

        Vector3 inputDirection = new Vector3(
            inputValue.x,
            0f,
            inputValue.y
        );

        Vector3 velocity = rb.linearVelocity;

        bool wasGrounded = grounded;

        grounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );

        // Just landed
        if (grounded && velocity.y <= 0f)
        {
            velocity.x = 0f;
            velocity.z = 0f;
            velocity.y = -0.5f;
        }

        if (inputDirection.sqrMagnitude > 0.01f)
        {
            inputDirection.Normalize();

            float angle =
                Mathf.Atan2(inputDirection.x, inputDirection.z) *
                Mathf.Rad2Deg +
                cameraTransform.eulerAngles.y;

            Quaternion rotation =
                Quaternion.Euler(0f, angle, 0f);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    rotation,
                    rotationSpeed * Time.fixedDeltaTime
                )
            );

            Vector3 direction =
                rotation * Vector3.forward;

            Vector3 targetVelocity =
                direction * moveSpeed;
            float accel = grounded ? groundAcceleration : airAcceleration;

            velocity.x = Mathf.MoveTowards(
                velocity.x,
                targetVelocity.x,
                accel * Time.fixedDeltaTime
            );

            velocity.z = Mathf.MoveTowards(
                velocity.z,
                targetVelocity.z,
                accel * Time.fixedDeltaTime
            );
        }
        else if (!grounded)
        {
            // Keep momentum while airborne
        }
        else
        {
            // Stop immediately when standing still on a tile
            velocity.x = 0f;
            velocity.z = 0f;
        }

        // Faster falling
        if (!grounded)
        {
            velocity += Physics.gravity *
                        (gravityMultiplier - 1f) *
                        Time.fixedDeltaTime;
        }

        if (jumpRequested && grounded)
        {
            velocity.y = jumpForce;
            jumpRequested = false;
        }

        rb.linearVelocity = velocity;
    }
}