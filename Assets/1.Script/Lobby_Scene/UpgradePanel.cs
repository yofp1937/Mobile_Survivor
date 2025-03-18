using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    UpgradeData data;
    int cost;

    [Header("# Data")]
    [SerializeField] List<GameObject> slots;

    [Header("# Reference")]
    [SerializeField] Text descText;
    [SerializeField] Text costText;
    [SerializeField] Button buyBtn;
    [SerializeField] Sprite levelImage;
    [SerializeField] Sprite emptyImage;

    void Awake()
    {
        buyBtn.GetComponent<Button>().interactable = false;   
    }

    void Start()
    {
        SetUpgradeSlots();
    }

    public void SetCost() // 필요한 골드 표시
    {
        // 종류는 data로 구분, level은 _level로 구분하여 작성
        int _level = GameManager.instance.StatusManager.GetUpgradeLevel(data.EnumName);

        if(_level == data.MaxLevel)
        {
            costText.text = "";
            return;
        }
        cost = data.CostList[_level];
        costText.text = cost.ToString();
    }

    void SetUpgradeSlots() // Lobby Scene 입장시 Level 이미지 변경
    {
        List<int> _list = GameManager.instance.StatusManager.UpgradeLevelDict.Values.ToList();

        for(int i = 0; i < _list.Count; i++)
        {
            for(int j = 1; j <= _list[i]; j++)
            {
                slots[i].transform.Find("Level_Panel").Find(j.ToString()).GetComponent<Image>().sprite = levelImage;
            }
        }
    }

    void ResetUpgradeSlots() // Upgrade 리셋시 Level 이미지 변경
    {
        List<int> _list = GameManager.instance.StatusManager.UpgradeLevelDict.Values.ToList();

        for(int i = 0; i < _list.Count; i++)
        {
            if(_list[i] > 0)
            {
                for(int j = 1; j <= _list[i]; j++)
                {
                    slots[i].transform.Find("Level_Panel").Find(j.ToString()).GetComponent<Image>().sprite = emptyImage;
                }
            }
        }
    }

    void LevelUpgradeSlots(UpgradeData data) // Upgrade 구매시 Level 이미지 변경
    {
        int level = GameManager.instance.StatusManager.UpgradeLevelDict[data.EnumName];

        slots[(int)data.EnumName].transform.Find("Level_Panel").Find(level.ToString()).GetComponent<Image>().sprite = levelImage;
    }

    #region "Btn"
    public void OnClickUpgradePanel()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        gameObject.SetActive(true);
    }

    public void OnClickUpgrade_Exit()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        gameObject.SetActive(false);
        buyBtn.GetComponent<Button>().interactable = false;
        cost = 0;
        descText.text = "";
        costText.text = "";
    }

    public void OnClickUpgradeSlots(int num)
    {
        AudioManager.instance.PlaySfx(Sfx.Click);

        data = GameManager.instance.StatusManager.UpgradeDataList[num];
        descText.text = data.Desc;

        buyBtn.GetComponent<Button>().interactable = true;
        SetCost();
    }
    
    public void OnClickResetBtn()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        // 강화에 사용된 골드 반환
        int resetgold = PlayerPrefs.GetInt("UseUpgradeGold", 0);
        GameManager.instance.Gold += resetgold;

        // 보유 골드 텍스트 갱신
        LobbyManager.instance.LoadHaveGold();
        
        // UseUpgradeGold 초기화
        PlayerPrefs.SetInt("UseUpgradeGold", 0);
        PlayerPrefs.Save();

        // 레벨에따라 슬롯별 체크되는거 전부 체크 해제
        ResetUpgradeSlots();

        // 데이터 리셋
        GameManager.instance.StatusManager.ResetUpgrade();

        // 설명, 필요 골드 텍스트 갱신
        descText.text = "";
        costText.text = "";
    }

    public void OnClickBuyBtn()
    {
        int level = GameManager.instance.StatusManager.UpgradeLevelDict[data.EnumName];

        if(GameManager.instance.Gold < cost) // 골드 부족하면 안눌림
        {
            return;
        }
        if(level == data.MaxLevel) // 레벨이 최고레벨이면 버튼 안눌림
        {
            return;
        }
        AudioManager.instance.PlaySfx(Sfx.Click);

        // 골드 사용
        GameManager.instance.Gold -= cost;
        level++;

        // 강화에 사용된 골드 누적
        int usedgold = PlayerPrefs.GetInt("UseUpgradeGold", 0);
        usedgold += cost;
        PlayerPrefs.SetInt("UseUpgradeGold", usedgold);
        PlayerPrefs.Save();

        // 보유 골드 텍스트 갱신
        LobbyManager.instance.LoadHaveGold();

        // 레벨업
        GameManager.instance.StatusManager.SetUpgradeLevel(data.EnumName, level);

        // 필요 골드 텍스트 갱신
        SetCost();

        // 레벨업 Image 변경
        LevelUpgradeSlots(data);
    }
    #endregion
}
