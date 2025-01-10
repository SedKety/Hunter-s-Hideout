using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/GunTemplateSO")]
public class GunStats : ScriptableObject
{
    [Header("Maximum amount of ammo at the start of the game")]
    public int maxAmmo;

    [Header("Time in between shooting")]
    public float cooldownTime;


    [Header("Bullet stats")]
    public int bulletDamage;

    [Header("PhysicalBullet")]
    public GameObject bulletObject;
    public float bulletSpeed;

}
