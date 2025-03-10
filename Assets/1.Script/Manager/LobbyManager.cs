using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    #region "Singleton"
    public static LobbyManager instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    
    public AdMobManager admobManager;
    public MainMenu mainMenu;

    private void Init() // Awake에서 실행
    {
        
    }

    void Update()
    {
        // Test Code - 숫자패드 + 누르면 골드 100원 증가
        if(Input.GetKeyDown(KeyCode.KeypadPlus) && GameManager.instance.IsDeveloperMode)
        {
            GameManager.instance.Gold += 100;
            mainMenu.LoadHaveGold();
        }

        // Test Code - 숫자패드 - 누르면 골드 초기화
        if(Input.GetKeyDown(KeyCode.KeypadMinus) && GameManager.instance.IsDeveloperMode)
        {
            GameManager.instance.Gold = 0;
            mainMenu.LoadHaveGold();
        }

        // Test Code - Space 누르면 GameManager의 UpgradeLevelDict 내용물 출력
        if(Input.GetKeyDown(KeyCode.Space) && GameManager.instance.IsDeveloperMode)
        {
            var test = GameManager.instance.Status.UpgradeLevelDict;

            foreach(var _ in test)
            {
                Debug.Log("Key: " + _.Key  + ", Value: " + _.Value);
            }
        }
    }
}
