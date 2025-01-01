﻿using System.Collections;
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

        // 初期位置
        defPos = transform.position;

        // 初期の向き
        if (direction == "right")
        {
            transform.localScale = new Vector2(-1, 1); // 向きの変更
        }

        // 赤色の敵の場合、初期状態でアクティブにする
        if (enemyColor == EnemyColor.Red)
        {
            isActive = true;
        }
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
        }
        else
        {
            // プレイヤーとの距離をチェック
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist < reactionDistance)
            {
                isActive = true; //起きている状態に変更

                // このエネミーに関連する全てのSEを停止（寝息を止める）
                SoundManager.instance.StopSE(gameObject);
                SoundManager.instance.StopSELoop(gameObject);
            }

            // 効果音の判定
            if (!isDead) //死んでいない
            {
                if (isActive) //起きている
                {
                    SoundManager.instance.PlaySELoop(SEType.EnemyMove, gameObject);  // 足音を再生
                }
                if (!isActive && enemyColor != EnemyColor.Blue) //寝ている && 敵が青じゃない(青は寝息しない)
                {
                    SoundManager.instance.PlaySELoop(SEType.EnemySleep, gameObject);  // 寝息を再生
                }
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

    void OnTriggerEnter2D(Collider2D collision)
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

                // このエネミーに関連する全てのSEを停止
                SoundManager.instance.StopSE(gameObject);
                SoundManager.instance.StopSELoop(gameObject);

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