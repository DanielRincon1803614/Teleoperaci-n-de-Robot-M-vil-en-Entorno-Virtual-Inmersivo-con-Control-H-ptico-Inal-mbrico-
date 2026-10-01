using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RobotController : MonoBehaviour
{
    public float maxLinearSpeed = 1.5f;
    public float maxAngularSpeed = 90f;
    public float acceleration = 4f;
    public bool useKeyboard = true;

    public float LinearInput { get; set; }
    public float AngularInput { get; set; }
    public float CurrentSpeed { get; private set; }

    public event Action<float> Collided;

    Rigidbody rb;
    float currentLinear;
    float currentAngular;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        if (!useKeyboard || Keyboard.current == null) return;
        var k = Keyboard.current;
        LinearInput = (k.upArrowKey.isPressed ? 1f : 0f) - (k.downArrowKey.isPressed ? 1f : 0f);
        AngularInput = (k.rightArrowKey.isPressed ? 1f : 0f) - (k.leftArrowKey.isPressed ? 1f : 0f);
    }

    void FixedUpdate()
    {
        float step = acceleration * Time.fixedDeltaTime;
        currentLinear = Mathf.MoveTowards(currentLinear, Mathf.Clamp(LinearInput, -1f, 1f) * maxLinearSpeed, step * maxLinearSpeed);
        currentAngular = Mathf.MoveTowards(currentAngular, Mathf.Clamp(AngularInput, -1f, 1f) * maxAngularSpeed, step * maxAngularSpeed);

        Vector3 forward = transform.forward * currentLinear;
        rb.linearVelocity = new Vector3(forward.x, rb.linearVelocity.y, forward.z);
        rb.angularVelocity = new Vector3(0f, currentAngular * Mathf.Deg2Rad, 0f);
        CurrentSpeed = currentLinear;
    }

    void OnCollisionEnter(Collision collision)
    {
        Collided?.Invoke(collision.relativeVelocity.magnitude);
    }
}