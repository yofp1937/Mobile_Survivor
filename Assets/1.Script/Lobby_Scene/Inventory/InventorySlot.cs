using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("# Main Data")]
    public GameObject Data;

    [Header("# Reference Data")]
    [SerializeField] Image _childImage;

    public void SetItemSlot(GameObject data)
    {
        Data = data;
        _childImage.sprite = Data.GetComponent<Equipment>().Sprite;
        _childImage.gameObject.SetActive(true);
    }

    public void Reload()
    {
        Data = null;
        _childImage.sprite = null;
        _childImage.gameObject.SetActive(false);
    }
}
