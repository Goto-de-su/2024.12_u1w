using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private const float TARGET_FPS = 60f;

    [SerializeField] private JumpParameters parameters;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerStateManager stateManager;

    private Vector2 jumpVelocity;
    private Vector2 jumpStartVelocity;
    private float inertiaVelocity;
    private bool isJumping;
    private float jumpStartHeight;
    private float jumpHoldTime;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (stateManager == null)
        {
            stateManager = GetComponent<PlayerStateManager>();
        }
    }

    public void StartJump(Vector2 currentVelocity, float inputDirection)
    {
        if (rb.IsTouchingLayers())
        {
            isJumping = true;
            jumpStartHeight = transform.position.y;
            jumpHoldTime = 0f;
            jumpStartVelocity = currentVelocity;

            // ��ԂɊ�Â����W�����v���x�̌v�Z
            jumpVelocity = CalculateJumpVelocity(currentVelocity, inputDirection);
            rb.linearVelocity = jumpVelocity;

            // ��Ԃ̍X�V
            stateManager.SetJumpState();
        }
    }

    private Vector2 CalculateJumpVelocity(Vector2 currentVelocity, float inputDirection)
    {
        float verticalVelocity = CalculateVerticalVelocity();
        float horizontalVelocity = CalculateHorizontalVelocity(currentVelocity, inputDirection);

        return new Vector2(horizontalVelocity, verticalVelocity);
    }

    private float CalculateVerticalVelocity()
    {
        float gravityMagnitude = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        return gravityMagnitude * parameters.jumpForceMultiplier * TARGET_FPS * Time.fixedDeltaTime;
    }

    private float CalculateHorizontalVelocity(Vector2 currentVelocity, float inputDirection)
    {
        // �����̉e�����v�Z
        float inertiaVelocity = currentVelocity.x * parameters.inertiaInfluence;
        if (Mathf.Abs(inertiaVelocity) < parameters.minimumInertia)
        {
            inertiaVelocity = Mathf.Sign(inertiaVelocity) * parameters.minimumInertia;
        }

        // ��ԂɊ�Â����W�����v�͂̌���
        float stateBasedForce = GetStateBasedJumpForce();

        // ���͕����̉e�����v�Z
        float inputForce = inputDirection * parameters.directionalJumpForce * stateBasedForce;

        // �����Ɠ��͂�g�ݍ��킹�čŏI�I�Ȑ������x������
        return inertiaVelocity + inputForce;
    }

    private float GetStateBasedJumpForce()
    {
        switch (stateManager.GetGroundState())
        {
            case PlayerStateManager.GroundState.WALK_ACCELERATING:
            case PlayerStateManager.GroundState.WALK_MAX_SPEED:
                return parameters.walkingJumpForce;

            case PlayerStateManager.GroundState.RUN_ACCELERATING:
            case PlayerStateManager.GroundState.RUN_MAX_SPEED:
                return parameters.runningJumpForce;

            case PlayerStateManager.GroundState.DECELERATE:
                return parameters.decelerateJumpForce;

            default:
                return parameters.walkingJumpForce;
        }
    }

    public void UpdateJump(bool isJumpHeld)
    {
        if (!isJumping) return;

        if (isJumpHeld)
        {
            jumpHoldTime += Time.deltaTime * TARGET_FPS;

            if (jumpHoldTime >= parameters.maxJumpHoldFrames)
            {
                isJumping = false;
            }
        }
        else
        {
            isJumping = false;
        }

        float jumpHeightRatio = Mathf.Clamp01(
            (jumpHoldTime - parameters.minJumpHoldFrames) /
            (parameters.maxJumpHoldFrames - parameters.minJumpHoldFrames)
        );

        float targetHeight = Mathf.Lerp(
            parameters.minJumpHeight,
            parameters.maxJumpHeight,
            jumpHeightRatio
        );

        AdjustJumpHeight(targetHeight);
    }

    public void HandleAirControl(float inputDirection)
    {
        if (!rb.IsTouchingLayers())
        {
            Vector2 currentVelocity = rb.linearVelocity;

            if (inputDirection != 0)
            {
                float velocityAdjustment = parameters.airControlSpeed * inputDirection;
                currentVelocity.x += velocityAdjustment;

                float maxSpeed = Mathf.Abs(jumpStartVelocity.x);
                currentVelocity.x = Mathf.Clamp(currentVelocity.x, -maxSpeed, maxSpeed);
            }

            rb.linearVelocity = currentVelocity;
        }
    }

    private void AdjustJumpHeight(float targetHeight)
    {
        if (!isJumping || rb.linearVelocity.y <= 0) return;

        float currentHeight = transform.position.y - jumpStartHeight;
        if (currentHeight >= targetHeight)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.y = 0;
            rb.linearVelocity = velocity;
            isJumping = false;
        }
    }
}