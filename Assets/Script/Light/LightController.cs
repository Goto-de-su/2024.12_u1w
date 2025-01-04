using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    [SerializeField, Tooltip("光が拡大縮小する速さ")]
    private float zoomSpeed = 1;
    [SerializeField, Tooltip("光が全体を照らしている時間")]
    private float lightUpTime = 3;
    [SerializeField, Tooltip("ライトアップを管理するコンポーネント")]
    private LightStock lightStock;

    private Light2D light2D;
    private float defaultOuterRadius;
    private float defaultInnerRadius;
    private const float maxRadius = 10.5f;
    private bool isZooming = false;
    private float currentRadius;
    private bool canExpand;

    private float lightUpStartTime;
    private float lightUpEndTime;
    private float lightUpTimeCount;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        defaultInnerRadius = light2D.pointLightInnerRadius;
        defaultOuterRadius = light2D.pointLightOuterRadius;
        light2D.pointLightInnerRadius = 0;
        currentRadius = defaultOuterRadius;
        canExpand = true;
    }

    void Update()
    {
        if (isZooming)
        {
            ZoomLight();
        }
    }

    /// <summary>
    /// 全体を照らす
    /// </summary>
    private void LightUp()
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
        if (currentRadius <= maxRadius + defaultOuterRadius && currentRadius >= defaultOuterRadius)
        {
            currentRadius += Time.deltaTime * zoomSpeed;
            light2D.pointLightInnerRadius += Time.deltaTime * zoomSpeed;
            light2D.pointLightOuterRadius = currentRadius;
        }
        // 範囲の拡大縮小が終わったら
        else
        {
            currentRadius = canExpand ? maxRadius + defaultOuterRadius : defaultOuterRadius;
            light2D.pointLightInnerRadius = canExpand ? maxRadius : defaultInnerRadius;
            // maskMaterial.SetFloat("_Radius", currentRadius);
            light2D.pointLightOuterRadius = currentRadius;
            isZooming = false;
            canExpand = !canExpand;
            
            if (canExpand)
            {
                lightUpStartTime = 0;
                lightUpEndTime = 0;
            }
        }
    }

    /// <summary>
    /// ライトを元に戻す
    /// </summary>
    private void RevertLight()
    {
        if (!isZooming && !canExpand)
        {
            isZooming = true;
            zoomSpeed = -zoomSpeed;
        }
    }

    public void SetLightUpStartTime()
    {
        if (Time.time > lightUpEndTime && lightUpStartTime != 0) return;
        lightUpStartTime = Time.time;
        LightUp();
        
    }

    public void SetLightUpEndTime()
    {
        lightUpEndTime = Time.time;
        lightUpTimeCount = lightUpEndTime - lightUpStartTime;

        if (lightUpTimeCount > lightUpTime)
        {
            RevertLight();
        }
        else
        {
            Invoke(nameof(RevertLight), lightUpTime - lightUpTimeCount);
        }
    }
}
