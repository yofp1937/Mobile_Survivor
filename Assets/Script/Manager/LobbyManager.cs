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
}
