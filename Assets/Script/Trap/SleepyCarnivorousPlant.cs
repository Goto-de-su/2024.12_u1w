using UnityEngine;

public class SleepyCarnivorousPlant : TrapBase
{
    [SerializeField] private float openDelay = 1f;
    private bool isTriggered = false;
    private static readonly int IsClosedParam = Animator.StringToHash("IsClosed");

    public override void OnPlayerEnter()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            Invoke(nameof(CloseTrap), activationDelay);
        }
    }

    private void CloseTrap()
    {
        animator.SetBool(IsClosedParam, true);
        OnTrapActivate();
        Invoke(nameof(OpenTrap), openDelay);
    }

    private void OpenTrap()
    {
        animator.SetBool(IsClosedParam, false);
        isTriggered = false;
    }
}