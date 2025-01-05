using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // 遷移先のシーン名
    [SerializeField]
    private string nextSceneName;

    void Update()
    {
        // マウスの左クリックを検出
        if (Input.GetMouseButtonDown(0))
        {
            LoadNextScene();
        }
    }

    // シーン遷移を行うメソッド
    public void LoadNextScene()
    {
        // 指定したシーンに遷移
        SceneManager.LoadScene(nextSceneName);
    }
}