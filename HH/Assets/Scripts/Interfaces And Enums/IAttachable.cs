using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttachable 
{
    public bool CanAttach(Attachables attemptedAttachable)
    {
        return false;
    }
    public void OnAttach(Attachables objectToAttach)
    {

    }
}
