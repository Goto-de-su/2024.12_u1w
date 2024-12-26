using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private JumpParameters parameters;

    private Rigidbody2D rb;
    private float jumpStartTime;
    private float jumpHoldTime;
    private bool isJumping;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = parameters.gravityScale;
    }

    public void StartJump()
    {
        if (rb.IsTouchingLayers())
        {
            isJumping = true;
            jumpStartTime = Time.time;
            jumpHoldTime = 0f;

            // 初速度を設定
            Vector2 velocity = rb.linearVelocity;
            velocity.y = parameters.initialJumpSpeed;
            rb.linearVelocity = velocity;
        }
    }

    public void UpdateJump(bool isJumpHeld)
    {
        if (!isJumping) return;

        if (isJumpHeld)
        {
            jumpHoldTime += Time.deltaTime * 60f; // フレーム数に変換

            // 最大ホールド時間を超えた場合
            if (jumpHoldTime >= parameters.maxJumpHoldFrames)
            {
                isJumping = false;
            }
        }
        else
        {
            // ボタンを離した場合
            isJumping = false;
        }

        // ジャンプの高さを調整
        float jumpHeightRatio = Mathf.Clamp01((jumpHoldTime - parameters.minJumpHoldFrames) /
            (parameters.maxJumpHoldFrames - parameters.minJumpHoldFrames));
        float targetHeight = Mathf.Lerp(parameters.minJumpHeight, parameters.maxJumpHeight, jumpHeightRatio);

        AdjustJumpHeight(targetHeight);
    }

    public void HandleAirControl(float inputDirection)
    {
        // 空中での水平移動
        Vector2 velocity = rb.linearVelocity;
        velocity.x = parameters.airControlSpeed * inputDirection;
        rb.linearVelocity = velocity;
    }

    private void AdjustJumpHeight(float targetHeight)
    {
        if (!isJumping || rb.linearVelocity.y <= 0) return;

        float currentHeight = transform.position.y - jumpStartTime;
        if (currentHeight >= targetHeight)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.y = 0;
            rb.linearVelocity = velocity;
            isJumping = false;
        }
    }
}
