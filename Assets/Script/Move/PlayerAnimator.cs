using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerStateManager stateManager;
    [SerializeField] private AnimationSettings animationSettings;

    [Header("Animator Parameters")]
    [SerializeField] private string currentSpeedParameter = "CurrentSpeed";
    [SerializeField] private string verticalSpeedParameter = "VerticalSpeed";
    [SerializeField] private string isGroundedParameter = "IsGrounded";

    public UnityEvent onAnimationComplete = new UnityEvent();

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (stateManager == null)
            stateManager = GetComponent<PlayerStateManager>();

        // Animatorの設定
        if (animationSettings.AnimatorController != null)
        {
            animator.runtimeAnimatorController = animationSettings.AnimatorController;
        }

        // StateManagerのイベントを購読
        stateManager.onStateChanged.AddListener(OnPlayerStateChanged);
    }

    private void OnPlayerStateChanged(PlayerStateManager.PlayerState oldState,
                                    PlayerStateManager.PlayerState newState,
                                    PlayerStateManager.StateChangeContext context)
    {
        // アニメーションクリップの取得と再生
        AnimationClip newClip = animationSettings.GetClipForState(newState);
        if (newClip != null)
        {
            PlayAnimation(newState, newClip);
        }

        // アニメーターパラメータの更新
        UpdateAnimatorParameters(context);
    }

    private void PlayAnimation(PlayerStateManager.PlayerState state, AnimationClip clip)
    {
        if (animator == null || clip == null) return;

        float transitionDuration = animationSettings.GetTransitionDuration(
            stateManager.GetCurrentState(),
            state
        );

        // アニメーションの再生
        animator.CrossFade(clip.name, transitionDuration);

        // 非ループアニメーションの場合、完了イベントをスケジュール
        if (!clip.isLooping)
        {
            float duration = clip.length / animator.speed;
            Invoke(nameof(OnCurrentAnimationComplete), duration);
        }
    }

    private void UpdateAnimatorParameters(PlayerStateManager.StateChangeContext context)
    {
        if (animator == null) return;

        // 速度パラメータの更新
        animator.SetFloat(currentSpeedParameter, Mathf.Abs(context.velocity.x));
        animator.SetFloat(verticalSpeedParameter, context.velocity.y);

        // 地上判定の更新
        animator.SetBool(isGroundedParameter, context.isGrounded);
    }

    public void InterruptAnimation()
    {
        CancelInvoke(nameof(OnCurrentAnimationComplete));
    }

    private void OnCurrentAnimationComplete()
    {
        onAnimationComplete.Invoke();
    }

    private void OnDestroy()
    {
        if (stateManager != null)
        {
            stateManager.onStateChanged.RemoveListener(OnPlayerStateChanged);
        }
    }
}