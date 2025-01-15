using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
    public static DroneManager Instance;
    public static List<DroneRestPad> restPads = new List<DroneRestPad>();

    public Queue<GameObject> boughtItems = new Queue<GameObject>();
    public Queue<Entity> retrievableItems = new Queue<Entity>();

    Coroutine coroutine;
    void Start()
    {
        EntityManager.OnEntityDeathAction += OnEntityDeath;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartPackageCoroutine(GameObject package)
    {
        StartCoroutine(GetPackage(package));
    }
    private IEnumerator GetPackage(GameObject package)
    {
        boughtItems.Enqueue(package);
        while (boughtItems.Count > 0)
        {
            if (TryGetValidDronePad(out DroneRestPad restPad))
            {
                print("Found a restpad");
                StartCoroutine(restPad.drone.RetrievePackage(package));
                boughtItems.Dequeue();
            }
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }


    public void OnEntityDeath(Entity entity)
    {
        print(entity.name + " Has died");
        retrievableItems.Enqueue(entity);
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
         coroutine = StartCoroutine(SendDroneToRetrieveItem());

    }


    public IEnumerator SendDroneToRetrieveItem()
    {
        while(retrievableItems.Count > 0)
        {
            if(TryGetValidDronePad(out DroneRestPad restPad))
            {
                restPad.drone.ActOnStateRetrieve(retrievableItems.Peek());
                retrievableItems.Dequeue();
            }
            //print("Still searching");
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }



    public bool TryGetValidDronePad(out DroneRestPad restPad)
    {
        for(int i = 0; i < restPads.Count; i++)
        {
            //print("Attempted restpad");
            if(restPads[i].drone.state == DroneStates.resting)
            {
                restPad = restPads[i];
                return true;
            }
        }
        restPad = null;
        return false;
    }
}
