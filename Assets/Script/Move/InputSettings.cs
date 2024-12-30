using UnityEngine;

[System.Serializable]
public class InputSettings
{
    [System.Serializable]
    public struct MovementKeySet
    {
        public KeyCode left;
        public KeyCode right;
    }

    [Header("Movement Keys")]
    public MovementKeySet primaryMovementKeys = new MovementKeySet
    {
        left = KeyCode.LeftArrow,
        right = KeyCode.RightArrow
    };

    public MovementKeySet alternativeMovementKeys = new MovementKeySet
    {
        left = KeyCode.A,
        right = KeyCode.D
    };

    [Header("Action Keys")]
    public KeyCode primaryJumpKey = KeyCode.Space;
    public KeyCode alternativeJumpKey = KeyCode.W;
    public KeyCode primaryDashKey = KeyCode.LeftShift;
    public KeyCode alternativeDashKey = KeyCode.LeftControl;
    public KeyCode skillKey = KeyCode.E;

    public bool IsLeftPressed()
    {
        return Input.GetKey(primaryMovementKeys.left) ||
               Input.GetKey(alternativeMovementKeys.left);
    }

    public bool IsRightPressed()
    {
        return Input.GetKey(primaryMovementKeys.right) ||
               Input.GetKey(alternativeMovementKeys.right);
    }

    public bool IsJumpPressed()
    {
        return Input.GetKey(primaryJumpKey) ||
               Input.GetKey(alternativeJumpKey);
    }

    public bool IsJumpTriggered()
    {
        return Input.GetKeyDown(primaryJumpKey) ||
               Input.GetKeyDown(alternativeJumpKey);
    }

    public bool IsDashTriggered()
    {
        return Input.GetKeyDown(primaryDashKey) ||
               Input.GetKeyDown(alternativeDashKey);
    }

    public bool IsSkillPressed()
    {
        return Input.GetKeyDown(skillKey);
    }

    public bool IsSkillReleased()
    {
        return Input.GetKeyUp(skillKey);
    }
}