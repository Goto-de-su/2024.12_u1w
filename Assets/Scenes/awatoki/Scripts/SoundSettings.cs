using UnityEngine;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    private void OnEnable()
    {
        if (SoundManager.instance != null)
        {
            // スライダーの値変更時のリスナーを削除
            // Remove listeners when slider values are changed
            bgmSlider.onValueChanged.RemoveAllListeners();
            seSlider.onValueChanged.RemoveAllListeners();

            // スライダーの初期値を設定
            // Set initial slider values
            bgmSlider.value = SoundManager.instance.GetBGMVolume();
            seSlider.value = SoundManager.instance.GetSEVolume();
            Debug.Log($"Initial BGM Slider Value: {bgmSlider.value}");
            Debug.Log($"Initial SE Slider Value: {seSlider.value}");

            // スライダーの値変更時のリスナーを追加
            // Add listeners when slider values are changed
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
            seSlider.onValueChanged.AddListener(SetSEVolume);
        }
        else
        {
            Debug.LogError("SoundManager instance is null");
        }
    }

    private void OnDisable()
    {
        SoundManager.instance.SetBGMVolume(bgmSlider.value);
        SoundManager.instance.SetSEVolume(seSlider.value);
        Debug.Log($"Initial BGM Slider Value: {bgmSlider.value}");
        Debug.Log($"Initial SE Slider Value: {seSlider.value}");
    }

    public void SetBGMVolume(float volume)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.SetBGMVolume(volume);
            Debug.Log($"BGM Volume Slider Value changed to: {volume}");
        }
        else
        {
            Debug.LogError("SoundManager instance is null");
        }
    }

    public void SetSEVolume(float volume)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.SetSEVolume(volume);
            Debug.Log($"SE Volume Slider Value changed to: {volume}");
        }
        else
        {
            Debug.LogError("SoundManager instance is null");
        }
    }
}
