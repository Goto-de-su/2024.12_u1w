using UnityEngine;

public class TrapBase : MonoBehaviour
{
    [SerializeField] protected float activationDelay = 1f;
    protected bool isActive = false;
    protected Animator animator;
    protected Collider2D trapCollider;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        trapCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEnter();
        }
    }

    public virtual void OnPlayerEnter()
    {
        // �h���N���X�ŃI�[�o�[���C�h
    }

    protected virtual void OnTrapActivate()
    {
        isActive = true;
        ApplyDamage();
    }

    protected virtual void ApplyDamage()
    {
        // �����I�ȃ_���[�W�����p
        Debug.Log($"{gameObject.name} : Damage Applied");
    }

    protected virtual void DisableCollider()
    {
        trapCollider.enabled = false;
    }
}
