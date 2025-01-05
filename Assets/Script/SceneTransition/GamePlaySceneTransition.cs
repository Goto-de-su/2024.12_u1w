using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlaySceneTransition : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName = "NextScene"; // インスペクターで設定する次のシーン名

    [SerializeField]
    private string playerTag = "Player"; // プレイヤーのタグ

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したオブジェクトがプレイヤーかどうかを確認
        if (collision.gameObject.CompareTag(playerTag))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        // 次のシーンをロード
        SceneManager.LoadScene(nextSceneName);
    }
}