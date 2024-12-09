using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObject/ShopItem")]
public class ShopItem : ScriptableObject
{
    public Sprite shopItemSprite;
    public string shopItemName;
    public int shopItemCost;
}
