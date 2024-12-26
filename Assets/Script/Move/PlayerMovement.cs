using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private MovementParameters parameters;

    private Rigidbody2D rb;
    private float currentSpeed;
    private bool isWalking;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void UpdateMovement(float inputDirection)
    {
        if (Mathf.Abs(inputDirection) > 0.1f)
        {
            isWalking = true;
            currentSpeed = CalculateAcceleration(inputDirection);
        }
        else
        {
            isWalking = false;
            currentSpeed = CalculateDeceleration();
        }

        // 速度を適用
        Vector2 velocity = rb.linearVelocity;
        velocity.x = currentSpeed;
        rb.linearVelocity = velocity;
    }

    private float CalculateAcceleration(float inputDirection)
    {
        float targetSpeed = parameters.maxWalkSpeed * inputDirection;
        float speedDifference = targetSpeed - currentSpeed;
        float acceleration = speedDifference / parameters.accelerationFrames;

        return currentSpeed + acceleration;
    }

    private float CalculateDeceleration()
    {
        if (Mathf.Abs(currentSpeed) < 0.1f)
        {
            return 0f;
        }

        float deceleration = currentSpeed / parameters.decelerationFrames;
        return currentSpeed - deceleration;
    }
}
