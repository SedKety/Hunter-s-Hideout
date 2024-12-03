using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingPoint : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<ISellable>(out var sellable))
        {
            sellable.OnSell();
        }
    }
}
