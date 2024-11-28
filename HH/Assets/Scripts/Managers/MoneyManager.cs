using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static float money = 0;
    public static float moneyMultiplier = 1;

    public static void AddMoney(float moneyToAdd)
    {
        money += moneyToAdd * moneyMultiplier;
    }
}
