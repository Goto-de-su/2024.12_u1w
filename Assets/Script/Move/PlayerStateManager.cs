using UnityEngine;
using UnityEngine.Events;

public class PlayerStateManager : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,       // 待機状態
        Walk,       // 歩き
        Run,        // 走り
        Jump,       // ジャンプ（上昇・落下含む）
        Landing,    // 着地
        Dead,       // 死亡
        Skill       // スキル使用中
    }

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

    [System.Serializable]
    public class StateChangeContext
    {
        public GroundState groundState;
        public AirState airState;
        public Vector2 velocity;
        public bool isGrounded;
    }

    [System.Serializable]
    public class StateChangeEvent : UnityEvent<PlayerState, PlayerState, StateChangeContext> { }
    public StateChangeEvent onStateChanged = new StateChangeEvent();

    [Header("State Settings")]
    [SerializeField] private float landingStateDuration = 0.5f;

    private PlayerState currentState = PlayerState.Idle;
    private GroundState currentGroundState = GroundState.IDLE;
    private AirState currentAirState = AirState.FALLING;
    private bool isInAir = false;
    private float landingStateTimer = 0f;

    private Rigidbody2D rb;
    private float groundDetectionThreshold = 0.1f;
    private float maxSpeedThreshold = 0.1f;
    private float walkingMaxSpeed = 10f;
    private float runningMaxSpeed = 15f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (currentState == PlayerState.Landing)
        {
            landingStateTimer -= Time.deltaTime;
            if (landingStateTimer <= 0)
            {
                SetState(PlayerState.Idle);
            }
        }
    }

    public void UpdateState(Vector2 velocity, float input, bool isRunning)
    {
        if (currentState == PlayerState.Dead || currentState == PlayerState.Skill) return;

        bool wasInAir = isInAir;
        isInAir = !rb.IsTouchingLayers();

        if (wasInAir && !isInAir)
        {
            SetState(PlayerState.Landing);
            return;
        }

        if (currentState != PlayerState.Landing && currentState != PlayerState.Dead)
        {
            UpdateMovementState(velocity, input, isRunning);
        }
    }

    private void UpdateMovementState(Vector2 velocity, float input, bool isRunning)
    {
        if (isInAir)
        {
            HandleAirState(velocity, input);
            if (currentState != PlayerState.Jump)
            {
                SetState(PlayerState.Jump);
            }
        }
        else
        {
            HandleGroundState(velocity, input, isRunning);
            UpdateMainState(velocity, input, isRunning);
        }
    }

    private void UpdateMainState(Vector2 velocity, float input, bool isRunning)
    {
        if (Mathf.Abs(input) > 0)
        {
            SetState(isRunning ? PlayerState.Run : PlayerState.Walk);
        }
        else if (currentState != PlayerState.Landing)
        {
            SetState(PlayerState.Idle);
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

    private void HandleAirState(Vector2 velocity, float input)
    {
        // アニメーションはJump一つだが、制御用に状態を分ける
        if (velocity.y > 0)
        {
            currentAirState = AirState.JUMP_RISING;
        }
        else
        {
            currentAirState = AirState.FALLING;
        }

        // 空中での横移動の制御のため、入力があればAIR_MOVINGに変更
        if (Mathf.Abs(input) > 0f)
        {
            currentAirState = AirState.AIR_MOVING;
        }
    }

    public void SetState(PlayerState newState)
    {
        if (currentState == newState) return;

        PlayerState oldState = currentState;
        currentState = newState;

        switch (newState)
        {
            case PlayerState.Landing:
                landingStateTimer = landingStateDuration;
                break;
        }

        var context = new StateChangeContext
        {
            groundState = currentGroundState,
            airState = currentAirState,
            velocity = rb != null ? rb.linearVelocity : Vector2.zero,
            isGrounded = !isInAir
        };

        onStateChanged.Invoke(oldState, newState, context);
    }

    public PlayerState GetCurrentState() => currentState;
    public GroundState GetGroundState() => currentGroundState;
    public AirState GetAirState() => currentAirState;
    public bool IsInAir() => isInAir;

    public void SetMaxSpeeds(float walkMax, float runMax)
    {
        walkingMaxSpeed = walkMax;
        runningMaxSpeed = runMax;
    }

    public void SetJumpState()
    {
        SetState(PlayerState.Jump);
        currentAirState = AirState.JUMP_RISING;
        isInAir = true;
    }

    public void SetDeadState()
    {
        SetState(PlayerState.Dead);
    }

    public void SetSkillState()
    {
        if (currentState != PlayerState.Dead && currentState == PlayerState.Idle)
        {
            SetState(PlayerState.Skill);
        }
    }

    public void ClearSkillState()
    {
        if (currentState == PlayerState.Skill)
        {
            SetState(PlayerState.Idle);
        }
    }
}