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
    EnemyMove,
    EnemySleep,
}

public class SoundManager : MonoBehaviour
{
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
    public AudioClip seEnemyMove;
    public AudioClip seEnemySleep;

    private AudioSource bgmAudioSource;
    private List<AudioSource> seAudioSources = new List<AudioSource>();
    private int sePoolSize = 3; // プールの最大数

    [SerializeField] private float bgmVolume = 1.0f;
    [SerializeField] private float seVolume = 1.0f;
    public static SoundManager instance;
    private BGMType playingBGM = BGMType.None;

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

        // SE用のAudioSourceをプールのサイズに合わせて生成
        for (int i = 0; i < sePoolSize; i++)
        {
            AudioSource seSource = gameObject.AddComponent<AudioSource>();
            seAudioSources.Add(seSource);
        }
    }

    private void Start()
    {
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
            case SEType.EnemyMove:
                seClip = seEnemyMove;
                break;
            case SEType.EnemySleep:
                seClip = seEnemySleep;
                break;
        }

        if (seClip == null)
        {
            Debug.LogWarning($"SE Clip is null for SEType: {type}");
            return;
        }

        // 空いているAudioSourceを見つける
        AudioSource availableSource = GetAvailableAudioSource();

        if (availableSource != null)
        {
            availableSource.PlayOneShot(seClip, seVolume);
            Debug.Log($"Playing SE: {type}, Clip Name: {seClip.name}, Volume: {seVolume}");
        }
        else
        {
            // すべてのSE AudioSourceが再生中なら最も古いものを停止して再生
            AudioSource oldestSource = seAudioSources[0];
            for (int i = 1; i < seAudioSources.Count; i++)
            {
                if (!seAudioSources[i].isPlaying)
                {
                    oldestSource = seAudioSources[i];
                    break;
                }
                else if (seAudioSources[i].time < oldestSource.time)
                {
                    oldestSource = seAudioSources[i];
                }
            }

            // 最も古いAudioSourceを停止して新しいSEを再生
            oldestSource.Stop();
            oldestSource.PlayOneShot(seClip, seVolume);
            Debug.Log($"Stopped oldest SE and playing new SE: {seClip.name}, Volume: {seVolume}");
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in seAudioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        return null;
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
        foreach (var seSource in seAudioSources)
        {
            seSource.volume = seVolume;
        }
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