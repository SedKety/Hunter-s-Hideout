using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandOfOuw : MonoBehaviour
{
    public int damage;
    public void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IDamagable>(out IDamagable enemy))
        {
            enemy.TakeDamage(damage);
        }
    }
}
