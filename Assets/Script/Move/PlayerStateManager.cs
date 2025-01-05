using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public enum GroundState
    {
        IDLE,
        WALK_ACCELERATING,
        WALK_MAX_SPEED,
        RUN_ACCELERATING,
        RUN_MAX_SPEED,
        DECELERATE
    }

    public enum AirState
    {
        JUMP_RISING,
        FALLING,
        AIR_MOVING
    }

    private GroundState currentGroundState = GroundState.IDLE;
    private AirState currentAirState = AirState.FALLING;
    private bool isInAir = false;

    private Rigidbody2D rb;
    private float groundDetectionThreshold = 0.1f;
    private float maxSpeedThreshold = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void UpdateState(Vector2 velocity, float input, bool isRunning)
    {
        // �ڒn����̍X�V
        bool wasInAir = isInAir;
        isInAir = !rb.IsTouchingLayers();

        // ��/�n���Ԃ̑J�ڏ���
        if (isInAir)
        {
            HandleAirState(velocity);
        }
        else
        {
            HandleGroundState(velocity, input, isRunning);

            // ���n���̏���
            if (wasInAir)
            {
                currentAirState = AirState.FALLING;
            }
        }
    }

    private void HandleGroundState(Vector2 velocity, float input, bool isRunning)
    {
        float absVelocityX = Mathf.Abs(velocity.x);

        if (Mathf.Abs(input) > 0)
        {
            if (isRunning)
            {
                currentGroundState = absVelocityX >= runningMaxSpeed - maxSpeedThreshold
                    ? GroundState.RUN_MAX_SPEED
                    : GroundState.RUN_ACCELERATING;
            }
            else
            {
                currentGroundState = absVelocityX >= walkingMaxSpeed - maxSpeedThreshold
                    ? GroundState.WALK_MAX_SPEED
                    : GroundState.WALK_ACCELERATING;
            }
        }
        else if (absVelocityX > groundDetectionThreshold)
        {
            currentGroundState = GroundState.DECELERATE;
        }
        else
        {
            currentGroundState = GroundState.IDLE;
        }
    }

    private void HandleAirState(Vector2 velocity)
    {
        if (velocity.y > 0)
        {
            currentAirState = AirState.JUMP_RISING;
        }
        else if (Mathf.Abs(velocity.x) > groundDetectionThreshold)
        {
            currentAirState = AirState.AIR_MOVING;
        }
        else
        {
            currentAirState = AirState.FALLING;
        }
    }

    public void SetJumpState()
    {
        currentAirState = AirState.JUMP_RISING;
        isInAir = true;
    }

    public GroundState GetGroundState()
    {
        return currentGroundState;
    }

    public AirState GetAirState()
    {
        return currentAirState;
    }

    public bool IsInAir()
    {
        return isInAir;
    }

    private float walkingMaxSpeed = 10f;  // MovementParameters����擾�\��
    private float runningMaxSpeed = 15f;  // MovementParameters����擾�\��

    // �O������MaxSpeed�l��ݒ肷�郁�\�b�h�iMovementParameters����ݒ肳���j
    public void SetMaxSpeeds(float walkMax, float runMax)
    {
        walkingMaxSpeed = walkMax;
        runningMaxSpeed = runMax;
    }
}