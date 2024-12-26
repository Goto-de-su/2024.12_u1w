using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerJump jump;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private float horizontalInput;
    private bool jumpInput;
    private bool jumpHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // •¨—‰‰Z‚Ìİ’è
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
        movement.UpdateMovement(horizontalInput);
        jump.UpdateJump(jumpHeld);

        if (jumpInput)
        {
            jump.StartJump();
            jumpInput = false;
        }

        // ‹ó’†‚Å‚ÌˆÚ“®§Œä
        if (!rb.IsTouchingLayers())
        {
            jump.HandleAirControl(horizontalInput);
        }
    }

    private void HandleInput()
    {
        // …•½•ûŒü‚Ì“ü—Í
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // ƒWƒƒƒ“ƒv‚Ì“ü—Í
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInput = true;
        }
        jumpHeld = Input.GetKey(KeyCode.Space);
    }

    private void UpdateSprite()
    {
        if (horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput < 0;
        }
    }
}
