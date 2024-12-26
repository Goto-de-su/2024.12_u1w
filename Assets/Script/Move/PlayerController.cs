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

        // �������Z�̐ݒ�
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

        // �󒆂ł̈ړ�����
        if (!rb.IsTouchingLayers())
        {
            jump.HandleAirControl(horizontalInput);
        }
    }

    private void HandleInput()
    {
        // ���������̓���
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // �W�����v�̓���
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
