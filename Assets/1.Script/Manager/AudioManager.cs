using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    #region "Singleton"
    public static AudioManager instance;
    void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {
            instance = this;
            Init();
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 값이 유지되도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 새로운 객체는 파괴
        }
    }
    #endregion

    [Header(" # BGM")]
    [SerializeField] AudioClip[] _bgmClip;
    public float BgmVolume;
    AudioSource _bgmPlayer;
    AudioHighPassFilter _bgmEffect;
    
    [Header(" # SFX")]
    [SerializeField] AudioClip[] _sfxClips;
    public float SfxVolume;
    public int Channels;
    AudioSource[] _sfxPlayers;
    int _channelIndex;

    void Init() // Awake에서 실행
    {
        BgmVolume = PlayerPrefs.GetFloat("Bgm", 1);
        SfxVolume = PlayerPrefs.GetFloat("Sfx", 1);

        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        _bgmPlayer = bgmObject.AddComponent<AudioSource>();
        _bgmPlayer.playOnAwake = false; // Active True가 되자마자 실행되는거 방지
        _bgmPlayer.loop = true; // bgm이 종료되면 처음부터 다시 반복
        _bgmPlayer.volume = BgmVolume;
        
        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        _sfxPlayers = new AudioSource[Channels];

        for(int i = 0; i < _sfxPlayers.Length; i++)
        {
            _sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            _sfxPlayers[i].playOnAwake = false;
            _sfxPlayers[i].bypassListenerEffects = true;
            _sfxPlayers[i].volume = SfxVolume;
        }

        PlayBgm(Bgm.Lobby);
    }

    public void SetBgmVolume(float volume)
    {
        PlayerPrefs.SetFloat("Bgm", volume);
        PlayerPrefs.Save();
        BgmVolume = volume;
        _bgmPlayer.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat("Sfx", volume);
        PlayerPrefs.Save();
        SfxVolume = volume;
        for(int i = 0; i < _sfxPlayers.Length; i++)
        {
            _sfxPlayers[i].volume = volume;
        }
    }

    public void PlayBgm(Bgm bgm)
    {
        _bgmPlayer.Stop();
        _bgmPlayer.clip = _bgmClip[(int)bgm];
        _bgmPlayer.Play();
    }

    public void PlaySfx(Sfx sfx)
    {
        for(int i=0; i < _sfxPlayers.Length; i++)
        {
            int loopIndex = (i + _channelIndex) % _sfxPlayers.Length;

            if(_sfxPlayers[loopIndex].isPlaying)
                continue;

            _sfxPlayers[loopIndex].clip = _sfxClips[(int)sfx];
            _sfxPlayers[loopIndex].Play();
            break;
        }
    }

    public void EffectBgm(bool isPlay) // 레벨업시 Bgm에 마스크씌움
    {
        _bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();
        _bgmEffect.enabled = isPlay;
    }

    #region "Btn"
    public void OnClickTestSfx()
    {
        for(int i=0; i < _sfxPlayers.Length; i++)
        {
            int loopIndex = (i + _channelIndex) % _sfxPlayers.Length;

            if(_sfxPlayers[loopIndex].isPlaying)
                continue;

            _sfxPlayers[loopIndex].clip = _sfxClips[0];
            _sfxPlayers[loopIndex].Play();
            break;
        }
    }
    #endregion
}
