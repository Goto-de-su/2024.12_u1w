using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private MovementParameters parameters;
    [SerializeField] private PlayerStateManager stateManager;

    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    private float inertiaVelocity;
    private float previousInputDirection;

    public struct MovementState
    {
        public bool isGrounded;
        public Vector2 velocity;
        public float inertiaVelocity;
        public bool isDecelerating;
    }

    public MovementParameters Parameters => parameters;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (stateManager == null)
        {
            stateManager = GetComponent<PlayerStateManager>();
        }

        ResetMovementState();
    }

    private void Start()
    {
        stateManager.SetMaxSpeeds(parameters.maxWalkSpeed, parameters.runSpeed);
    }

    private void ResetMovementState()
    {
        inertiaVelocity = 0f;
        previousInputDirection = 0f;
        currentVelocity = Vector2.zero;
    }

    public void UpdateMovement(float inputDirection, bool isRunning)
    {
        if (Mathf.Sign(inputDirection) != Mathf.Sign(previousInputDirection) && inputDirection != 0)
        {
            inertiaVelocity = 0f;
        }

        if (inputDirection == 0)
        {
            ApplyDeceleration();
        }
        else
        {
            UpdateMovementWithInput(inputDirection, isRunning);
        }

        previousInputDirection = inputDirection;
        currentVelocity = rb.linearVelocity;

        // ó‘Ô‚ðXV
        stateManager.UpdateState(currentVelocity, inputDirection, isRunning);
    }

    private bool IsWallInDirection(float direction)
    {
        if (!rb.IsTouchingLayers()) return false;

        ContactPoint2D[] contacts = new ContactPoint2D[1];
        int contactCount = rb.GetContacts(contacts);
        if (contactCount == 0) return false;

        Vector2 contactNormal = contacts[0].normal;
        return (direction > 0 && contactNormal.x < -0.5f) || (direction < 0 && contactNormal.x > 0.5f);
    }

    private void UpdateMovementWithInput(float inputDirection, bool isRunning)
    {
        if (IsWallInDirection(inputDirection))
        {
            currentVelocity = new Vector2(0f, rb.linearVelocity.y);
            rb.linearVelocity = currentVelocity;
            return;
        }

        float targetSpeed = isRunning ? parameters.runSpeed : parameters.maxWalkSpeed;
        targetSpeed *= inputDirection;

        float speedDifference = targetSpeed - inertiaVelocity;
        float acceleration = speedDifference / parameters.accelerationFrames;
        inertiaVelocity += acceleration;

        currentVelocity = new Vector2(inertiaVelocity, rb.linearVelocity.y);
        rb.linearVelocity = currentVelocity;
    }

    private void ApplyDeceleration()
    {
        if (Mathf.Abs(inertiaVelocity) < 0.1f)
        {
            inertiaVelocity = 0f;
            currentVelocity = new Vector2(0f, rb.linearVelocity.y);
            rb.linearVelocity = currentVelocity;
            return;
        }

        float deceleration = (inertiaVelocity / parameters.decelerationFrames);
        inertiaVelocity -= deceleration;

        currentVelocity = new Vector2(inertiaVelocity, rb.linearVelocity.y);
        rb.linearVelocity = currentVelocity;
    }

    public Vector2 GetCurrentVelocity()
    {
        return currentVelocity;
    }

    public float GetInertiaVelocity()
    {
        return inertiaVelocity;
    }

    public bool IsDecelerating()
    {
        return Mathf.Abs(inertiaVelocity) > 0.1f && previousInputDirection == 0f;
    }

    public MovementState GetCurrentMovementState()
    {
        return new MovementState
        {
            isGrounded = rb.IsTouchingLayers(),
            velocity = currentVelocity,
            inertiaVelocity = inertiaVelocity,
            isDecelerating = IsDecelerating()
        };
    }
}