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
        // 派生クラスでオーバーライド
    }

    protected virtual void OnTrapActivate()
    {
        isActive = true;
        ApplyDamage();
    }

    protected virtual void ApplyDamage()
    {
        // 将来的なダメージ実装用
        Debug.Log($"{gameObject.name} : Damage Applied");
    }

    protected virtual void DisableCollider()
    {
        trapCollider.enabled = false;
    }
}
