using UnityEngine;

public class LightUp : MonoBehaviour
{
    [SerializeField, Tooltip("マテリアルの中")]
    private Material maskMaterial;
    [SerializeField, Tooltip("初期半径")]
    private float defaultRadius = 0.03f;
    [SerializeField, Tooltip("光が拡大縮小する速さ")]
    private float zoomSpeed = 1;
    private bool isZoomed = false;
    private float currentRadius;
    private float maxRadius = 0.6f;
    private bool isExpand;

    void Start()
    {
        maskMaterial.SetFloat("_Radius", defaultRadius);
        currentRadius = defaultRadius;
        isExpand = true;
    }

    void Update()
    {
        // デバック用
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnLightUp();
        }
        if (isZoomed)
        {
            ZoomLight();
        }
    }

    /// <summary>
    /// 全体を照らす時に呼び出す
    /// </summary>
    public void OnLightUp()
    {
        isZoomed = true;
        // 拡大するか縮小するかを決める
        zoomSpeed = isExpand ? Mathf.Abs(zoomSpeed) : -Mathf.Abs(zoomSpeed);
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
        else
        {
            currentRadius = isExpand ? maxRadius : defaultRadius;
            maskMaterial.SetFloat("_Radius", currentRadius);
            isZoomed = false;
            isExpand = !isExpand;
        }
    }
}
