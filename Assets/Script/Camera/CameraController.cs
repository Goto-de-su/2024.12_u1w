using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;  // プレイヤーのTransform
    private Vector3 offset;                     // カメラの初期オフセット

    private void Start()
    {
        // カメラの初期位置からプレイヤーまでの距離を保存
        offset = transform.position - player.position;
    }

    private void LateUpdate()
    {
        // プレイヤーの位置にオフセットを加えた位置にカメラを移動
        transform.position = new Vector3(
            player.position.x + offset.x,
            player.position.y + offset.y,
            offset.z  // Z軸は固定
        );
    }
}
