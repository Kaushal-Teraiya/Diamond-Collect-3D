using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TPSCamera : MonoBehaviour
{
    public Transform target;

    [Header("Distance")]
    public float distance = 6f;
    public float height = 2.5f;

    [Header("Rotation")]
    public float horizontalSensitivity = 120f;
    public float verticalSensitivity = 80f;
    public float minPitch = -10f;
    public float maxPitch = 45f;

    [Header("Follow")]
    public float positionSmoothTime = 0.08f;

    [Header("Collision")]
    public float collisionRadius = 0.3f;
    public LayerMask collisionLayers;

    float yaw;
    float pitch;

    Vector3 followPosition;
    Vector3 followVelocity;
    bool initialized;
    public float normalPitch = 15f;
    public float jumpPitch = 35f;
    public float pitchTransitionSpeed = 8f;
    public LayerMask groundLayer;

    void Start()
    {
        if (target == null)
            return;

        yaw = target.eulerAngles.y;
        followPosition = target.position;
        pitch = normalPitch;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, height, -distance);

        transform.position = target.position + offset;
        transform.rotation = rotation;

        initialized = true;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        if (!initialized)
        {
            Start();
            return;
        }

        Rigidbody rb = target.GetComponent<Rigidbody>();

        bool grounded = Physics.Raycast(target.position + Vector3.up * 0.1f, Vector3.down, 1.2f, groundLayer);
        float targetPitch = grounded? normalPitch : jumpPitch;

        pitch = Mathf.Lerp(pitch , targetPitch , pitchTransitionSpeed * Time.deltaTime);
        HandleRotation();

        followPosition = Vector3.SmoothDamp(
            followPosition,
            target.position,
            ref followVelocity,
            positionSmoothTime
        );

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 offset = rotation * new Vector3(
            0f,
            height,
            -distance
        );

        Vector3 desiredPosition = followPosition + offset;

        Vector3 direction = desiredPosition - followPosition;

        if (Physics.SphereCast(
            followPosition,
            collisionRadius,
            direction.normalized,
            out RaycastHit hit,
            direction.magnitude,
            collisionLayers,
            QueryTriggerInteraction.Ignore))
        {
            desiredPosition = hit.point + hit.normal * collisionRadius;
        }

        transform.position = desiredPosition;
        transform.rotation = rotation;
    }

    void HandleRotation()
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();

            yaw += delta.x * horizontalSensitivity * Time.deltaTime;
            pitch -= delta.y * verticalSensitivity * Time.deltaTime;
        }

#else

        if (Touchscreen.current != null)
        {
            Touch touch = Touchscreen.current.primaryTouch;

            if (touch.press.isPressed)
            {
                Vector2 position = touch.position.ReadValue();

                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject())
                    return;

                if (position.x < Screen.width * 0.5f)
                    return;

                Vector2 delta = touch.delta.ReadValue();

                yaw += delta.x *
                       horizontalSensitivity *
                       0.02f *
                       Time.deltaTime;

                pitch -= delta.y *
                         verticalSensitivity *
                         0.02f *
                         Time.deltaTime;
            }
        }

#endif

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }
}