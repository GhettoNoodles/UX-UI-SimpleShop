using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    public ItemInfo info;
    [SerializeField] private TextMeshProUGUI priceTxt;
    public int price;
    public bool upgrade;
    public Image image;
    public bool clicked = false;
    // Start is called before the first frame update
    void Start()
    {
        image.sprite = info.sprite;
        price = info.price;
        priceTxt.text = "$" + price;
        upgrade = info.upgrade;
    }

    public void itemBought()
    {
        clicked = true;
    }
}
