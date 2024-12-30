using UnityEngine;

public class MovementParameters : MonoBehaviour
{
    [Header("Speed Settings")]
    [Tooltip("Initial walking speed in pixels per second")]
    public float initialWalkSpeed = 5f;

    [Tooltip("Maximum walking speed in pixels per second")]
    public float maxWalkSpeed = 10f;

    [Header("Time Settings")]
    [Tooltip("Frames needed to reach maximum speed")]
    public int accelerationFrames = 10;

    [Tooltip("Frames needed to come to a complete stop")]
    public int decelerationFrames = 8;

    [Header("Dash Settings")]
    [Tooltip("Maximum dash speed in pixels per second")]
    public float dashSpeed = 500f;

    [Tooltip("Duration of dash in seconds")]
    public float dashDuration = 0.3f;

    [Tooltip("Cooldown time between dashes in seconds")]
    public float dashCooldown = 1f;

    [Header("Dash Stop Settings")]
    [Tooltip("Time taken to come to a complete stop from dash (seconds)")]
    public float dashStopDecelerationTime = 0.2f;

    [Tooltip("Rate of deceleration during dash stop (0-1)")]
    [Range(0f, 1f)]
    public float dashStopDecelerationRate = 0.8f;
}