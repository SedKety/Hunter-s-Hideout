using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjects/EntityStats")]
public class EntityStats : ScriptableObject
{
    //------------------------------------------------------------------------------------------//Health

    [Header("Health variables")]
    public int health;
    public bool canHeal;
    public int healthRegen;
    public float healthRegenTimer;

    //------------------------------------------------------------------------------------------//Attacking

    [Header("Attacking variables")]
    public int damage;

    public int hitCount;

    [Tooltip("The time in between hitting")]
    public float hitDelay;

    [Tooltip("If a randomly generated number is higher then this the entity will run away instead of attacking")]
    [Range(0, 100)] public int contactWithdrawalChance;

    //------------------------------------------------------------------------------------------//Movement

    [Header("Movement variables")]
    public float movementSpeedNormal;
    public float movementSpeedSprint;

    [Tooltip("How far away the entity can pick a new position to move to")]
    public float distanceRange = 10f;

    [Tooltip("How far away the entity can be from its destination untill its done")]
    public float distanceOffsetTillNextPosition = 1;

    [Header("Standing still variables")]
    [Tooltip("The x is the minumum value and the Y is the maximum value")]
    public Vector2 standStillTime;

    public float standStillChance;

    [Header("Scared variables")]
    public float scaredTime;

    [Tooltip("the higher this is the faster it will return to standing/searching state")]
    public float scaredTimeMultiplier = 1;

    //------------------------------------------------------------------------------------------//Miscelaneous

    [Header("EntityStates Variables")]
    public EntityStates startState;

    [Header("Miscelaneous")]
    public GameObject headGO;

    [SerializeField] public MinMax enemyWeightAmounts;



    protected Animator animator;
}
