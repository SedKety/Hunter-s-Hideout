using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class AmmoBox : Holdable
{
    [SerializeField] private GameObject magazine;
    [BurstCompile]
    public override void Pickup(InteractionController hand)
    {
        var localMag = Instantiate(magazine);
        if (localMag.TryGetComponent<Magazine>(out Magazine magazineScript))
        {
            magazineScript.Pickup(hand);
        }
    }
}
