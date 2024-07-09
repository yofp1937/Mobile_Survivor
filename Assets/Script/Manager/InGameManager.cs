using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    
    [Header("# GameObject")]
    public Player player;
    public GameObject LevelUpPanel;

    void Awake()
    {
        GameManager.instance.TimerStart();
        LevelUpPanel.SetActive(false);
        CreatePlayerCharacter();
    }

    public void CreatePlayerCharacter()
    {
        GameObject characterinstance = Instantiate(GameManager.instance.Character, player.gameObject.transform.position, Quaternion.identity);
        Debug.Log(player.gameObject.transform.position);
        characterinstance.transform.SetParent(player.gameObject.transform);
    }
}
