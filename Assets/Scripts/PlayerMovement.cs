using UnityEngine;
using UnityEngine.InputSystem;

public class MobilePlayerMovement : MonoBehaviour
{
    public FloatingJoystick joystick;
    public Transform cameraTransform;

    [Header("Movement")]
    public float moveSpeed = 7f;
    public float sprintMultiplier = 1.5f;
    public float rotationSpeed = 12f;
    public float directionSmoothTime = 0.12f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public float groundCheckDistance = 0.15f;
    public LayerMask groundLayer;

    public bool sprintActive = false;

    Rigidbody rb;
    GameInputc input;

    Vector3 currentMoveDirection;
    Vector3 moveDirectionVelocity;

    bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        input = new GameInputc();
        input.Enable();

        rb.freezeRotation = true;
    }

    void OnDestroy()
    {
        input?.Disable();
    }

    void Update()
    {
        if (!GameManager.Instance.IsInState(GameState.Running))
            return;

        CheckGrounded();

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.IsInState(GameState.Running))
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        Vector2 keyboardInput =
            input.Gameplay.Move.ReadValue<Vector2>();

        float joystickX = joystick.Horizontal;
        float joystickZ = joystick.Vertical;

        float horizontal =
            Mathf.Abs(keyboardInput.x) > Mathf.Abs(joystickX)
            ? keyboardInput.x
            : joystickX;

        float vertical =
            Mathf.Abs(keyboardInput.y) > Mathf.Abs(joystickZ)
            ? keyboardInput.y
            : joystickZ;

        Vector3 inputDirection =
            new Vector3(horizontal, 0f, vertical);

        float moveAmount = inputDirection.magnitude;

        if (moveAmount > 0.1f)
        {
            float targetAngle =
                Mathf.Atan2(inputDirection.x, inputDirection.z)
                * Mathf.Rad2Deg
                + cameraTransform.eulerAngles.y;

            Quaternion targetRotation =
                Quaternion.Euler(0f, targetAngle, 0f);

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );

            Vector3 targetMoveDirection =
                targetRotation * Vector3.forward;

            currentMoveDirection =
                Vector3.SmoothDamp(
                    currentMoveDirection,
                    targetMoveDirection,
                    ref moveDirectionVelocity,
                    directionSmoothTime
                );
        }
        else
        {
            currentMoveDirection = Vector3.zero;
        }

        float speed = moveSpeed;

        if (sprintActive)
            speed *= sprintMultiplier;

        Vector3 velocity = currentMoveDirection * speed;

        // Preserve Rigidbody's Y velocity for jumping/gravity
        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;
    }

    void CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        isGrounded = Physics.Raycast(
            origin,
            Vector3.down,
            groundCheckDistance + 0.1f,
            groundLayer
        );
    }
}