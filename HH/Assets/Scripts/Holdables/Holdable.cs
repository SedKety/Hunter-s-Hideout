using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
[RequireComponent(typeof(Rigidbody))]
public abstract class Holdable : MonoBehaviour, IUsable
{
    public Rigidbody rb;

    public bool canBeThrown = true;
    public bool held;


    //this method is called to perform tasks assigned to certain holdables upon using, such as healing, shooting and filling hunger.
    public abstract void OnItemUse();

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
}
