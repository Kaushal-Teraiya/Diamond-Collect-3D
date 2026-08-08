using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TPSCamera : MonoBehaviour
{
    public Transform target;

    [Header("Distance Settings")]
    public float distance = 6f;
    public float height = 2.5f;

    [Header("Rotation Settings")]
    public float horizontalSensitivity = 120f;
    public float verticalSensitivity = 80f;

    public float minPitch = -10f;
    public float maxPitch = 45f;

    public float smoothSpeed = 10f;

    [Header("Collision Settings")]
    public float collisionRadius = 0.3f;
    public LayerMask collisionLayers;

    float currentYaw = 0f;
    float currentPitch = 15f;

    void LateUpdate()
    {
        if (target == null)
            return;

        HandleRotation();
        FollowPlayerWithCollision();
    }

    void HandleRotation()
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        // New Input System mouse control
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            currentYaw += mouseDelta.x *
                          horizontalSensitivity *
                          Time.deltaTime;

            currentPitch -= mouseDelta.y *
                            verticalSensitivity *
                            Time.deltaTime;
        }

#else

        // Mobile touch control
        if (Touchscreen.current != null)
        {
            Touch touch = Touchscreen.current.primaryTouch;

            if (touch.press.isPressed)
            {
                Vector2 touchPosition = touch.position.ReadValue();

                // Ignore touches on UI
                if (EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject())
                    return;

                // Ignore left side (joystick)
                if (touchPosition.x < Screen.width * 0.5f)
                    return;

                Vector2 delta = touch.delta.ReadValue();

                currentYaw += delta.x *
                              horizontalSensitivity *
                              0.02f *
                              Time.deltaTime;

                currentPitch -= delta.y *
                                verticalSensitivity *
                                0.02f *
                                Time.deltaTime;
            }
        }

#endif

        currentPitch = Mathf.Clamp(
            currentPitch,
            minPitch,
            maxPitch
        );
    }

    void FollowPlayerWithCollision()
    {
        Quaternion rotation =
            Quaternion.Euler(currentPitch, currentYaw, 0f);

        Vector3 offset =
            rotation * new Vector3(0f, height, -distance);

        Vector3 desiredPosition =
            target.position + offset;

        Vector3 direction =
            desiredPosition - target.position;

        if (Physics.SphereCast(
            target.position,
            collisionRadius,
            direction.normalized,
            out RaycastHit hit,
            direction.magnitude,
            collisionLayers))
        {
            desiredPosition =
                hit.point + hit.normal * collisionRadius;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.rotation = rotation;
    }
}