using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    // ボタンオブジェクト
    public Button startButton;       // ゲーム開始ボタン
    public Button optionsButton;     // オプション画面ボタン
    public Button creditsButton;     // クレジット画面ボタン
    public Button exitButton;        // ゲーム終了ボタン
    public Button titleBackButton;   // タイトルバックボタン

    // インスペクタで設定する初期シーン名
    [Header("シーン設定")]
    public string titleSceneName = "Title"; // タイトルシーン名
    public string gameSceneName = "SampleScene";  // ゲームシーン名
    public string optionSceneName = "Option";
    public string creditsSceneName = "Credits";

    void Start()
    {
        // 各ボタンのリスナーを設定
        if (startButton != null)
            startButton.onClick.AddListener(() => LoadScene(gameSceneName));

        if (optionsButton != null)
            optionsButton.onClick.AddListener(() => LoadScene(optionSceneName));

        if (creditsButton != null)
            creditsButton.onClick.AddListener(() => LoadScene(creditsSceneName));

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);  // EXITボタンのリスナーを追加

        if (titleBackButton != null)
            titleBackButton.onClick.AddListener(() => LoadScene(titleSceneName)); // タイトルバックボタンの処理

        // シーンがロードされた後に呼ばれるイベントを登録
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ゲーム開始時にタイトルBGMを再生
        SoundManager.instance.PlayTitleBGM();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // タイトルシーンがロードされたときにBGMを再生
        if (scene.name == titleSceneName)
        {
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayTitleBGM();
            }
        }
        if (scene.name == gameSceneName)
        {
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayInGameBGM();
            }
        }
    }

    /// <summary>
    /// 指定したシーンをロードする関数
    /// </summary>
    /// <param name="sceneName">ロードするシーンの名前</param>
    private void LoadScene(string sceneName)
    {
        // 音量設定を保存したい場合に使う
        SaveVolumes();

        // シーンをロード
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 音量設定を保存する（必要に応じて拡張）
    /// </summary>
    private void SaveVolumes()
    {
        if (SoundManager.instance != null)
        {
            float bgmVolume = SoundManager.instance.GetBGMVolume();
            Debug.Log($"Saved BGM Volume: {bgmVolume}");
        }
    }

    /// <summary>
    /// ゲームを終了する処理
    /// </summary>
    private void ExitGame()
    {
        // アプリケーションを終了
        Debug.Log("Exiting game...");
        Application.Quit();

        // エディタで実行中の場合は、ゲームを停止する
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
