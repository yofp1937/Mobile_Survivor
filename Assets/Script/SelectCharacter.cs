using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacter : MonoBehaviour
{
    [Header("# Character Prefab")]
    public List<GameObject> characters; // Hierarchy상에 존재하는 캐릭터 GameObject들
    public List<GameObject> prefabs; // Project상에 존재하는 캐릭터 Prefab들
    public Button startbtn;

    void Awake()
    {
        
    }

    void Select(int index)
    {
        foreach(GameObject character in characters)
        {
            character.SetActive(false);
        }
        characters[index].SetActive(true);
        GameManager.instance.Character = prefabs[index];
        startbtn.interactable = true;
    }

    public void OnClickSelectKnight()
    {
        Select(0);
    }
    public void OnClickSelectMerchant()
    {
        Select(1);
    }
    public void OnClickSelectPeasant()
    {
        Select(2);
    }
    public void OnClickSelectPriest()
    {
        Select(3);
    }
    public void OnClickSelectSoldier()
    {
        Select(4);
    }
    public void OnClickSelectThief()
    {
        Select(5);
    }
}
