using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour, IOnHoverImpulsable
{
    public ShopItem shopItem;
    [SerializeField] private TextMeshProUGUI itemName, itemCost;

    private bool canBeBought = true;
   public UnityEvent onBought;

    public GameObject objectToBuy;
    public void Start()
    {
        Init();
    }

    public void Init()
    {
        if(shopItem == null) { Destroy(gameObject); return; }
        itemName.text = shopItem.shopItemName;
        itemCost.text = shopItem.shopItemCost.ToString();
    }
    [ContextMenu("OnClicked")]
    public void OnClicked()
    {
        print("Trying to buy");
        if (!canBeBought || onBought == null ) { return; }
        print("cant be bought ");
        if (MoneyManager.money >= shopItem.shopItemCost)
        {
            GameManager.Instance.moneyManager.AddMoney(-shopItem.shopItemCost);
            onBought.Invoke();
        }
        else
        {
            print("Not enough money");
        }
    }

    public void SendGetPackageRequest()
    {
        DroneManager.Instance.StartPackageCoroutine(objectToBuy);
    }
}
