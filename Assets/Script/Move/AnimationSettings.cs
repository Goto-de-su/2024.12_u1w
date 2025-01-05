using UnityEngine;

[System.Serializable]
public class AnimationSettings
{
    [Header("Animator Settings")]
    [SerializeField] private RuntimeAnimatorController animatorController;

    [Header("State Animation Clips")]
    [SerializeField] private AnimationClip idleAnimation;
    [SerializeField] private AnimationClip walkAnimation;
    [SerializeField] private AnimationClip runAnimation;
    [SerializeField] private AnimationClip jumpAnimation;
    [SerializeField] private AnimationClip landingAnimation;
    [SerializeField] private AnimationClip deadAnimation;
    [SerializeField] private AnimationClip skillAnimation;  

    [Header("Transition Settings")]
    [SerializeField] private float defaultTransitionDuration = 0.1f;
    [SerializeField] private float quickTransitionDuration = 0.05f;

    // Getters
    public RuntimeAnimatorController AnimatorController => animatorController;
    public AnimationClip IdleAnimation => idleAnimation;
    public AnimationClip WalkAnimation => walkAnimation;
    public AnimationClip RunAnimation => runAnimation;
    public AnimationClip JumpAnimation => jumpAnimation;
    public AnimationClip LandingAnimation => landingAnimation;
    public AnimationClip DeadAnimation => deadAnimation;
    public AnimationClip SkillAnimation => skillAnimation;  

    public float DefaultTransitionDuration => defaultTransitionDuration;
    public float QuickTransitionDuration => quickTransitionDuration;

    public AnimationClip GetClipForState(PlayerStateManager.PlayerState state)
    {
        return state switch
        {
            PlayerStateManager.PlayerState.Idle => idleAnimation,
            PlayerStateManager.PlayerState.Walk => walkAnimation,
            PlayerStateManager.PlayerState.Run => runAnimation,
            PlayerStateManager.PlayerState.Jump => jumpAnimation,
            PlayerStateManager.PlayerState.Landing => landingAnimation,
            PlayerStateManager.PlayerState.Dead => deadAnimation,
            PlayerStateManager.PlayerState.Skill => skillAnimation,  
            _ => idleAnimation
        };
    }

    public float GetTransitionDuration(PlayerStateManager.PlayerState fromState, PlayerStateManager.PlayerState toState)
    {
        // “Á’è‚Ìó‘Ô‘JˆÚ‚Å‚Í‘f‘‚­Ø‚è‘Ö‚¦‚½‚¢ê‡‚È‚Ç‚Ìˆ—
        if (toState == PlayerStateManager.PlayerState.Landing)
        {
            return quickTransitionDuration;
        }

        // ƒXƒLƒ‹ó‘Ô‚Ö‚Ì‘JˆÚ‚à‘f‘‚­
        if (toState == PlayerStateManager.PlayerState.Skill || fromState == PlayerStateManager.PlayerState.Skill)
        {
            return quickTransitionDuration;
        }

        return defaultTransitionDuration;
    }
}