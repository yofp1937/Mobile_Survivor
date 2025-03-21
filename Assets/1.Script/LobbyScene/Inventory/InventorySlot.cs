using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("# Main Data")]
    public GameObject Data;

    [Header("# Reference Data")]
    [SerializeField] Image _weaponImage;
    [SerializeField] Text _levelText;

    public void SetItemSlot(GameObject data)
    {
        Data = data;
        _weaponImage.sprite = Data.GetComponent<Equipment>().Sprite;
        _weaponImage.gameObject.SetActive(true);
        SetLevelText();
    }

    public void ResetData()
    {
        Data = null;
        _weaponImage.sprite = null;
        _levelText.text = "";
        _weaponImage.gameObject.SetActive(false);
    }

    public void SetLevelText()
    {
        Equipment data = Data.GetComponent<Equipment>();
        if(data.EquipLevel == 0)
        {
            _levelText.text = "";
        }
        else
        {
            _levelText.text = data.EquipLevel.ToString();
        }
    }
}
