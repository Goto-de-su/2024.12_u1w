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
}