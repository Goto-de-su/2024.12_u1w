using System.Collections;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    public enum TrapColor { Red, Yellow, Blue }
    public TrapColor trapColor;

    private bool isWaiting = true;
    private bool isActive = false;

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
                    animator.Play(closeAnime);
                    nowAnime = closeAnime;
                }
                // もし現在のアニメーションがクローズなら次はオープン
                else if (nowAnime == closeAnime)
                {
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
                // 0.5秒待機後、クローズアニメを再生してisWaitingをfalseにする
                StartCoroutine(WaitAndClose(0.5f));
            }

            // エネミータグを持つオブジェクトが触れた場合
            else if (collision.CompareTag("Enemy"))
            {
                // 即座にクローズアニメを再生してisWaitingをfalseにする
                animator.Play(closeAnime);
                nowAnime = closeAnime;
                isWaiting = false;
            }
        }
    }

    private IEnumerator WaitAndClose(float waitTime)
    {
        // 0.5秒待機
        yield return new WaitForSeconds(waitTime);

        // クローズアニメを再生
        animator.Play(closeAnime);
        nowAnime = closeAnime;

        // isWaitingをfalseにして待機状態を終了
        isWaiting = false;
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
