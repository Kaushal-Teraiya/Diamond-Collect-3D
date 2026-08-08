using UnityEngine;
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
        if (target == null) return;

        HandleRotation();
        FollowPlayerWithCollision();
    }

    void HandleRotation()
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        // PC mouse control
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        currentYaw += mouseX * horizontalSensitivity * Time.deltaTime;

        currentPitch -= mouseY * verticalSensitivity * Time.deltaTime;

#else

        // MOBILE touch control (ignore joystick side touches)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Ignore touches on UI (joystick)
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            if (touch.position.x < Screen.width * 0.5f)
                return; // ignore left side touches (movement joystick)

            if (touch.phase == TouchPhase.Moved)
            {
                currentYaw += touch.deltaPosition.x *
                              horizontalSensitivity * 0.02f * Time.deltaTime;

                currentPitch -= touch.deltaPosition.y *
                                verticalSensitivity * 0.02f * Time.deltaTime;
            }
        }

#endif

        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
    }

    void FollowPlayerWithCollision()
    {
        Quaternion rotation =
            Quaternion.Euler(currentPitch, currentYaw, 0);

        Vector3 offset =
            rotation * new Vector3(0, height, -distance);

        Vector3 desiredPosition =
            target.position + offset;

        Vector3 direction =
            desiredPosition - target.position;

        RaycastHit hit;

        if (Physics.SphereCast(
            target.position,
            collisionRadius,
            direction.normalized,
            out hit,
            distance,
            collisionLayers
        ))
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