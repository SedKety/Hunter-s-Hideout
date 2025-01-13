using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamScript : MonoBehaviour
{
    Vector3 scale = Vector3.zero;
    [SerializeField] bool shouldDebug;
    private Transform debugSphere;
    public void Start()
    {
        if (shouldDebug)
        {
            debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            Destroy(debugSphere.GetComponent<SphereCollider>());
        }
    }
    public void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            //the 200 is because the size of the object is 0.005, 0.005 * 200 = 1 
            scale = new Vector3(1, 1, Vector3.Distance(transform.position, hit.point) * 200);
            transform.localScale = scale;
            if (shouldDebug) { debugSphere.localPosition = hit.point; print(hit.collider.gameObject.name);  }

            
        }
        else
        {
            scale = new Vector3 (0, 0, 1000);
        }
    }
}
