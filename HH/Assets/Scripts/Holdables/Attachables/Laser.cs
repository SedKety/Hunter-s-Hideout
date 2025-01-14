using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Attachables
{
    public GameObject laserBeam;

    public override void OnAttach(Holdable holdable = null)
    {
        base.OnAttach();
        laserBeam.SetActive(true);
    }
    protected override void OnDeAttach()
    {
        base.OnDeAttach();
        laserBeam.SetActive(false);
    }
} 
