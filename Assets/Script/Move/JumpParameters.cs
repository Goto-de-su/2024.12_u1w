using UnityEngine;

[System.Serializable]
public class JumpParameters
{
    [Header("Jump Heights")]
    [Tooltip("Maximum jump height in pixels")]
    public float maxJumpHeight = 15f;

    [Tooltip("Minimum jump height in pixels")]
    public float minJumpHeight = 10f;

    [Header("Jump Timing")]
    [Tooltip("Frames needed for minimum jump height")]
    public int minJumpHoldFrames = 5;

    [Tooltip("Frames needed for maximum jump height")]
    public int maxJumpHoldFrames = 20;

    [Header("Air Control")]
    [Tooltip("Horizontal movement speed in air (pixels per second)")]
    public float airControlSpeed = 2f;

    [Header("Physics Settings")]
    [Tooltip("Initial vertical speed for jump (pixels per second)")]
    public float initialJumpSpeed = 500f;

    [Tooltip("Gravity scale for falling")]
    public float gravityScale = 2f;
}