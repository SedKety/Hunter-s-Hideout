using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SellingPoint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyDisplay;
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<ISellable>(out var sellable))
        {
            moneyDisplay.text = sellable.ReturnValue().ToString();
            sellable.OnSell();
        }
    }
}
