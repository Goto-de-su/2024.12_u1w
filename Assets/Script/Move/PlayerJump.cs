using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private JumpParameters parameters;

    private Rigidbody2D rb;
    private float airTimer;
    private Vector2 jumpStartVelocity;
    private bool isInAir;
    private bool isJumping;
    private float jumpStartTime;
    private float jumpHoldTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartJump(Vector2 currentVelocity)
    {
        if (rb.IsTouchingLayers())
        {
            isJumping = true;
            isInAir = true;
            airTimer = 0f;
            jumpStartTime = Time.time;
            jumpHoldTime = 0f;
            jumpStartVelocity = currentVelocity;

            Vector2 jumpVelocity = new Vector2(jumpStartVelocity.x, parameters.initialJumpSpeed);
            rb.linearVelocity = jumpVelocity;
        }
    }

    public void UpdateJump(bool isJumpHeld)
    {
        if (!isJumping) return;

        if (isJumpHeld)
        {
            jumpHoldTime += Time.deltaTime * 60f;

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

        if (isInAir)
        {
            airTimer += Time.deltaTime;

            if (rb.IsTouchingLayers())
            {
                isInAir = false;
                Debug.Log($"Air Time without input: {airTimer:F2} seconds");
            }
        }
    }

    public void HandleAirControl(float inputDirection)
    {
        if (!rb.IsTouchingLayers())
        {
            Vector2 airVelocity = rb.linearVelocity;
            airVelocity.x = parameters.airControlSpeed * inputDirection;
            rb.linearVelocity = airVelocity;
        }
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