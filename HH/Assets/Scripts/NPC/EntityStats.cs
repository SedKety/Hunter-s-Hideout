using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjects/EntityStats")]
public class EntityStats : ScriptableObject
{
    [Header("Health variables")]
    public int health;
    public bool canHeal;
    public int healthRegen;
    public float healthRegenTimer;
}
