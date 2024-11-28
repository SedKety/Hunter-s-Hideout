using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : HarmlessEntity
{
    //Activates method depended on what state the entity is in
    public abstract void ActOnState();
}
