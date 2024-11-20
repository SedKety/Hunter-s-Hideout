using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjects/NpcStats")]
public class NpcStats : ScriptableObject
{
    [Header("Health variables")]
    public int health;
    public bool canHeal;
    public int healthRegen;
    public float healthRegenTimer;
}
