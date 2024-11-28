using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MapManager : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;
    public void Start()
    {
        Invoke(nameof(GenerateNavMesh), 1f);
    }
    public void GenerateNavMesh()
    {
        if (navMeshSurface == null)
        {
            navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
        }
        navMeshSurface.BuildNavMesh();
    }
}
