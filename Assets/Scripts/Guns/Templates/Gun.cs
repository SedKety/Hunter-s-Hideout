using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Gun : Holdable
{
    public GunStats gunStatsSO;
    public Transform shootPoint;


    private int _currentAmmo;

    private bool _isReloading;
    private bool _onCooldown;

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

    public void Awake()
    {
        InitStats();
    }
    //this method is called when the item is been used
    public override void OnHeldItemUse()
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

        //_reloadTime = gunStatsSO.reloadTime;
    }

    //Checks whether there is enough ammo in the gun to continue shooting, otherwise returns false
    public bool CanShoot()
    {
        if (_currentAmmo >= 0)
        {
            return true;
        }
        //else if (!_isReloading)
        //{
        //    StartCoroutine(Reload());
        //}
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

    //Shoots the right bullet according to the _bulletType variable, 
    public void Shoot()
    {
        GameObject bullet = Instantiate(_bulletObject, shootPoint.position, shootPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity += new Vector3(_bulletSpeed, 0, 0);
        StartCoroutine(StartCooldownTimer());
    }

    //Starts the cooldown for in between shots 
    public IEnumerator StartCooldownTimer()
    {
        yield return new WaitForSeconds(_cooldownTime);
        _onCooldown = false;
    }

}
