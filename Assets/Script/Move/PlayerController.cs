using UnityEngine;
using UnityEngine.InputSystem;  // InputSystemの名前空間を追加

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerJump jump;
    [SerializeField] private LightController light;
    [SerializeField] private PlayerStateManager stateManager;

    private PlayerControls controls;  // PlayerControlsインスタンス
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator; // アニメーター追加

    private float horizontalInput;
    private bool jumpInput;
    private bool jumpHeld;
    private bool isSkillActive;
    private bool isDead = false; // 死亡フラグ

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();  // Animator取得

        // Animatorが取得できなかった場合に警告を表示
        if (animator == null)
        {
            Debug.LogWarning("Animator is not assigned on the Player object.");
        }

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        controls = new PlayerControls();  // PlayerControlsインスタンス作成
    }

    private void OnEnable()
    {
        controls.Enable();  // 入力を有効にする
    }

    private void OnDisable()
    {
        controls.Disable();  // 入力を無効にする
    }

    private void Update()
    {
        if (!isDead) // 死亡時は入力を受け付けない
        {
            HandleInput();
            UpdateSprite();
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return; // 死亡時は動かない

        bool isRunning = controls.Player.Run.ReadValue<float>() > 0.5f; // ゲームパッドで走り判定
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
            horizontalInput = controls.Player.Move.ReadValue<Vector2>().x;  // ゲームパッドで移動

            if (controls.Player.Jump.triggered)  // ゲームパッドでジャンプ
            {
                jumpInput = true;
            }
            jumpHeld = controls.Player.Jump.ReadValue<float>() > 0.5f;  // ジャンプボタン保持

            Vector2 currentVelocity = movement.GetCurrentVelocity();
            bool isStationary = Mathf.Abs(currentVelocity.x) < 0.1f && Mathf.Abs(currentVelocity.y) < 0.1f;

            if (controls.Player.UseLight.ReadValue<float>() > 0.5f && isStationary)  // ゲームパッドで灯りを使用
            {
                isSkillActive = true;
                stateManager.SetSkillState();
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

        if (controls.Player.UseLight.ReadValue<float>() <= 0.5f && isSkillActive)  // ゲームパッドでスキル解除
        {
            isSkillActive = false;
            stateManager.ClearSkillState();
            if (light != null)
            {
                light.SetLightUpEndTime();
            }
        }
    }

    // ✅ 敵かトラップに触れたらゲームオーバー処理を実行
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Dead"))
        {
            Die();
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

    private void Die()
    {
        isDead = true; // 死亡フラグを立てる

        // 物理挙動を止める
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // 死亡アニメーションを再生
        animator.Play("Player_dying");

        // 3秒後に「死体のアニメーション」に切り替え
        Invoke("SetDeadAnimation", 1.0f);

        // 4秒後にリトライボタンを表示
        Invoke("ShowRetryButton", 3.0f);
    }

    private void SetDeadAnimation()
    {
        animator.Play("Player_dead"); // 死亡ポーズ
    }

    private void ShowRetryButton()
    {
        GameOverUI.instance.ShowRetryButton();
    }
}
