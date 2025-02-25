using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI instance;

    [SerializeField] private GameObject retryButton; // リトライボタンのオブジェクト
    private Button retryButtonComponent;

    private void Awake()
    {
        // シングルトン化
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 最初はボタンを非表示
        retryButton.SetActive(false);
        retryButtonComponent = retryButton.GetComponent<Button>();
    }

    public void ShowRetryButton()
    {
        retryButton.SetActive(true);

        // オプション：ボタンを自動的にクリックしたい場合は、以下を追加
        // retryButtonComponent.onClick.Invoke();
    }

    public void RetryGame()
    {
        SceneManager.LoadScene("GameScene"); // GameScene を再ロード
    }
}
