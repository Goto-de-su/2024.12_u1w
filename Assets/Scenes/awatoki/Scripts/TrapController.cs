using System.Collections;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    public enum TrapColor { Red, Yellow, Blue }
    public TrapColor trapColor;
     [SerializeField] private float detectionDistance = 20.0f; // 音声を再生する距離

    // NOTE:
    // 2025/01/05 Gogona記載
    // トラップのSESEを追加
    [SerializeField, Tooltip("トラップが開くSE")]
    private AudioClip trapOpenSE;
    [SerializeField, Tooltip("トラップが閉じるSE")]
    private AudioClip trapCloseSE;
    [SerializeField, Tooltip("攻撃SE")]
    private AudioClip trapAttackSE;
    private AudioSource audioSource;

    private bool isWaiting = true; // 罠が未発動の状態
    private bool isActive = false; // 常に起動し続ける状態
    private bool isDeadTrap = false; // Deadタグを持っている状態

    private Animator animator;
    private GameObject player; // プレイヤーオブジェクト

    string waitAnime = "";
    string openAnime = "";
    string closeAnime = "";
    string closedAnime = "";
    string nowAnime = "";
    string oldAnime = "";

    void Start()
    {
        animator = GetComponent<Animator>();
        SetAnimationsBasedOnColor();

        nowAnime = waitAnime;
        oldAnime = waitAnime;

        // プレイヤーオブジェクトを取得
        player = GameObject.FindWithTag("Player");

        // 赤色のトラップの場合、初期状態でアクティブにする
        if (trapColor == TrapColor.Red)
        {
            isActive = true;
            isWaiting = false;
            nowAnime = openAnime; // ゲーム開始時は開いた状態からスタート
            animator.Play(nowAnime);
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // isDeadTrapの状態に応じてタグを更新
        UpdateTrapTag();

        // アクティブ状態がtrueのときにアニメーションを交互に再生
        if (isActive)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // アニメーションが終了したかをチェック
            if (stateInfo.normalizedTime >= 1f)
            {
                float distance = GetDistanceToPlayer();
                if (nowAnime == openAnime)
                {
                    isDeadTrap = true;
                    animator.Play(closeAnime);
                    nowAnime = closeAnime;
                    /* if (distance <= detectionDistance)
                    {
                        SoundManager.instance.PlaySE(SEType.FlowerClose, gameObject);
                    } */
                    audioSource.PlayOneShot(trapCloseSE);
                }
                else if (nowAnime == closeAnime)
                {
                    isDeadTrap = false;
                    animator.Play(openAnime);
                    nowAnime = openAnime;
                    /* if (distance <= detectionDistance)
                    {
                        SoundManager.instance.PlaySE(SEType.FlowerOpen, gameObject);
                    } */
                    audioSource.PlayOneShot(trapOpenSE);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isWaiting)
        {
            float distance = GetDistanceToPlayer();

            // プレイヤータグを持つオブジェクトが触れた場合
            if (collision.CompareTag("Player"))
            {
                StartCoroutine(WaitAndClose(0.5f));
                /* if (distance <= detectionDistance)
                {
                    SoundManager.instance.PlaySE(SEType.Trap, gameObject);
                } */
                audioSource.PlayOneShot(trapAttackSE);

                // 黄色いパックンはwait状態に戻る
                if (trapColor == TrapColor.Yellow)
                {
                    StartCoroutine(WaitAndOpen(4.0f));
                }
            }

            // エネミータグを持つオブジェクトが触れた場合
            else if (collision.CompareTag("Enemy"))
            {
                animator.Play(closeAnime);
                nowAnime = closeAnime;
                isWaiting = false;
                /* if (distance <= detectionDistance)
                {
                    SoundManager.instance.PlaySE(SEType.Trap, gameObject);
                } */
                audioSource.PlayOneShot(trapAttackSE);

                // 触れたエネミーの死亡処理を呼び出す
                EnemyController enemyController = collision.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.Die();
                }

                // 黄色いパックンはwait状態に戻る
                if (trapColor == TrapColor.Yellow)
                {
                    StartCoroutine(WaitAndOpen(4.0f));
                }
            }
        }
    }

    private float GetDistanceToPlayer()
    {
        if (player == null) return float.MaxValue;
        return Vector2.Distance(transform.position, player.transform.position);
    }

    private IEnumerator WaitAndClose(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        animator.Play(closeAnime);
        nowAnime = closeAnime;
        isWaiting = false;
    }

    private IEnumerator WaitAndOpen(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isDeadTrap = true;
        animator.Play(openAnime);
        nowAnime = openAnime;
        isWaiting = true;
    }

    private void UpdateTrapTag()
    {
        if (isDeadTrap)
        {
            gameObject.tag = "Dead";
        }
        else
        {
            gameObject.tag = "Untagged";
        }
    }

    private void SetAnimationsBasedOnColor()
    {
        switch (trapColor)
        {
            case TrapColor.Red:
                waitAnime = "TrapRed_wait";
                openAnime = "TrapRed_open";
                closeAnime = "TrapRed_close";
                closedAnime = "";
                break;
            case TrapColor.Yellow:
                waitAnime = "TrapYellow_wait";
                openAnime = "TrapYellow_open";
                closeAnime = "TrapYellow_close";
                closedAnime = "TrapYellow_closed";
                break;
            case TrapColor.Blue:
                waitAnime = "TrapBlue_wait";
                openAnime = "";
                closeAnime = "TrapBlue_close";
                closedAnime = "TrapBlue_closed";
                break;
        }
    }
}
