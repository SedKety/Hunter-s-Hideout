using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum HarmlessEntityStates
{
    scared,
    searching,
    running, 
    standing
}
public abstract  class HarmlessEntity : Entity
{
    public HarmlessEntityStates currentState;
    public NavMeshAgent agent;

    public float runSpeed;
    public float normalSpeed;

    //Activates method depended on what state the entity is in
    public virtual void ActOnState(HarmlessEntityStates _state) { }
}
