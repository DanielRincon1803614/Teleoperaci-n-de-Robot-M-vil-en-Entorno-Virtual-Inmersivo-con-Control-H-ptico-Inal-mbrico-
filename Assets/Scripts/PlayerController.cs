using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    public float sprintMultiplier = 1.6f;
    public float acceleration = 0.08f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 0.1f;

    public Vector2 LookInput { get; private set; }

    CharacterController controller;
    Vector3 horizontalVelocity;
    Vector3 velocitySmooth;
    float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        LookInput = Cursor.lockState == CursorLockMode.Locked
            ? mouse.delta.ReadValue() * mouseSensitivity
            : Vector2.zero;

        transform.Rotate(0f, LookInput.x, 0f);

        Vector2 move = Vector2.zero;
        if (keyboard.wKey.isPressed) move.y += 1f;
        if (keyboard.sKey.isPressed) move.y -= 1f;
        if (keyboard.dKey.isPressed) move.x += 1f;
        if (keyboard.aKey.isPressed) move.x -= 1f;
        move = Vector2.ClampMagnitude(move, 1f);

        float speed = moveSpeed * (keyboard.leftShiftKey.isPressed ? sprintMultiplier : 1f);
        Vector3 target = (transform.right * move.x + transform.forward * move.y) * speed;
        horizontalVelocity = Vector3.SmoothDamp(horizontalVelocity, target, ref velocitySmooth, acceleration);

        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }
}
