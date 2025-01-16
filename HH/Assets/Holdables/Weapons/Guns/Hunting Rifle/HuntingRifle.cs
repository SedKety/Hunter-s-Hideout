using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HuntingRifle : Gun
{
    public Transform trigger, emptyMagPos, fullMagPos;
    public bool movePos;
    public Vector3 ejectionForce;

    public float ammoDeleteTime;
    public override void Awake()
    {
        base.Awake();
        if (movePos)
        {
            trigger.position = emptyMagPos.position;
        }
    }
    protected override void Shoot()
    {
        base.Shoot();
        if(magazine.currentAmmo <= 0)
        {
            print("No ammo, Very L, no rizz");
            if (movePos) { trigger.position = emptyMagPos.position; }
            var mag = magazine;
            mag.DeAttach();
            Vector3 relativeEjectionForce = transform.TransformDirection(ejectionForce);
            mag.rb.AddForce(relativeEjectionForce, ForceMode.Impulse);
            mag.transform.parent = null;
            StartCoroutine(DeleteBulletTimer(mag.gameObject));
        }
    }
    public override void OnAttach(Attachables objectToAttach)
    {
        var attachableStruct = attachables.Where(a => a.attachable.GetComponent<Attachables>().GetType() == objectToAttach.GetType() && a.attachableTransform.childCount == 0).FirstOrDefault();
        if (attachableStruct.attachableTransform != null)
        {
            GameObject attachableGameObject = objectToAttach.gameObject;
            attachableGameObject.transform.position = attachableStruct.attachableTransform.position;
            attachableGameObject.transform.rotation = attachableStruct.attachableTransform.rotation;
            attachableGameObject.transform.parent = attachableStruct.attachableTransform;
            objectToAttach.OnAttach(this);
        }
        else
        {
            Debug.LogWarning("Attachable could not be found or has already been attached." );
        }
        if (magazine != null)
        {
            if (magazine.currentAmmo > 0)
            {   
                if (movePos) { trigger.position = fullMagPos.position; }
            }
        }

    }

    private IEnumerator DeleteBulletTimer(GameObject bullet)
    {
        yield return new WaitForSeconds(ammoDeleteTime);
        Destroy(bullet);
    }
}
