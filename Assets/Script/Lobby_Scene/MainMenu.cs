using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor.SearchService;
using Unity.VisualScripting;

public class MainMenu : MonoBehaviour
{
    GameObject StartGame;

    void Awake()
    {
        StartGame = transform.Find("StartGame_Panel").gameObject; // MainMenu의 자식 객체중 StartGame이라는 이름의 객체를 가져옴
        StartGame.SetActive(false);
    }

    public void OnClickStartGame()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        StartGame.SetActive(true);
    }

    public void OnClickUpgrades()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Debug.Log("강화");
    }

    public void OnClickSettings()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Debug.Log("설정");
    }
    public void OnClickExit()
    {
        // #if라는 전처리기 지시문 사용
        // UnityEditor에서 실행중이면 실행을 취소하고
        // 다른 환경에서 실행중이면 어플리케이션을 끝낸다
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnClickStartGame_Start()
    {
        if(GameManager.instance.SelectCharacter == null)
        {
            return;
        } else {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
            SceneManager.LoadScene("InGame");
        }
    }

    public void OnClickStartGame_Exit()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        StartGame.SetActive(false);
    }
}
