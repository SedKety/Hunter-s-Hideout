using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public MoneyManager moneyManager;
    public Transform player;

    public void Awake()
    {
        if (Instance == null) { Instance = this; }
        else
        {
            Destroy(this);
        }
    }
}
