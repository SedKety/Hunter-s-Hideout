using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public List<ShopItem> shopItems = new List<ShopItem>();
    public GameObject shopItemTemplate;

    public Transform shopItemParent;
    public void Start()
    {

    }
    public void Init()
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            Instantiate(shopItemTemplate, transform.position, transform.rotation, shopItemParent)
                .GetComponent<ShopItemButton>().shopItem = shopItems[i];
        }
    }
}
