using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DefaultNamespace;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemInfo BootsInfo;
    [SerializeField] private ItemInfo ArmorInfo;
    [SerializeField] private ItemInfo GauntletsInfo;
    [SerializeField] private ItemInfo ShieldInfo;
    [SerializeField] private ItemInfo BowInfo;
    [SerializeField] private ItemInfo SwordInfo;
    [SerializeField] private ItemInfo DaggerInfo;
    [SerializeField] private ItemInfo WandInfo;
    [SerializeField] private ItemInfo BPUpgradeInfo;
    private List<ItemInfo> itemInfos;
    private List<ItemScript> shopItems;
    private List<ItemScript> backPackItems;
    private List<ItemScript> ChestItems;
    [SerializeField] private GameObject ShopPanel;
    [SerializeField] private GameObject BPPanel;
    [SerializeField] private GameObject ChestPanel;
    [SerializeField] private TextMeshProUGUI capShop;
    [SerializeField] private TextMeshProUGUI capChest;
    [SerializeField] private TextMeshProUGUI capBP;
    [SerializeField] private TextMeshProUGUI balanceTxt;
    [SerializeField] private TextMeshProUGUI backpackPrompt;
    [SerializeField] private ItemScript ItemPrefab;
    private int balance = 100;
    private Vector2 itemPos;
    private int backpackMax = 4;
    private float spawnTimer = 2f;
    private float spawnTimerMax = 2f;
    private enum screenState
    {
        Chest,
        Shop
    }

    private screenState activePanel = screenState.Shop;

    // Start is called before the first frame update
    void Start()
    {
        itemInfos = new List<ItemInfo>
        {
            BootsInfo,
            ArmorInfo,
            GauntletsInfo,
            ShieldInfo,
            BowInfo,
            SwordInfo,
            DaggerInfo,
            WandInfo,
            BPUpgradeInfo
        };
        shopItems = new List<ItemScript>();
        backPackItems = new List<ItemScript>();
        ChestItems = new List<ItemScript>();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimerMax = shopItems.Count < 4 ? 2f : 5f;
        spawnTimer += Time.deltaTime;
        if (spawnTimer>=spawnTimerMax && shopItems.Count < 8)
        {
            int rando = Random.Range(0, (7 - shopItems.Count));
            itemPos = DetermineShopCoords(shopItems.Count);
            var currentItem = Instantiate(ItemPrefab, itemPos, quaternion.identity);
            currentItem.transform.SetParent(ShopPanel.transform, false);
            currentItem.info = itemInfos[rando];
            shopItems.Add(currentItem);
            itemInfos.RemoveAt(rando);
            capShop.text = "Capacity:\n" + shopItems.Count + "/8";
            spawnTimer = 0f;
        }
        else if (shopItems.Count >= 8)
        {
            spawnTimer = 0f;
        }

        for (var i = 0; i < shopItems.Count; i++)
        {
            if (shopItems[i].clicked)
            {
                shopItems[i].clicked = false;
                if (shopItems[i].upgrade)
                {
                    if (shopItems[i].price <= balance)
                    {
                        backpackMax = 8;
                        balance -= shopItems[i].price;
                        balanceTxt.text = "Balance:\n" + "$" + balance;
                        capBP.text = "Capacity:\n" + backPackItems.Count + "/" + backpackMax;
                        Destroy(shopItems[i].gameObject);
                        shopItems.RemoveAt(i);
                        for (var k = 0; k < shopItems.Count; k++)
                        {
                            shopItems[k].transform.localPosition = DetermineShopCoords(k);
                        }

                        break;
                    }
                }
                else
                {
                    if (backPackItems.Count < backpackMax)
                    {
                        
                        if (shopItems[i].price <= balance)
                        {
                            ItemBought(i);
                            break;
                        }
                        else
                        {
                            balanceTxt.color = Color.red;
                        }
                    }
                }
            }
        }

        for (var j = 0; j < backPackItems.Count; j++)
        {
            if (backPackItems[j].clicked)
            {
                backPackItems[j].clicked = false;
                if (activePanel == screenState.Shop)
                {
                   
                    ItemSold(j);
                    balanceTxt.color = Color.black;
                    break;
                }

                if (activePanel == screenState.Chest)
                {
                    MoveToChest(j);
                    break;
                }
            }
        }

        for (var c = 0; c < ChestItems.Count; c++)
        {
            if (ChestItems[c].clicked)
            {
                ChestItems[c].clicked = false;
                if (backPackItems.Count < backpackMax)
                {
                    MoveToBackpack(c);
                    break;
                }
            }
        }
    }

    private Vector2 DetermineShopCoords(int itemOrder)
    {
        var returnPos = new Vector2(0, 0);
        switch (itemOrder)
        {
            case 0:
                returnPos = new Vector2(-300, 175);
                break;
            case 1:
                returnPos = new Vector2(-100, 175);
                break;
            case 2:
                returnPos = new Vector2(100, 175);
                break;
            case 3:
                returnPos = new Vector2(300, 175);
                break;
            case 4:
                returnPos = new Vector2(-300, -125);
                break;
            case 5:
                returnPos = new Vector2(-100, -125);
                break;
            case 6:
                returnPos = new Vector2(100, -125);
                break;
            case 7:
                returnPos = new Vector2(300, -125);
                break;
        }

        return returnPos;
    }

    private void ItemBought(int i)
    {
        shopItems[i].transform.SetParent(BPPanel.transform, false);
        capShop.text = "Capacity:\n" + shopItems.Count + "/8";
        balance -= shopItems[i].price;
        balanceTxt.text = "Balance:" + "\n" + "$" + balance;
        backPackItems.Add(shopItems[i]);
        capBP.text = "Capacity:\n" + backPackItems.Count + "/" + backpackMax;
        itemInfos.Add(shopItems[i].info);
        shopItems.RemoveAt(i);
        for (var k = 0; k < shopItems.Count; k++)
        {
            shopItems[k].transform.localPosition = DetermineShopCoords(k);
        }

        for (var k = 0; k < backPackItems.Count; k++)
        {
            backPackItems[k].transform.localPosition = DetermineShopCoords(k);
        }
    }

    private void ItemSold(int i)
    {
        balance += backPackItems[i].price;
        balanceTxt.text = "Balance:" + "\n" + "$" + balance;

        if (shopItems.Count < 8)
        {
            shopItems.Add(backPackItems[i]);
            backPackItems[i].transform.SetParent(ShopPanel.transform, false);
            capShop.text = "Capacity:\n" + shopItems.Count + "/8";
        }
        else
        {
            Destroy(backPackItems[i].gameObject);
        }

        // itemInfos.Add(shopItems[i].info);
        backPackItems.RemoveAt(i);
        capBP.text = "Capacity:\n" + backPackItems.Count + "/" + backpackMax;

        for (var k = 0; k < shopItems.Count; k++)
        {
            shopItems[k].transform.localPosition = DetermineShopCoords(k);
        }

        for (var k = 0; k < backPackItems.Count; k++)
        {
            backPackItems[k].transform.localPosition = DetermineShopCoords(k);
        }
    }

    public void SwitchToShop()
    {
        ChestPanel.SetActive(false);
        ShopPanel.SetActive(true);
        backpackPrompt.text = "Click items to sell";
        activePanel = screenState.Shop;
    }

    public void SwitchToChest()
    {
        ChestPanel.SetActive(true);
        ShopPanel.SetActive(false);
        backpackPrompt.text = "Click items to move";
        activePanel = screenState.Chest;
    }

    private void MoveToChest(int i)
    {
        if (ChestItems.Count < 8)
        {
            ChestItems.Add(backPackItems[i]);
            backPackItems[i].transform.SetParent(ChestPanel.transform, false);
            capChest.text = "Capacity:\n" + ChestItems.Count + "/8";
            backPackItems.RemoveAt(i);
            capBP.text = "Capacity:\n" + backPackItems.Count + "/" + backpackMax;


            // itemInfos.Add(shopItems[i].info);


            for (var k = 0; k < ChestItems.Count; k++)
            {
                ChestItems[k].transform.localPosition = DetermineShopCoords(k);
            }

            for (var k = 0; k < backPackItems.Count; k++)
            {
                backPackItems[k].transform.localPosition = DetermineShopCoords(k);
            }
        }
    }

    private void MoveToBackpack(int i)
    {
        if (backPackItems.Count < backpackMax)
        {
            backPackItems.Add(ChestItems[i]);
            ChestItems[i].transform.SetParent(BPPanel.transform, false);
            capBP.text = "Capacity:\n" + backPackItems.Count + "/" + backpackMax;
            ChestItems.RemoveAt(i);
            capChest.text = "Capacity:\n" + ChestItems.Count + "/8";

            for (var k = 0; k < ChestItems.Count; k++)
            {
                ChestItems[k].transform.localPosition = DetermineShopCoords(k);
            }

            for (var k = 0; k < backPackItems.Count; k++)
            {
                backPackItems[k].transform.localPosition = DetermineShopCoords(k);
            }
        }
    }
}