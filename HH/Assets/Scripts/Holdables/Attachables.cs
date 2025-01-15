using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachables : Holdable
{
    [Header("Attachable Variables")]
    public float attachRange = 0.25f;
    private bool attached;

    public Holdable attachedItem;
    public LayerMask excludedLayers;

    public override void Awake()
    {
        base.Awake();
        rb.excludeLayers = excludedLayers;
    }

    public override IEnumerator WhileHeld()
    {
        while (heldByPlayer)
        {
            yield return new WaitForEndOfFrame();
            TryToAttach();
        }
    }

    public override void Pickup(InteractionController hand)
    {
        base.Pickup(hand);
        if(attached)
        {
            OnDeAttach();
        }
    }
    protected virtual void TryToAttach()
    {
        GameObject attemptedObject = null;

        if (Hand.Left != null && Hand.Left.interactionController != null && Hand.Left.interactionController.heldObject != null)
        {
            var leftHeldObject = Hand.Left.interactionController.heldObject.gameObject;

            if (leftHeldObject != gameObject)
            {
                attemptedObject = leftHeldObject;
            }
        }

        if (attemptedObject == null && Hand.Right != null && Hand.Right.interactionController != null && Hand.Right.interactionController.heldObject != null)
        {
            var rightHeldObject = Hand.Right.interactionController.heldObject.gameObject;

            if (rightHeldObject != gameObject)
            {
                attemptedObject = rightHeldObject;
            }
        }

        if (attemptedObject == null)
        {
            return;
        }

        float distance = Vector3.Distance(attemptedObject.transform.position, transform.position);
        if (distance >= attachRange)
        {
            return;
        }
        if (attemptedObject.TryGetComponent<IAttachable>(out IAttachable iAttachable))
        {
            if (iAttachable.CanAttach(this))
            {
                iAttachable.OnAttach(this);
            }
        }
    }

    public virtual void OnAttach(Holdable holdable = null)
    {
        var heldObject = Hand.Left.interactionController.heldObject;
        if (heldObject != null)
        {
            if (heldObject == this)
            {
                Hand.Left.interactionController.StopHoldingObject();
            }
            else
            {
                Hand.Right.interactionController.StopHoldingObject();
            }
            rb.isKinematic = true;
            attached = true;
        }
        if (holdable != null)
        {
            attachedItem = holdable;
        }

    }
    protected virtual void OnDeAttach()
    {
        attached = false;
        attachedItem = null;
    }

}
