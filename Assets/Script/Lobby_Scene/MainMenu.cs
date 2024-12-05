using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor.SearchService;
using Unity.VisualScripting;
using System;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("# Panel")]
    public GameObject StartGame;
    public GameObject Settings;
    public GameObject Score;

    [Header("# StartGame Panel")]
    public List<GameObject> SG_character_Slots;

    [Header("# Score Panel")]
    public GameObject Sc_character_Image;
    public GameObject Sc_kill;
    Text killtext;
    public GameObject Sc_gold;
    Text goldtext;
    public GameObject Sc_potion;
    Text potiontext;
    public GameObject Sc_magnet;
    Text magnettext;
    public List<GameObject> Sc_Weapons;

    void Awake()
    {
        StartGame.SetActive(false);
        Settings.SetActive(false);
        Score.SetActive(false);

        killtext = Sc_kill.GetComponent<Text>();
        goldtext = Sc_gold.GetComponent<Text>();
        potiontext = Sc_potion.GetComponent<Text>();
        magnettext = Sc_magnet.GetComponent<Text>();
    }

    void Start()
    {
        if(GameManager.instance.boolScore)
        {
            ActiveScore();
        }
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
        Settings.SetActive(true);
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

    public void OnClickSettings_Exit()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Settings.SetActive(false);
    }

    void ActiveScore()
    {
        // 플레이한 캐릭터 표시
        String charcname = GameManager.instance.SelectCharacter.name;
        GameObject charc = null;
        foreach(GameObject slot in SG_character_Slots)
        {
            Transform _ = slot.transform.Find(charcname);
            if(_ != null && _.gameObject.name == charcname)
            {
                charc = _.gameObject;
                break;
            }
        }
        Sc_character_Image.GetComponent<Image>().sprite = charc.GetComponent<Image>().sprite;

        // 킬수, 획득 골드 표시
        killtext.text = string.Format("{0:F0}", GameManager.instance.kill);
        goldtext.text = string.Format("{0:F0}", GameManager.instance.getGold);
        potiontext.text = string.Format("{0:F0}", GameManager.instance.getPotion);
        magnettext.text = string.Format("{0:F0}", GameManager.instance.getMagnet);

        // 무기 이미지, 무기별 데미지 표시
        int index = 0;
        foreach(var dict in GameManager.instance.accumWeaponDamageDict)
        {
            AccumWeaponData data = dict.Value;
            // 이미지 설정
            Image wimage = Sc_Weapons[index].transform.Find("Image").GetComponent<Image>();
            wimage.sprite = data.Weapon.itemIcon;
            wimage.gameObject.SetActive(true);

            // 레벨 텍스트 설정
            Text text = Sc_Weapons[index].transform.Find("LvText").GetComponent<Text>();
            text.text = "Lv." + data.Level;
            text.gameObject.SetActive(true);

            // 데미지 텍스트 설정
            text = Sc_Weapons[index].transform.Find("DamageText").GetComponent<Text>();
            text.text = FormatDamage(data.TotalDamage);
            text.gameObject.SetActive(true);

            index++;
        }

        GameManager.instance.boolScore = false;
        Score.SetActive(true);
    }

    string FormatDamage(float damage)
    {
        if (damage >= 1_000_000_000_000) // 1T 이상
            return $"{damage / 1_000_000_000_000:0.#}T";
        else if (damage >= 1_000_000_000) // 1B 이상
            return $"{damage / 1_000_000_000:0.#}B";
        else if (damage >= 1_000_000) // 1M 이상
            return $"{damage / 1_000_000:0.#}M";
        else if (damage >= 1_000) // 1K 이상
            return $"{damage / 1_000:0.#}K";
        else // 1K 미만
            return $"{damage:0}";
    }

    public void HideScore()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Score.SetActive(false);
        GameManager.instance.GameDataReset();
    }
}
