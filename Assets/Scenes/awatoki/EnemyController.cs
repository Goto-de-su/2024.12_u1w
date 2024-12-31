using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyColor { Red, Yellow, Blue } // 敵の色の選択肢
    public EnemyColor enemyColor;

    public int hp = 3;                    // HP
    public float speed = 3.0f;            // 移動速度
    public string direction = "left";     // 向き right or left
    public float reactionDistance = 5.0f; // 反応する距離
    public float range = 0.0f;            // 動き回る範囲
    public float chaseRange = 5.0f;       // プレイヤーを追いかける距離
    Vector3 defPos;                       // 初期位置
    bool isActive = false;                // アクティブフラグ
    bool isDead = false;                  // 死亡フラグ
    private GameObject player;            // プレイヤーオブジェクトのキャッシュ

    // アニメーション対応
    Animator animator; // アニメーター
    string sleepAnime = "";
    string walkAnime = "";
    string deadAnime = "";
    string nowAnime = "";
    string oldAnime = "";

    // Coroutineを管理するための参照
    private Coroutine enemySleepCoroutine;
    private Coroutine enemyMoveCoroutine;

    void Start()
    {
        // アニメーターをとってくる
        animator = GetComponent<Animator>();
        SetAnimationBasedOnColor(); // 色に基づいてアニメーション設定

        nowAnime = sleepAnime;
        oldAnime = sleepAnime;

        // プレイヤーオブジェクトを取得してキャッシュ
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object with tag 'Player' not found!");
        }

        // 初期の向きを設定
        if (direction == "right")
        {
            transform.localScale = new Vector2(-1, 1); // 向きの変更
        }

        // 初期位置
        defPos = transform.position;
    }

    void Update()
    {
        if (player == null || isDead) return; // プレイヤーが見つからない場合や死亡している場合は処理をスキップ

        if (isActive)
        {
            // アニメーション切り替え
            nowAnime = walkAnime;
            if (nowAnime != oldAnime)
            {
                animator.Play(nowAnime);
                oldAnime = nowAnime;
            }

            // プレイヤーとの距離をチェックして追いかける
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist <= chaseRange) // プレイヤーとの距離が追跡範囲内であれば
            {
                // プレイヤーを追いかける
                if (player.transform.position.x < transform.position.x)
                {
                    direction = "left";
                    transform.localScale = new Vector2(1, 1); // 左方向
                }
                else
                {
                    direction = "right";
                    transform.localScale = new Vector2(-1, 1); // 右方向
                }
            }

            // 移動ロジック
            if (range > 0.0f)
            {
                if (transform.position.x < defPos.x - (range / 2))
                {
                    direction = "right";
                    transform.localScale = new Vector2(-1, 1); // 向きの変更
                }
                else if (transform.position.x > defPos.x + (range / 2))
                {
                    direction = "left";
                    transform.localScale = new Vector2(1, 1); // 向きの変更
                }
            }

            // EnemyMove音の管理（5秒に1回繰り返し再生）
            if (enemyMoveCoroutine == null)
            {
                enemyMoveCoroutine = StartCoroutine(PlayEnemyMoveSound());
            }
        }
        else
        {
            // プレイヤーとの距離をチェック
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist < reactionDistance)
            {
                isActive = true;
            }

            // EnemySleep音の管理（12秒に1回繰り返し再生）
            if (enemySleepCoroutine == null)
            {
                enemySleepCoroutine = StartCoroutine(PlayEnemySleepSound());
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead) return; // 死亡時は移動しない

        // Rigidbody2D を取得
        Rigidbody2D rbody = GetComponent<Rigidbody2D>();
        if (rbody == null) return; // Rigidbody2D がアタッチされていない場合は処理をスキップ

        // isActive が true の場合のみ速度を設定
        if (isActive)
        {
            if (direction == "right")
            {
                rbody.linearVelocity = new Vector2(speed, rbody.linearVelocity.y);
            }
            else
            {
                rbody.linearVelocity = new Vector2(-speed, rbody.linearVelocity.y);
            }
        }
        else
        {
            // 動きを止める
            rbody.linearVelocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Deadタグのオブジェクトに触れた場合
        if (collision.CompareTag("Dead"))
        {
            if (!isDead) // すでに死んでいる場合は何もしない
            {
                // 死亡処理
                isDead = true;

                // 移動を停止
                isActive = false;

                // アニメーションをDeadに切り替える
                nowAnime = deadAnime;
                if (nowAnime != oldAnime)
                {
                    animator.Play(nowAnime);
                    oldAnime = nowAnime;
                }

                // Rigidbody2Dの速度も停止
                Rigidbody2D rbody = GetComponent<Rigidbody2D>();
                if (rbody != null)
                {
                    rbody.linearVelocity = Vector2.zero;
                }
            }
        }
        else
        {
            // 死亡していない場合にのみ向きを反転
            if (!isDead)
            {
                // 向きを反転
                if (direction == "right")
                {
                    direction = "left";
                    transform.localScale = new Vector2(1, 1); // 向きの変更
                }
                else
                {
                    direction = "right";
                    transform.localScale = new Vector2(-1, 1); // 向きの変更
                }
            }
        }
    }

    private IEnumerator PlayEnemyMoveSound()
    {
        while (!isDead && isActive)
        {
            SoundManager.instance.PlaySE(SEType.EnemyMove);
            yield return new WaitForSeconds(5f);
        }

        // Coroutineが終了したときは参照をリセット
        enemyMoveCoroutine = null;
    }

    private IEnumerator PlayEnemySleepSound()
    {
        while (!isDead && !isActive)
        {
            SoundManager.instance.PlaySE(SEType.EnemySleep);
            yield return new WaitForSeconds(12f);
        }

        // Coroutineが終了したときは参照をリセット
        enemySleepCoroutine = null;
    }

    // 指定された色に基づいてアニメーション名を設定
    private void SetAnimationBasedOnColor()
    {
        switch (enemyColor)
        {
            case EnemyColor.Red:
                sleepAnime = "Red_sleep";
                walkAnime = "Red_walk";
                deadAnime = "Red_dead";
                break;
            case EnemyColor.Yellow:
                sleepAnime = "Yellow_sleep";
                walkAnime = "Yellow_walk";
                deadAnime = "Yellow_dead";
                break;
            case EnemyColor.Blue:
                sleepAnime = "Blue_sleep";
                walkAnime = "Blue_walk";
                deadAnime = "Blue_dead";
                break;
        }
    }
}
