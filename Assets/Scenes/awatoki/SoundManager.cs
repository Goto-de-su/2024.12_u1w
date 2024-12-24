using System.Collections.Generic;
using UnityEngine;

public enum BGMType
{
    None,
    Title,
    InGame,
    InEndRoll,
}

public enum SEType
{
    GameClear,
    GameOver,
    Shoot,
    ItemGet,
    GetDamage,
    Button,
    PlayerFootsteps,
    PlayerFall,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioSource bgmAudioSource;
    private AudioSource seAudioSource;

    [SerializeField] private float bgmVolume = 1.0f;
    [SerializeField] private float seVolume = 1.0f;

    private BGMType playingBGM = BGMType.None;

    public AudioClip bgmInTitle;
    public AudioClip bgmInGame;
    public AudioClip bgmInEndRoll;
    public AudioClip meGameClear;
    public AudioClip meGameOver;
    public AudioClip seShoot;
    public AudioClip seItemGet;
    public AudioClip seDamage;
    public AudioClip seButton;
    public AudioClip sePlayerFootsteps;
    public AudioClip sePlayerFall;

    private void Awake()
    {
        // 新しいインスタンスがすでに存在する場合、古いインスタンスを破棄
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーン遷移後も破棄されないようにする
        }
        else
        {
            Destroy(gameObject); // 新しいシーンに新たなインスタンスが作成されないようにする
            return;
        }

        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource.loop = true;

        seAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayTitleBGM()
    {
        PlayBGM(BGMType.Title);
    }

    public void PlayInGameBGM()
    {
        PlayBGM(BGMType.InGame);
    }

    public void PlayEndRollBGM()
    {
        PlayBGM(BGMType.InEndRoll);
    }

    public void PlayBGM(BGMType type)
    {
        if (playingBGM == type)
        {
            return;
        }

        AudioClip bgmClip = null;
        switch (type)
        {
            case BGMType.Title:
                bgmClip = bgmInTitle;
                break;
            case BGMType.InGame:
                bgmClip = bgmInGame;
                break;
            case BGMType.InEndRoll:
                bgmClip = bgmInEndRoll;
                break;
        }

        bgmAudioSource.clip = bgmClip;
        bgmAudioSource.volume = bgmVolume;
        bgmAudioSource.Play();

        playingBGM = type;

        Debug.Log($"Playing BGM: {type}, Volume: {bgmVolume}");
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
        playingBGM = BGMType.None;
        Debug.Log("BGM Stopped");
    }

    public void PlaySE(SEType type)
    {
        AudioClip seClip = null;
        switch (type)
        {
            case SEType.GameClear:
                seClip = meGameClear;
                break;
            case SEType.GameOver:
                seClip = meGameOver;
                break;
            case SEType.Shoot:
                seClip = seShoot;
                break;
            case SEType.ItemGet:
                seClip = seItemGet;
                break;
            case SEType.GetDamage:
                seClip = seDamage;
                break;
            case SEType.Button:
                seClip = seButton;
                break;
            case SEType.PlayerFootsteps:
                seClip = sePlayerFootsteps;
                break;
            case SEType.PlayerFall:
                seClip = sePlayerFall;
                break;
        }

        seAudioSource.clip = seClip;
        seAudioSource.volume = seVolume;
        seAudioSource.Play();

        Debug.Log($"Playing SE: {type}, Volume: {seVolume}");
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmAudioSource.volume = bgmVolume;
        Debug.Log($"SetBGMVolume called: volume = {volume}, bgmVolume = {bgmVolume}");
    }

    public void SetSEVolume(float volume)
    {
        seVolume = Mathf.Clamp01(volume);
        seAudioSource.volume = seVolume;
        Debug.Log($"SetSEVolume called: volume = {volume}, seVolume = {seVolume}");
    }

    public float GetBGMVolume()
    {
        Debug.Log($"GetBGMVolume called: bgmVolume = {bgmVolume}");
        return bgmVolume;
    }

    public float GetSEVolume()
    {
        Debug.Log($"GetSEVolume called: seVolume = {seVolume}");
        return seVolume;
    }
}
