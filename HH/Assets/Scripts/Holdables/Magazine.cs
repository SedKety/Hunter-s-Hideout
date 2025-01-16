using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine : Attachables
{
    [Header("Magazine Variables")]
    [SerializeField] private int maxAmmo;
    public int currentAmmo;

    public override void Awake()
    {
        base.Awake();
        currentAmmo = maxAmmo;
    }
    public override void Pickup(InteractionController hand)
    {
        base.Pickup(hand);
    }
    public override void OnAttach(Holdable holdable = null)
    {
        base.OnAttach(holdable);
        if (holdable != null && holdable.TryGetComponent<Gun>(out Gun heldGun))
        {
            heldGun.magazine = this;
        }
    }
    protected override void OnDeAttach()
    {
        base.OnDeAttach();
        if (attachedItem != null && attachedItem.TryGetComponent<Gun>(out Gun attachedGun))
        {
            attachedGun.magazine = null;
        }
    }
}
