using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header(" # BGM")]
    public AudioClip[] bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;
    
    [Header(" # UI")]
    public AudioClip[] uiClip;
    public float uiVolume;
    AudioSource uiPlayer;

    [Header(" # SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Bgm
    {
        Lobby, InGame
    }

    public enum Sfx
    {
        Click, Heal, ThrowWeapon, Fireball, Explosion, Laser, Thunder, Wave, Saprk
    }

    void Awake()
    {
        instance = this;

        var obj = FindObjectsOfType<AudioManager>(); // obj로 GameManager를 전부 찾아와 배열로 변환
        if(obj.Length == 1) // obj가 1개면 씬 전환에도 값이 유지되게 설정
        {
            DontDestroyOnLoad(gameObject);
        }
        else // obj가 1개보다 많으면 현재 obj 파괴
        {
            Destroy(gameObject);
        }

        Init();
    }

    void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false; // Active True가 되자마자 실행되는거 방지
        bgmPlayer.loop = true; // bgm이 종료되면 처음부터 다시 반복
        bgmPlayer.volume = bgmVolume;
        
        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].bypassListenerEffects = true;
            sfxPlayers[i].volume = sfxVolume;
        }
    }

    public void SetBgmVolume(float volume)
    {
        bgmVolume = volume;
        bgmPlayer.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        for(int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i].volume = volume;
        }
    }

    public void InGameInit()
    {
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // AudioManager와 설정창 Slider, Btn 연결
        GameObject volset = InGameManager.instance.VolumeSettings;
        Slider bgm = volset.transform.Find("Bgm_Group").Find("Bgm_Slider").GetComponent<Slider>();
        bgm.value = bgmVolume;
        Slider sfx = volset.transform.Find("Sfx_Group").Find("Sfx_Slider").GetComponent<Slider>();
        sfx.value = sfxVolume;
        
        bgm.onValueChanged.AddListener(AudioManager.instance.SetBgmVolume);
        sfx.onValueChanged.AddListener(AudioManager.instance.SetSfxVolume);
        volset.transform.Find("Sfx_Group").Find("Sfx_TestBtn").GetComponent<Button>().onClick.AddListener(AudioManager.instance.TestSfx);
    }

    public void PlayBgm(Bgm bgm)
    {
        bgmPlayer.Stop(); // 이전 진행중이던 Bgm 정지
        bgmPlayer.clip = bgmClip[(int)bgm];
        bgmPlayer.Play(); // 원하는 Bgm 실행
    }

    public void StopBgm()
    {
        bgmPlayer.Stop();
    }

    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }

    public void PlaySfx(Sfx sfx) // 비어있는 Player를 찾아서 sfx 브금을 동작시킴
    {
        for(int i=0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            if(sfxPlayers[loopIndex].isPlaying)
                continue;

            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    public void TestSfx()
    {
        for(int i=0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            if(sfxPlayers[loopIndex].isPlaying)
                continue;

            sfxPlayers[loopIndex].clip = sfxClips[0];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
