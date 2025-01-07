using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObject/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string shopItemName;
    public int shopItemCost;
}
