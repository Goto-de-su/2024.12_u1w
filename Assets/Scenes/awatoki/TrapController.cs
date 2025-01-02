using System.Collections;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    public enum TrapColor { Red, Yellow, Blue }
    public TrapColor trapColor;

    private bool isWaiting = true; // 罠が未発動の状態
    private bool isActive = false; // 常に起動し続ける状態
    private bool isDeadTrap = false; // Deadタグを持っている状態

    private Animator animator;
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

        // 赤色のトラップの場合、初期状態でアクティブにする
        if (trapColor == TrapColor.Red)
        {
            isActive = true;
            isWaiting = false;
            nowAnime = closeAnime; // ゲーム開始時は閉じた状態からスタート
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
            // 現在のアニメーションの状態を確認
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            // アニメーションが終了したかをチェック
            if (stateInfo.normalizedTime >= 1f) // アニメーションの進行が100%を越えたら
            {
                // もし現在のアニメーションがオープンなら次はクローズ
                if (nowAnime == openAnime)
                {
                    isDeadTrap = true;
                    animator.Play(closeAnime);
                    nowAnime = closeAnime;
                }
                // もし現在のアニメーションがクローズなら次はオープン
                else if (nowAnime == closeAnime)
                {
                    isDeadTrap = false;
                    animator.Play(openAnime);
                    nowAnime = openAnime;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isWaiting)
        {
            // プレイヤータグを持つオブジェクトが触れた場合
            if (collision.CompareTag("Player"))
            {
                // 0.5秒待機後、トラップが発動する
                StartCoroutine(WaitAndClose(0.5f));

                // 黄色のトラップは、4秒待機後にwait状態に戻る
                if (trapColor == TrapColor.Yellow)
                {
                    StartCoroutine(WaitAndOpen(4.0f));
                }
            }

            // エネミータグを持つオブジェクトが触れた場合
            else if (collision.CompareTag("Enemy"))
            {
                // 即座にクローズアニメを再生してisWaitingをfalseにする
                animator.Play(closeAnime);
                nowAnime = closeAnime;
                isWaiting = false;

                // 触れたエネミーのEnemyControllerを取得してDie()を呼び出す
                EnemyController enemyController = collision.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.Die();  // エネミーを死亡させる
                }
                
                // 黄色のトラップは、4秒待機後にwait状態に戻る
                if (trapColor == TrapColor.Yellow)
                {
                    StartCoroutine(WaitAndOpen(4.0f));
                }
            }
        }
    }

    private IEnumerator WaitAndClose(float waitTime)
    {
        // 待機時間
        yield return new WaitForSeconds(waitTime);

        // クローズアニメを再生
        animator.Play(closeAnime);
        nowAnime = closeAnime;
        isWaiting = false;
    }

    private IEnumerator WaitAndOpen(float waitTime)
    {
        // 待機時間
        yield return new WaitForSeconds(waitTime);

        // オープンアニメを再生
        isDeadTrap = true;
        animator.Play(openAnime);
        nowAnime = openAnime;
        isWaiting = true;
    }

    private void UpdateTrapTag()
    {
        // isDeadTrapがTrueの場合にDeadタグを付与、Falseの場合はDeadタグを外す
        if (isDeadTrap)
        {
            gameObject.tag = "Dead"; // Deadタグを設定
        }
        else
        {
            gameObject.tag = "Untagged"; // Deadタグを外す
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
