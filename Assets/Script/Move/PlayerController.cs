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

        // 物理演算の設定
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

        // 空中での移動制御
        if (!rb.IsTouchingLayers())
        {
            jump.HandleAirControl(horizontalInput);
        }
    }

    private void HandleInput()
    {
        // 水平方向の入力
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // ジャンプの入力
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
