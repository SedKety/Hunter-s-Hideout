using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public delegate void OnEntityDeath(Entity entityHead);
    public static event OnEntityDeath OnEntityDeathAction;


    public static void CallOnEntityDeath(Entity entityHead)
    {
        OnEntityDeathAction?.Invoke(entityHead);
        EntitySpawner.currentEntityCount--;
    }
}
