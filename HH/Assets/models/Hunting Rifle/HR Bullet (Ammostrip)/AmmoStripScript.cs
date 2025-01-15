using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoStripScript : Attachables
{
    public GameObject bulletObject;
    protected override void TryToAttach()
    {
        GameObject attemptedObject = null;

        if (Hand.Left != null && Hand.Left.interactionController != null && Hand.Left.interactionController.heldObject != null)
        {
            var leftHeldObject = Hand.Left.interactionController.heldObject.gameObject;

            if (leftHeldObject != bulletObject)
            {
                attemptedObject = leftHeldObject;
            }
        }

        if (attemptedObject == null && Hand.Right != null && Hand.Right.interactionController != null && Hand.Right.interactionController.heldObject != null)
        {
            var rightHeldObject = Hand.Right.interactionController.heldObject.gameObject;

            if (rightHeldObject != bulletObject)
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
                iAttachable.OnAttach(bulletObject.GetComponent<Attachables>());
            }
        }
    }
}
