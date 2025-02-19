using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHead : Holdable, ISellable
{
    [Header("Head Variables")]
    public float headWeight;
    public float baseMoneyValue;

    [Tooltip("This value will either increase or decrease the worth of weight on an animal head")]
    public float valueBasedOnWeightMultiplier = 1;

    public override void Pickup(InteractionController hand)
    {
        base.Pickup(hand);
    }
    public void OnSell()
    {
        var moneyValue = baseMoneyValue + (valueBasedOnWeightMultiplier * headWeight);
        GameManager.Instance.moneyManager.AddMoney(moneyValue);
        Destroy(gameObject);
    }
    public override void OnItemUse()
    {
        print("Attempted To Use an Non-Usable item of type:  " + gameObject.name);
    }

    public int ReturnValue()
    {
        return (int)(baseMoneyValue + (valueBasedOnWeightMultiplier * headWeight));
    }
}
