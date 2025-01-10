using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ammo : Holdable
{
    public int connectionRange;
    //public override IEnumerator WhileHeld()
    //{
    //    var guns = FindObjectsByType<Gun>(FindObjectsSortMode.None);
    //    while (heldByPlayer)
    //    {
    //        Gun gun = (Gun)guns.Where(a => Vector3.Distance(a.transform.position, transform.position) < connectionRange);
    //        if (gun != null)
    //        {
    //            print(gun.name); ;
    //        }
    //    }
    //    yield return null;
    //}
}
