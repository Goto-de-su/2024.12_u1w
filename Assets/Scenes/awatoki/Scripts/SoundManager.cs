using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGMの種類を列挙する列挙型。
/// </summary>
public enum BGMType
{
    None,
    Title,
    InGame,
    InEndRoll,
}

/// <summary>
/// SEの種類を列挙する列挙型。
/// </summary>
public enum SEType
{
    GameClear,
    GameOver,
    PlayerJump,
    PlayerFall,
    PlayerWalk,
    PlayerRun,
    EnemyMove,
    EnemySleep,
    HedgehogSleep,
    FlowerOpen,
    FlowerClose,
    Trap,
    Light,
}

/// <summary>
/// サウンドを管理するクラス。BGMとSEの再生・停止、ボリュームの設定などを行う。
/// </summary>
public class SoundManager : MonoBehaviour
{
    // --- 各種AudioClipの定義 ---
    public AudioClip bgmInTitle;
    public AudioClip bgmInGame;
    public AudioClip bgmInEndRoll;
    public AudioClip meGameClear;
    public AudioClip meGameOver;
    public AudioClip sePlayerJump;
    public AudioClip sePlayerFall;
    public AudioClip sePlayerWalk;
    public AudioClip sePlayerRun;
    public AudioClip seEnemyMove;
    public AudioClip seEnemySleep;
    public AudioClip seHedgehogSleep;
    public AudioClip seFlowerOpen;
    public AudioClip seFlowerClose;
    public AudioClip seTrap;
    public AudioClip seLight;

    // --- 内部変数 ---
    private AudioSource bgmAudioSource; // BGM再生用AudioSource
    private List<AudioSource> seAudioSources = new List<AudioSource>(); // SE再生用AudioSourceのプール
    private Dictionary<AudioSource, GameObject> seSourceOwners = new Dictionary<AudioSource, GameObject>(); // AudioSourceとオーナー(GameObject)の関連付け
    private int sePoolSize = 16; // SEのプールサイズ

    [SerializeField] private float bgmVolume = 1.0f; // BGMの音量
    [SerializeField] private float seVolume = 1.0f; // SEの音量
    public static SoundManager instance; // シングルトンインスタンス
    private BGMType playingBGM = BGMType.None; // 現在再生中のBGMの種類

    /// <summary>
    /// シングルトンの初期化とAudioSourceの設定を行う。
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいで破棄されないようにする
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // BGM用AudioSourceの初期化
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource.loop = true;

        // SE用AudioSourceのプールを初期化
        for (int i = 0; i < sePoolSize; i++)
        {
            AudioSource seSource = gameObject.AddComponent<AudioSource>();
            seAudioSources.Add(seSource);
            seSourceOwners[seSource] = null; // 初期化時はオーナーなし
        }
    }

    // --- BGM再生関連のメソッド ---
    public void PlayTitleBGM() => PlayBGM(BGMType.Title);
    public void PlayInGameBGM() => PlayBGM(BGMType.InGame);
    public void PlayEndRollBGM() => PlayBGM(BGMType.InEndRoll);

    /// <summary>
    /// 指定したBGMを再生する。
    /// </summary>
    public void PlayBGM(BGMType type)
    {
        if (playingBGM == type) return; // 同じBGMが既に再生中の場合はスキップ

        AudioClip bgmClip = type switch
        {
            BGMType.Title => bgmInTitle,
            BGMType.InGame => bgmInGame,
            BGMType.InEndRoll => bgmInEndRoll,
            _ => null,
        };

        bgmAudioSource.clip = bgmClip;
        bgmAudioSource.volume = bgmVolume;
        bgmAudioSource.Play();

        playingBGM = type;
    }

    /// <summary>
    /// 現在再生中のBGMを停止する。
    /// </summary>
    public void StopBGM()
    {
        bgmAudioSource.Stop();
        playingBGM = BGMType.None;
    }

    // --- SE再生関連のメソッド ---
    public void PlaySE(SEType type, GameObject owner = null)
    {
        AudioClip seClip = type switch
        {
            SEType.GameClear => meGameClear,
            SEType.GameOver => meGameOver,
            SEType.PlayerJump => sePlayerJump,
            SEType.PlayerFall => sePlayerFall,
            SEType.FlowerOpen => seFlowerOpen,
            SEType.FlowerClose => seFlowerClose,
            SEType.Trap => seTrap,
            _ => null,
        };

        if (seClip == null)
        {
            Debug.LogWarning($"SE Clip is null for SEType: {type}");
            return;
        }

        AudioSource availableSource = GetAvailableAudioSource();

        if (availableSource != null)
        {
            availableSource.PlayOneShot(seClip, seVolume);
            seSourceOwners[availableSource] = owner;
        }
        else
        {
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

            oldestSource.Stop();
            oldestSource.PlayOneShot(seClip, seVolume);
            seSourceOwners[oldestSource] = owner;
        }
    }

    public void PlaySELoop(SEType type, GameObject owner = null)
    {
        AudioClip seClip = type switch
        {
            SEType.PlayerWalk => sePlayerWalk,
            SEType.PlayerRun => sePlayerRun,
            SEType.EnemyMove => seEnemyMove,
            SEType.EnemySleep => seEnemySleep,
            SEType.HedgehogSleep => seHedgehogSleep,
            SEType.Light => seLight,
            _ => null,
        };

        if (seClip == null)
        {
            Debug.LogWarning($"SE Clip is null for SEType: {type}");
            return;
        }

        AudioSource availableSource = GetAvailableAudioSource();

        if (availableSource != null)
        {
            availableSource.loop = true; // ループ再生を有効化
            availableSource.clip = seClip; // クリップを設定
            availableSource.Play(); // 再生開始
            seSourceOwners[availableSource] = owner;
        }
        else
        {
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

            oldestSource.Stop();
            oldestSource.loop = true; // ループ再生を有効化
            oldestSource.clip = seClip; // クリップを設定
            oldestSource.Play(); // 再生開始
            seSourceOwners[oldestSource] = owner;
        }
    }

    /// <summary>
    /// 指定したオーナーに関連付けられたSEを停止する。
    /// </summary>
    public void StopSE(GameObject owner)
    {
        foreach (var pair in seSourceOwners)
        {
            if (pair.Value == owner && pair.Key.isPlaying)
            {
                pair.Key.Stop();
            }
        }
    }

	public void StopSELoop(GameObject owner)
	{
		foreach (var pair in seSourceOwners)
		{
			if (pair.Value == owner)
			{
				// AudioSourceがnullか破棄されている場合、処理をスキップ
				if (pair.Key == null)
				{
					continue;
				}
				
				// ループしているSEを対象
				if (pair.Key.loop)
				{
					// AudioSourceがまだ再生中か確認
					if (pair.Key.isPlaying)
					{
						pair.Key.Stop(); // ループしているSEを停止
					}
				}
			}
		}
	}

	public void OnDestroy()
	{
		// SEを停止したい場合、破棄されたAudioSourceにアクセスしないようにする
		foreach (var pair in seSourceOwners)
		{
			if (pair.Key == null) // AudioSourceがnullの場合、処理をスキップ
			{
				continue;
			}
			
			// AudioSourceが再生中か確認
			if (pair.Key.isPlaying)
			{
				pair.Key.Stop();
			}
		}
	}

    // --- ユーティリティメソッド ---
    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in seAudioSources)
        {
            if (!source.isPlaying) return source;
        }
        return null;
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmAudioSource.volume = bgmVolume;
    }

    public void SetSEVolume(float volume)
    {
        seVolume = Mathf.Clamp01(volume);
        foreach (var seSource in seAudioSources)
        {
            seSource.volume = seVolume;
        }
    }

    public float GetBGMVolume() => bgmVolume;
    public float GetSEVolume() => seVolume;
}
