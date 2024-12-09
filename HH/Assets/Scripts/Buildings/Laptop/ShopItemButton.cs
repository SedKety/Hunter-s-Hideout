using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
    public ShopItem shopItem;
    [SerializeField] private Image itemSprite;
    [SerializeField] private TextMeshProUGUI itemName, itemCost;

    private bool canBeBought = true;
   public UnityEvent onBought;
    public void Start()
    {
        Init();
    }

    public void Init()
    {
        if(shopItem == null) { Destroy(gameObject); return; }
        itemSprite.sprite = shopItem.shopItemSprite;
        itemName.text = shopItem.shopItemName;
        itemCost.text = shopItem.shopItemCost.ToString();
    }
    [ContextMenu("OnBuyButtonClicked")]
    public void OnBuyButtonClicked()
    {
        print("Trying to buy");
        if (!canBeBought) { return; }
        print("cant be bought ");
        if (MoneyManager.money >= shopItem.shopItemCost)
        {
            MoneyManager.money -= shopItem.shopItemCost;
            onBought.Invoke();
            canBeBought = false;
        }
        else
        {
            print("Not enough money");
        }
    }
}
