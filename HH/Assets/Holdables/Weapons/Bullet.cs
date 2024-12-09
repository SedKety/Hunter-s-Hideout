using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public int damage;
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamagable>(out IDamagable hitEntity))
        {
            hitEntity.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
