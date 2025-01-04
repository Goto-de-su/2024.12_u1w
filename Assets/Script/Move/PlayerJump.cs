using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private const float TARGET_FPS = 60f;

    [SerializeField] private JumpParameters parameters;
    [SerializeField] private Rigidbody2D rb;

    private Vector2 jumpStartVelocity;
    private bool isInAir;
    private bool isJumping;
    private float jumpStartHeight;
    private float jumpHoldTime;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    public void StartJump(Vector2 currentVelocity, bool isRunning)
    {
        if (rb.IsTouchingLayers())
        {
            isJumping = true;
            isInAir = true;
            jumpStartHeight = transform.position.y;
            jumpHoldTime = 0f;

            // 現在の速度をそのまま使用
            jumpStartVelocity = currentVelocity;

            // ジャンプの初速を設定
            Vector2 jumpVelocity = new Vector2(jumpStartVelocity.x, CalculateJumpVelocity());
            rb.linearVelocity = jumpVelocity;
        }
    }

    private float CalculateJumpVelocity()
    {
        float gravityMagnitude = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        return gravityMagnitude * parameters.jumpForceMultiplier * TARGET_FPS * Time.fixedDeltaTime;
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

        if (isInAir && rb.IsTouchingLayers())
        {
            isInAir = false;
        }
    }

    public void HandleAirControl(float inputDirection)
    {
        if (!rb.IsTouchingLayers())
        {
            Vector2 currentVelocity = rb.linearVelocity;

            if (inputDirection != 0)
            {
                // 入力方向に応じて水平速度を調整
                float velocityAdjustment = parameters.airControlSpeed * inputDirection;
                currentVelocity.x += velocityAdjustment;

                // ジャンプ開始時の速度を上限として制限
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