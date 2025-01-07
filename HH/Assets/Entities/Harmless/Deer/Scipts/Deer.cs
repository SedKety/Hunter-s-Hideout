using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Deer : Entity
{
    public override void TakeDamage(int damageTaken)
    {
        _health -= damageTaken;
        if (_health <= 0)
        {
            OnDeath();
            return;
        }
        ActOnState(EntityStates.fleeing);
    }

}
