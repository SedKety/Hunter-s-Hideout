using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamagable
{
    public EntityStats npcStats;

    #region npcstat variables
    [SerializeField]private int _health;
    private bool _canHeal;
    private int _healthRegen;
    private float _healthRegenTimer;
    #endregion
    //Take the stats from the npcStats scriptable object and applies those to this object
    public virtual void Awake()
    {
        InitStats();
    }
    public virtual void Start()
    {

    }
    public virtual void InitStats()
    {
        _health = npcStats.health;
        if(npcStats.canHeal == true)
        {
            _healthRegen = npcStats.healthRegen;
            _healthRegenTimer = npcStats.healthRegenTimer;
            _canHeal = npcStats.canHeal;
        }
    }
    //This method is called upon being hit
    [ContextMenu("TakeDamage")]
    public virtual void TakeDamage(int damageTaken)
    {
        _health -= damageTaken;
        if(_health <= 0)
        {
            OnDeath();
        }
    }

    //Finds a position to which it will move to
    public virtual bool GetPosToMoveTo() { return false; }

    //This method is executed upon health being lower then 0
    public virtual void OnDeath() { }
}
