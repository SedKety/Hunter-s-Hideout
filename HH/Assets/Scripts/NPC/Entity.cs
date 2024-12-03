using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamagable
{
    public EntityStats npcStats;

    #region npcstat variables
    protected int _health;
    protected bool _canHeal;
    protected int _healthRegen;
    protected float _healthRegenTimer;
    protected float _moneyDroppedUponDeath;
    #endregion

    public bool isDead;
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

    //Tries to generate a position to move to, will return positive if it has found one
    public virtual bool GetPosToMoveTo() { return false; }

    //This method is executed upon health being lower then 0
    [ContextMenu("OnDeath")]
    protected virtual void OnDeath() {  }
}
