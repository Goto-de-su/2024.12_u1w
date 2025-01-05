using System.Collections;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    public enum TrapColor { Red, Yellow, Blue }
    public TrapColor trapColor;

    [SerializeField] private float detectionDistance = 20.0f; // 音声を再生する距離
    [SerializeField] private float resetTime = 4.0f; // 黄色のリセット待機時間
    [SerializeField] private float trapInterval = 2.0f; // 赤色のインターバル

    private bool isWaiting = true; // 罠が未発動の状態
    private bool isActive = false; // 常に起動し続ける状態
    private bool isDeadTrap = false; // Deadタグを持っている状態

    private Animator animator;
    private GameObject player; // プレイヤーオブジェクト
    private Coroutine activeTrapCoroutine;

    private string waitAnime = "";
    private string openAnime = "";
    private string closeAnime = "";
    private string closedAnime = "";
    private string nowAnime = "";
    private string oldAnime = "";

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
    }

    void Update()
    {
        // isDeadTrapの状態に応じてタグを更新
        UpdateTrapTag();

        // アクティブ状態がtrueのときにアニメーションを交互に再生
        if (isActive)
        {
            if (activeTrapCoroutine == null)
            {
                activeTrapCoroutine = StartCoroutine(ActiveTrap());
            }
        }
        else
        {
            if (activeTrapCoroutine != null)
            {
                StopCoroutine(activeTrapCoroutine);
                activeTrapCoroutine = null;
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
                if (distance <= detectionDistance)
                {
                    SoundManager.instance.PlaySE(SEType.Trap, gameObject);
                }

                // 黄色いパックンはwait状態に戻る
                if (trapColor == TrapColor.Yellow)
                {
                    StartCoroutine(WaitAndOpen(resetTime));
                }
            }

            // エネミータグを持つオブジェクトが触れた場合
            else if (collision.CompareTag("Enemy"))
            {
                animator.Play(closeAnime);
                nowAnime = closeAnime;
                isWaiting = false;
                if (distance <= detectionDistance)
                {
                    SoundManager.instance.PlaySE(SEType.Trap, gameObject);
                }

                // 触れたエネミーの死亡処理を呼び出す
                EnemyController enemyController = collision.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.Die();
                }

                // 黄色いパックンはwait状態に戻る
                if (trapColor == TrapColor.Yellow)
                {
                    StartCoroutine(WaitAndOpen(resetTime));
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

        // 黄色いトラップがクローズした場合に再度オープンする処理
        if (trapColor == TrapColor.Yellow)
        {
            StopCoroutine(nameof(WaitAndOpen)); // 既存のコルーチンを停止
            StartCoroutine(WaitAndOpen(resetTime)); // 再度オープン
        }
    }

    private IEnumerator WaitAndOpen(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isDeadTrap = false; // "Dead" タグを解除
        animator.Play(openAnime);
        nowAnime = openAnime;
        isWaiting = true;
    }

    private IEnumerator ActiveTrap()
    {
        while (isActive)
        {
            // ①クローズアニメを再生
            yield return StartCoroutine(WaitAndClose(0f));

            // ②2秒待つ
            yield return new WaitForSeconds(trapInterval);

            // ③オープンアニメを再生
            yield return StartCoroutine(WaitAndOpen(0f));

            // ④0秒待つ
            yield return new WaitForSeconds(0f);
        }
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
