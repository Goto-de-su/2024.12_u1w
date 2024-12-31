using UnityEngine;

public class BearTrap: TrapBase
{
    private static readonly int IsClosedParam = Animator.StringToHash("IsClosed");

    public override void OnPlayerEnter()
    {
        if (!isActive)
        {
            Invoke(nameof(CloseTrap), activationDelay);
        }
    }

    private void CloseTrap()
    {
        animator.SetBool(IsClosedParam, true);
        OnTrapActivate();
        DisableCollider();
    }
}