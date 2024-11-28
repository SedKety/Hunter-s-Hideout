using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
[Flags]
public enum GrabStyle: byte
{
    ray = 1 ,
    collision = 2,
}
public class Grab : MonoBehaviour
{
    public Transform rayPos;
    public Transform heldItemHolder;

    public float rayRange;

    public GrabStyle grabStyle;

    private Holdable heldObject;


    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed & grabStyle.HasFlag(GrabStyle.ray))
        {
            if (heldObject == null)
            {
                ShootRay();
            }
            else
            {
                StartCoroutine(Drop());
            }
        }
    }

    public int maxVelFrames;

    private Vector3 lastPos;
    private Vector2 lastDir;

    private float[] vel;
    private int velIndex;

    public float velMultiplier;

    private void Start()
    {
        vel = new float[maxVelFrames];
    }

    private void Update()
    {
        if (heldObject == null) return;


        vel[velIndex] = Vector3.Distance(lastPos, transform.position);

        velIndex += 1;
        if(velIndex == maxVelFrames)
        {
            velIndex = 0;
        }

        lastDir = transform.position - lastPos;


        lastPos = transform.position;
    }


    public void OnTriggerEnter(Collider other)
    {
        //maak hier ff button logic achter
        if (!grabStyle.HasFlag(GrabStyle.collision)) { return; }
    }
    //shoots ray :thumbsup:
    public void ShootRay()
    {
        Ray ray = new Ray(rayPos.position, rayPos.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayRange))
        {
            if (hit.transform.TryGetComponent(out Holdable hitItem) && hitItem.held == false)
            {
                hitItem.transform.SetParent(heldItemHolder, false, false);
                hitItem.held = true;

                heldObject = hitItem;
                heldObject.rb.isKinematic = true;
            }
        }
        //chan heeft dit script geschreven
    }


    private IEnumerator Drop()
    {
        heldObject.transform.parent = null;
        heldObject.held = false;
        heldObject.rb.isKinematic = false;


        if (heldObject.canBeThrown)
        {
            float highestVel = -10;
            for (int i = 0; i < maxVelFrames; i++)
            {
                if (vel[i] > highestVel)
                {
                    highestVel = vel[i];
                }
            }

            yield return new WaitForFixedUpdate();

            heldObject.rb.velocity = lastDir * highestVel * velMultiplier;
        }

        heldObject = null;
    }
}
