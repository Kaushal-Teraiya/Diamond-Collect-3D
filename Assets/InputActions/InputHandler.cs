using UnityEngine;
using UnityEngine.InputSystem;
public class InputHandler : MonoBehaviour
{
    private GameInputc input;

    void Awake()
    {
        input = new GameInputc();
    }

    void OnEnable()
    {
        input.Enable();
        input.Gameplay.Click.performed += OnClick;
    }

    void OnDisable()
    {
        input.Gameplay.Click.performed -= OnClick;
        input.Disable();
    }

    void OnClick(InputAction.CallbackContext context)
    {
        Debug.Log("Click");
        Vector2 screenPosition;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.tap.isPressed)
        {
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else
        {
            screenPosition = Mouse.current.position.ReadValue();
        }

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit != null && hit.TryGetComponent<IClickable>(out var clickable))
        {
            clickable.OnClicked();
        }
    }
}
