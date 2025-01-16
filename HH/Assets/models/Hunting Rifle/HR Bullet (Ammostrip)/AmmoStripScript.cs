using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AmmoStripScript : Attachables
{
    public GameObject bulletObject;
    [SerializeField] private int numOfBullets;
    [SerializeField] private GameObject[] bullets;
    [SerializeField] private bool bulletsLeft;
    private int currentBulletLeftoverCount;

    public void Start()
    {
        bulletsLeft = true;
        currentBulletLeftoverCount = numOfBullets - 1; // Initialize to last bullet
    }

    protected override void TryToAttach()
    {
        if (!bulletsLeft) { return; }

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
                var newSpawnedBullet = Instantiate(bulletObject);
                TurnOffAmmo();
                iAttachable.OnAttach(newSpawnedBullet.GetComponent<Attachables>());
                numOfBullets--;
                if (numOfBullets == 0)
                {
                    bulletsLeft = false;
                }
            }
        }
    }

    public void TurnOffAmmo()
    {
        if (currentBulletLeftoverCount >= 0 && currentBulletLeftoverCount < bullets.Length)
        {
            if (bullets[currentBulletLeftoverCount])
            {
                bullets[currentBulletLeftoverCount].gameObject.SetActive(false);
                currentBulletLeftoverCount--;
            }
        }
    }
}
