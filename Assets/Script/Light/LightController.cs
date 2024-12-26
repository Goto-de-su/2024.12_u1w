using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField, Tooltip("マスクのマテリアル")]
    private Material maskMaterial;
    [SerializeField, Tooltip("初期半径")]
    private float defaultRadius = 0.03f;
    [SerializeField, Tooltip("光が拡大縮小する速さ")]
    private float zoomSpeed = 1;
    [SerializeField, Tooltip("光が全体を照らしている時間")]
    private float lightUpTime = 3;
    [SerializeField, Tooltip("ライトアップを管理するコンポーネント")]
    private LightStock lightStock;

    private bool isZooming = false;
    private float currentRadius;
    private float maxRadius = 0.6f;
    private bool canExpand;

    void Start()
    {
        maskMaterial.SetFloat("_Radius", defaultRadius);
        currentRadius = defaultRadius;
        canExpand = true;
    }

    void Update()
    {
        // デバック用
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LightUp();
        }
        if (isZooming)
        {
            ZoomLight();
        }
    }

    /// <summary>
    /// 全体を照らす
    /// </summary>
    public void LightUp()
    {
        // ズーム中でなく、拡大可能でストックがあるなら
        if (!isZooming && canExpand && lightStock.currentStock > 0)
        {
            lightStock.UseLight();
            isZooming = true;
            // 拡大するか縮小するかを決める
            zoomSpeed = Mathf.Abs(zoomSpeed);
        }
    }

    /// <summary>
    /// 光の範囲を拡大縮小する
    /// </summary>
    void ZoomLight()
    {
        if (currentRadius <= maxRadius && currentRadius >= defaultRadius)
        {
            currentRadius += Time.deltaTime * zoomSpeed;
            maskMaterial.SetFloat("_Radius", currentRadius);
        }
        // 範囲の拡大縮小が終わったら
        else
        {
            currentRadius = canExpand ? maxRadius : defaultRadius;
            maskMaterial.SetFloat("_Radius", currentRadius);
            isZooming = false;
            canExpand = !canExpand;
            if (!canExpand) Invoke(nameof(RevertLight), lightUpTime);
        }
    }

    /// <summary>
    /// ライトを元に戻す
    /// </summary>
    private void RevertLight()
    {
        isZooming = true;
        zoomSpeed = -zoomSpeed;
    }
}
