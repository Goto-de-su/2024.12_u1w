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

        // Animator�̐ݒ�
        if (animationSettings.AnimatorController != null)
        {
            animator.runtimeAnimatorController = animationSettings.AnimatorController;
        }

        // StateManager�̃C�x���g���w��
        stateManager.onStateChanged.AddListener(OnPlayerStateChanged);
    }

    private void OnPlayerStateChanged(PlayerStateManager.PlayerState oldState,
                                    PlayerStateManager.PlayerState newState,
                                    PlayerStateManager.StateChangeContext context)
    {
        // �A�j���[�V�����N���b�v�̎擾�ƍĐ�
        AnimationClip newClip = animationSettings.GetClipForState(newState);
        if (newClip != null)
        {
            PlayAnimation(newState, newClip);
        }

        // �A�j���[�^�[�p�����[�^�̍X�V
        UpdateAnimatorParameters(context);
    }

    private void PlayAnimation(PlayerStateManager.PlayerState state, AnimationClip clip)
    {
        if (animator == null || clip == null) return;

        float transitionDuration = animationSettings.GetTransitionDuration(
            stateManager.GetCurrentState(),
            state
        );

        // �A�j���[�V�����̍Đ�
        animator.CrossFade(clip.name, transitionDuration);

        // �񃋁[�v�A�j���[�V�����̏ꍇ�A�����C�x���g���X�P�W���[��
        if (!clip.isLooping)
        {
            float duration = clip.length / animator.speed;
            Invoke(nameof(OnCurrentAnimationComplete), duration);
        }
    }

    private void UpdateAnimatorParameters(PlayerStateManager.StateChangeContext context)
    {
        if (animator == null) return;

        // ���x�p�����[�^�̍X�V
        animator.SetFloat(currentSpeedParameter, Mathf.Abs(context.velocity.x));
        animator.SetFloat(verticalSpeedParameter, context.velocity.y);

        // �n�㔻��̍X�V
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