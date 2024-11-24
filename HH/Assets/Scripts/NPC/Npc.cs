using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour, IDamagable
{
    public NpcStats npcStats;

    #region npcstat variables
    private int _health;
    private bool _canHeal;
    private int _healthRegen;
    private float _healthRegenTimer;
    #endregion
    //Take the stats from the npcStats scriptable object and applies those to this object
    public void InitStats()
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
    public void TakeDamage(int damageTaken)
    {
        _health -= damageTaken;
        if(_health <= 0)
        {
            OnDeath();
        }
    }
    //
    public void OnDeath()
    {

    }
}
