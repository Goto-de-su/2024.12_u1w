using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerJump jump;
    [SerializeField] private InputSettings inputSettings;
    [SerializeField] private new LightController light;
    [SerializeField] private PlayerStateManager stateManager;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float horizontalInput;
    private bool jumpInput;
    private bool jumpHeld;
    private bool isSkillActive;

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
        UpdateSprite();
    }

    private void FixedUpdate()
    {
        bool isRunning = inputSettings.IsRunning();
        bool isGrounded = rb.IsTouchingLayers();

        if (isGrounded)
        {
            movement.UpdateMovement(horizontalInput, isRunning);
        }
        else
        {
            jump.HandleAirControl(horizontalInput);
        }

        jump.UpdateJump(jumpHeld);

        if (jumpInput)
        {
            jump.StartJump(movement.GetCurrentVelocity(), horizontalInput);
            jumpInput = false;
        }
    }

    private void HandleInput()
    {
        if (!isSkillActive)
        {
            float leftInput = inputSettings.IsLeftPressed() ? -1f : 0f;
            float rightInput = inputSettings.IsRightPressed() ? 1f : 0f;
            horizontalInput = leftInput + rightInput;

            if (inputSettings.IsJumpTriggered())
            {
                jumpInput = true;
            }
            jumpHeld = inputSettings.IsJumpPressed();

            Vector2 currentVelocity = movement.GetCurrentVelocity();
            bool isStationary = Mathf.Abs(currentVelocity.x) < 0.1f && Mathf.Abs(currentVelocity.y) < 0.1f;

            if (inputSettings.IsSkillPressed() && isStationary)
            {
                isSkillActive = true;
                stateManager.SetSkillState();  // スキル状態に遷移
                if (light != null)
                {
                    light.SetLightUpStartTime();
                }
            }
        }
        else
        {
            horizontalInput = 0;
            jumpInput = false;
            jumpHeld = false;
        }

        if (inputSettings.IsSkillReleased() && isSkillActive)
        {
            isSkillActive = false;
            stateManager.ClearSkillState();  // スキル状態を解除
            if (light != null)
            {
                light.SetLightUpEndTime();
            }
        }
    }

    private void UpdateSprite()
    {
        if (rb.IsTouchingLayers())
        {
            Vector2 velocity = movement.GetCurrentVelocity();
            if (Mathf.Abs(velocity.x) > 0.1f)
            {
                spriteRenderer.flipX = velocity.x < 0;
            }
        }
    }
}