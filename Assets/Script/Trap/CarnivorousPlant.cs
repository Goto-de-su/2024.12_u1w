using UnityEngine;

public class CarnivorousPlant : TrapBase
{
    [SerializeField] private float interval = 2f;
    private bool isClosing = false;
    private static readonly int IsClosedParam = Animator.StringToHash("IsClosed");

    private void Start()
    {
        InvokeRepeating(nameof(ToggleTrap), interval, interval);
    }

    private void ToggleTrap()
    {
        isClosing = !isClosing;
        animator.SetBool(IsClosedParam, isClosing);

        if (isClosing)
        {
            OnTrapActivate();
        }
    }
}
