using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : Entity
{
    public override void TakeDamage(int damageTaken)
    {
        base.TakeDamage(damageTaken);
       ActOnState(EntityStates.attacking);
    }
}
