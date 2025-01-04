using UnityEngine;

public class Hedgehog : MonoBehaviour
{
    [SerializeField] private float scaleMultiplier = 0.05f; // 呼吸でどれだけ拡大するか
    [SerializeField] private float breathSpeed = 1.0f; // 呼吸のスピード
    [SerializeField] private float detectionDistance = 20.0f; // 音声を再生する距離

    private Vector3 initialScale;
    private GameObject player; // プレイヤーオブジェクト
    private bool isSoundPlaying = false; // 現在音声が再生中かどうかを追跡

    void Start()
    {
        // 初期スケールを保存
        initialScale = transform.localScale;

        // プレイヤーオブジェクトを取得
        player = GameObject.FindWithTag("Player");

        // 寝息のループ再生を開始
        /// NOTE: 2025/01/05 Gogona記載
        /// ハリネズミが発するSEはSoundRangeAttenuationクラスで調整するため
        /// コメントアウトしています。
        /* SoundManager.instance.PlaySELoop(SEType.HedgehogSleep, gameObject); */
    }

    void Update()
    {
        // 呼吸に合わせてスケールを変化させる
        float scaleChange = Mathf.Sin(Time.time * breathSpeed) * scaleMultiplier;
        transform.localScale = new Vector3(
            initialScale.x + scaleChange,
            initialScale.y + scaleChange,
            initialScale.z
        );
        
        /// NOTE: 2025/01/05 Gogona記載
        /// ハリネズミが発するSEはSoundRangeAttenuationクラスで調整するため
        /// コメントアウトしています。
        /* // プレイヤーとの距離をチェック
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            if (distance <= detectionDistance && !isSoundPlaying)
            {
                // 距離が20以下で音声を再生
                // SoundManager.instance.PlaySELoop(SEType.HedgehogSleep, gameObject);
                isSoundPlaying = true;
            }
            else if (distance > detectionDistance && isSoundPlaying)
            {
                // 距離が20を超えたら音声を停止
                // SoundManager.instance.StopSELoop(gameObject);
                isSoundPlaying = false;
            }
        } */
    }

    /// NOTE: 2025/01/05 Gogona記載
    /// ハリネズミが発するSEはSoundRangeAttenuationクラスで調整するため
    /// コメントアウトしています。
    /* void OnDestroy()
    {
        // オブジェクトが破棄される際にループSEを停止
        // SoundManager.instance.StopSELoop(gameObject);
    } */
}
