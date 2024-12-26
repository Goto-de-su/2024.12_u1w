using UnityEngine;

[System.Serializable]
public class MovementParameters
{
    [Header("Speed Settings")]
    [Tooltip("Initial walking speed in pixels per second")]
    public float initialWalkSpeed = 100f;

    [Tooltip("Maximum walking speed in pixels per second")]
    public float maxWalkSpeed = 300f;

    [Header("Time Settings")]
    [Tooltip("Frames needed to reach maximum speed")]
    public int accelerationFrames = 10;

    [Tooltip("Frames needed to come to a complete stop")]
    public int decelerationFrames = 8;
}
