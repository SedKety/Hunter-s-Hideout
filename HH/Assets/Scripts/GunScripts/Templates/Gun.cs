using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
public struct Attachable
{
    public string name;
    public GameObject attachable;
    public Transform attachableTransform;
}
public class Gun : Holdable, IAttachable
{
    #region variables
    public GunStats gunStatsSO;
    public Transform shootPoint;
    public Magazine magazine;
    public ParticleSystem muzzleFlare;

    [SerializeField] private AudioSource _audioSource;

    public List<Attachable> attachables;
    //These are the variables taken from the GunStats Scriptable Object
    #region private gunstats variables
    private int _maxAmmo;

    private float _cooldownTime;

    private int _bulletDamage;
    private float _bulletDistance;
    private GameObject _bulletObject;
    private float _bulletSpeed;

    //private int _reloadTime;
    #endregion


    #endregion


    #region methods
    public override void Awake()
    {
        base.Awake();
        InitStats();
    }
    //this method is called to perform tasks assigned to certain holdables upon using, such as healing, shooting and filling hunger,
    //in this case it will shoot
    public override void OnItemUse()
    {
        StartCoroutine(TryToShoot());
    }

    //Take the stats from the gunStats scriptable object and applies those to this object
    public void InitStats()
    {
        _maxAmmo = gunStatsSO.maxAmmo;
        _cooldownTime = gunStatsSO.cooldownTime;
        _bulletDamage = gunStatsSO.bulletDamage;
        _bulletObject = gunStatsSO.bulletObject;
        _bulletSpeed = gunStatsSO.bulletSpeed;
        //_reloadTime = gunStatsSO.reloadTime;
    }

    //Checks whether there is enough ammo in the gun to continue shooting, otherwise returns false
    public bool CanShoot()
    {
        if (magazine != null && magazine.currentAmmo >= 0)
        {
            return true;
        }
        return false;
    }

    //Attempts to shoot, dependend on whether the player can shoot or not
    public IEnumerator TryToShoot()
    {
        if (CanShoot())
        {
            Shoot();
        }
        yield break;
    }

    public void Shoot()
    {
        GameObject bullet = Instantiate(_bulletObject, shootPoint.position, shootPoint.rotation);
        bullet.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, _bulletSpeed), ForceMode.Impulse);
        bullet.GetComponent<Bullet>().damage = _bulletDamage;
        magazine.currentAmmo--;
        muzzleFlare.Play();
        if (_audioSource)
        {
            _audioSource.Play();
        }
    }


    
    public bool CanAttach(Attachables attemptedAttachable)
    {
        bool canAttach = attachables.Where(a => a.attachable.GetComponent<Attachables>().GetType() == attemptedAttachable.GetType() && a.attachableTransform.childCount == 0).Any();
        print(canAttach);

        return canAttach;
    }

    public void OnAttach(Attachables objectToAttach)
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
            Debug.LogWarning("Attachable could not be found or has already been attached.");
        }
    } 

    #endregion
}
