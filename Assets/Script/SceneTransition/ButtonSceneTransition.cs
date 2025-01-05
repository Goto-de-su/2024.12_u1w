using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSceneTransition : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName;  // インスペクターで設定する遷移先シーン名

    [SerializeField]
    private Button transitionButton;  // ボタンへの参照

    void Start()
    {
        // ボタンコンポーネントの取得（アタッチされていない場合）
        if (transitionButton == null)
        {
            transitionButton = GetComponent<Button>();
        }

        // ボタンにクリックイベントを追加
        transitionButton.onClick.AddListener(LoadNextScene);
    }

    // シーン遷移を行うメソッド
    public void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("遷移先のシーン名が設定されていません");
            return;
        }

        // シーン遷移
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        // クリーンアップ
        if (transitionButton != null)
        {
            transitionButton.onClick.RemoveListener(LoadNextScene);
        }
    }
}