using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using TMPro;

public class Hideout : MonoBehaviour, IDamagable
{
    public static Hideout instance;

    // Change to non-static for inspector use
    public static Transform[] hideOutHitPosition;
    public Transform[] hideOutHitPositionWrapper;

    [SerializeField]private int health;
    public TMP_Text healthText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            hideOutHitPosition = hideOutHitPositionWrapper;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Update()
    {
        if (healthText)
        {


            healthText.text = health.ToString();
        }
    }
    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        if (health <= 0)
        {
            OnHideOutDeath();
        }
    }

    public void OnHideOutDeath()
    {
        // Handle hideout death logic here
    }

    public static Transform GetRandomHideoutPos()
    {
        return hideOutHitPosition[Random.Range(0, hideOutHitPosition.Length)];
    }
    public static Transform GetHideOutPosClosestToV3(Vector3 posToCheck)
    {
        return hideOutHitPosition
       .OrderBy(pos => Vector3.Distance(pos.position, posToCheck))
       .FirstOrDefault();
    }
}
