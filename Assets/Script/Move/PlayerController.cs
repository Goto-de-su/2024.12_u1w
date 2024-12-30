using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerJump jump;
    [SerializeField] private InputSettings inputSettings;
    [SerializeField] private LightController light;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float horizontalInput;
    private bool jumpInput;
    private bool jumpHeld;
    private bool isDashing;
    private bool isSkillActive;
    private float dashCooldownTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Update()
    {
        HandleInput();
        UpdateDashCooldown();
        UpdateSprite();
    }

    private void FixedUpdate()
    {
        movement.UpdateMovement(horizontalInput, isDashing);
        jump.UpdateJump(jumpHeld);

        if (jumpInput)
        {
            jump.StartJump(movement.GetCurrentVelocity());
            jumpInput = false;
        }

        if (!rb.IsTouchingLayers())
        {
            jump.HandleAirControl(horizontalInput);
        }
    }

    private void HandleInput()
    {
        float leftInput = inputSettings.IsLeftPressed() ? -1 : 0;
        float rightInput = inputSettings.IsRightPressed() ? 1 : 0;
        horizontalInput = leftInput + rightInput;

        if (inputSettings.IsJumpPressed())
        {
            jumpInput = true;
        }
        jumpHeld = inputSettings.IsJumpPressed();

        if (inputSettings.IsDashTriggered() && dashCooldownTimer <= 0)
        {
            StartDash();
        }

        if (inputSettings.IsSkillPressed() && !isSkillActive)
        {
            isSkillActive = true;
            if (light != null)
            {
                light.SetLightUpStartTime();
            }
        }
        else if (inputSettings.IsSkillReleased() && isSkillActive)
        {
            isSkillActive = false;
            if (light != null)
            {
                light.SetLightUpEndTime();
            }
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashCooldownTimer = movement.Parameters.dashCooldown;
        Invoke(nameof(EndDash), movement.Parameters.dashDuration);
    }

    private void EndDash()
    {
        isDashing = false;
    }

    private void UpdateDashCooldown()
    {
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void UpdateSprite()
    {
        Vector2 velocity = movement.GetCurrentVelocity();
        if (Mathf.Abs(velocity.x) > 0.1f)
        {
            spriteRenderer.flipX = velocity.x < 0;
        }
    }
}