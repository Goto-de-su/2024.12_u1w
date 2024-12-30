using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private MovementParameters parameters;

    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    private float inertiaVelocity;
    private float previousInputDirection;
    private bool isDashStopping;
    private float currentStopTime;
    private float initialStopVelocity;

    public MovementParameters Parameters => parameters;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ResetMovementState();
    }

    private void ResetMovementState()
    {
        isDashStopping = false;
        currentStopTime = 0f;
        inertiaVelocity = 0f;
        previousInputDirection = 0f;
        currentVelocity = Vector2.zero;
    }

    public void UpdateMovement(float inputDirection, bool isDashing)
    {
        if (Mathf.Sign(inputDirection) != Mathf.Sign(previousInputDirection) && inputDirection != 0)
        {
            isDashStopping = false;
            currentStopTime = 0f;
        }

        bool shouldInitiateDashStop = isDashing &&
                                    !isDashStopping &&
                                    previousInputDirection != 0 &&
                                    inputDirection == -previousInputDirection;

        if (shouldInitiateDashStop)
        {
            InitiateDashStop();
        }

        if (isDashStopping)
        {
            UpdateDashStop();
        }
        else
        {
            UpdateNormalMovement(inputDirection, isDashing);
        }

        previousInputDirection = inputDirection;
    }

    private bool IsWallInDirection(float direction)
    {
        if (!rb.IsTouchingLayers()) return false;

        ContactPoint2D[] contacts = new ContactPoint2D[1];
        int contactCount = rb.GetContacts(contacts);
        if (contactCount == 0) return false;

        Vector2 contactNormal = contacts[0].normal;

        Debug.Log($"Wall Check - Direction: {direction}, Contact Normal: {contactNormal}, Position: {transform.position}, Contact Point: {contacts[0].point}");

        return (direction > 0 && contactNormal.x < -0.5f) || (direction < 0 && contactNormal.x > 0.5f);
    }

    private void InitiateDashStop()
    {
        isDashStopping = true;
        currentStopTime = 0f;
        initialStopVelocity = rb.linearVelocity.x;
    }

    private void UpdateDashStop()
    {
        currentStopTime += Time.deltaTime;
        float stopProgress = currentStopTime / parameters.dashStopDecelerationTime;

        if (stopProgress >= 1f)
        {
            isDashStopping = false;
            currentVelocity = new Vector2(0f, rb.linearVelocity.y);
            rb.linearVelocity = currentVelocity;
            return;
        }

        float decelerationMultiplier = Mathf.Pow(1f - parameters.dashStopDecelerationRate, stopProgress);
        float currentSpeed = initialStopVelocity * decelerationMultiplier;
        currentVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
        rb.linearVelocity = currentVelocity;
    }

    private void UpdateNormalMovement(float inputDirection, bool isDashing)
    {
        Debug.Log($"Movement Update - Input: {inputDirection}, IsDashing: {isDashing}, Velocity: {rb.linearVelocity}");

        if (inputDirection == 0)
        {
            ApplyDeceleration();
            return;
        }

        if (!isDashing && IsWallInDirection(inputDirection))
        {
            Debug.Log("Wall detected, stopping movement");
            currentVelocity = new Vector2(0f, rb.linearVelocity.y);
            rb.linearVelocity = currentVelocity;
            return;
        }

        float targetSpeed = isDashing ? parameters.dashSpeed : parameters.maxWalkSpeed;
        targetSpeed *= inputDirection;

        ApplyAcceleration(targetSpeed);
    }

    private void ApplyAcceleration(float targetSpeed)
    {
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
}