using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private PlayerStateManager playerStateManager;

    private void Awake()
    {
        playerStateManager = GetComponent<PlayerStateManager>();
        if (animator == null)
        {
            animator = GetComponent<Animator>(); // Animatorコンポーネントを取得
        }

        // 状態が変更されたときにアニメーションを更新
        playerStateManager.onStateChanged.AddListener(UpdateAnimation);
    }

    private void UpdateAnimation(PlayerStateManager.PlayerState oldState, PlayerStateManager.PlayerState newState, PlayerStateManager.StateChangeContext context)
    {
        ResetAnimatorParameters(); // すべてのパラメータをリセット

        switch (newState)
        {
            case PlayerStateManager.PlayerState.Idle:
                animator.SetBool("Idle", true);
                break;

            case PlayerStateManager.PlayerState.Walk:
                animator.SetBool("Walk", true);
                break;

            case PlayerStateManager.PlayerState.Run:
                animator.SetBool("Run", true);
                break;

            case PlayerStateManager.PlayerState.Jump:
                animator.SetTrigger("Jump");
                break;

            case PlayerStateManager.PlayerState.Landing:
                animator.SetTrigger("Landing");
                break;

            case PlayerStateManager.PlayerState.Dead:
                animator.SetTrigger("Dead");
                break;

            case PlayerStateManager.PlayerState.Skill:
                animator.SetTrigger("Skill");
                break;
        }
    }

    private void ResetAnimatorParameters()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
    }
}
