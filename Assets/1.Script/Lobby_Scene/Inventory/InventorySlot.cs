using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("# Main Data")]
    Equipment _equip;

    [Header("# Reference Data")]
    [SerializeField] Image _childImage;

    public void SetItemSlot(Equipment data)
    {
        _equip = data;
        _childImage.sprite = _equip.Data.Sprite;
    }
}
