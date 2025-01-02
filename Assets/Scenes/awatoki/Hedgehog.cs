using UnityEngine;

public class Hedgehog : MonoBehaviour
{
    [SerializeField] private float scaleMultiplier = 0.05f; // 呼吸でどれだけ拡大するか
    [SerializeField] private float breathSpeed = 1.0f; // 呼吸のスピード

    private Vector3 initialScale;

    void Start()
    {
        // 初期スケールを保存
        initialScale = transform.localScale;

        // 寝息のループ再生を開始
        SoundManager.instance.PlaySELoop(SEType.HedgehogSleep, gameObject);
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
    }

    void OnDestroy()
    {
        // オブジェクトが破棄される際にループSEを停止
        SoundManager.instance.StopSELoop(gameObject);
    }
}
