using UnityEngine;

[System.Serializable]
public class JumpParameters
{
    [Header("Jump Heights")]
    [Tooltip("Maximum jump height in pixels")]
    public float maxJumpHeight = 15f;

    [Tooltip("Minimum jump height in pixels")]
    public float minJumpHeight = 10f;

    [Header("Jump Force")]
    [Tooltip("Jump force multiplier relative to gravity (1 means same as fall speed)")]
    [Range(0.1f, 2.0f)]
    public float jumpForceMultiplier = 1.0f;

    [Header("Jump Timing (Based on 60 FPS)")]
    [Tooltip("Number of frames to hold for minimum jump height (at 60 FPS)")]
    [Range(1, 30)]
    public int minJumpHoldFrames = 5;

    [Tooltip("Number of frames to hold for maximum jump height (at 60 FPS)")]
    [Range(1, 60)]
    public int maxJumpHoldFrames = 20;

    [Header("Air Control")]
    [Tooltip("Air control speed in pixels per second")]
    [Range(0f, 5f)]
    public float airControlSpeed = 1f;

    [Header("State-based Jump Forces")]
    [Tooltip("Horizontal force multiplier when jumping while walking")]
    [Range(0.5f, 1.5f)]
    public float walkingJumpForce = 0.8f;

    [Tooltip("Horizontal force multiplier when jumping while running")]
    [Range(1.0f, 2.0f)]
    public float runningJumpForce = 1.2f;

    [Tooltip("Horizontal force multiplier when jumping during deceleration")]
    [Range(0.5f, 1.0f)]
    public float decelerateJumpForce = 0.7f;

    [Tooltip("Force multiplier applied to directional input during jump")]
    [Range(0.5f, 1.5f)]
    public float directionalJumpForce = 0.8f;

    [Header("Inertia Settings")]
    [Tooltip("How much current velocity affects jump direction (0 = none, 1 = full)")]
    [Range(0f, 1f)]
    public float inertiaInfluence = 0.7f;

    [Tooltip("Minimum velocity maintained from inertia")]
    [Range(0f, 0.5f)]
    public float minimumInertia = 0.2f;
}