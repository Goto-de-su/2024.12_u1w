using UnityEngine;

public class MovementParameters : MonoBehaviour
{
    [Header("Speed Settings")]
    [Tooltip("Initial walking speed in pixels per second")]
    public float initialWalkSpeed = 5f;

    [Tooltip("Maximum walking speed in pixels per second")]
    public float maxWalkSpeed = 10f;

    [Tooltip("Maximum running speed in pixels per second")]
    public float runSpeed = 15f;

    [Header("Time Settings")]
    [Tooltip("Frames needed to reach maximum speed")]
    public int accelerationFrames = 10;

    [Tooltip("Frames needed to come to a complete stop")]
    public int decelerationFrames = 8;
}