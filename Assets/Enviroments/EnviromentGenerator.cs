using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentGenerator : MonoBehaviour
{
    [Header("Ground mesh generation variables")]
    public int gridWidth;
    public int gridHeight;
    public float spaceBetweenVertsMultiplier;

    //decides the scale of the noise, 
    public float frequency;

    public MeshFilter groundMeshFilter;
    private Mesh groundMesh;



    [Header("Debug bools")]
    public bool debugVerts;
    public bool debugTris;
    public bool debugNormals;

    public void Start()
    {
        GenerateGroundMesh();
    }
    [ContextMenu("GenerateGroundMesh")]
    public void GenerateGroundMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        float perlinvalue = 0f;

        //Generates the verts for the ground mesh
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                perlinvalue = Random.Range(-1f, 1f); //Mathf.PerlinNoise(x * frequency, z * frequency);
                print(perlinvalue);
                verts.Add(new Vector3(x * spaceBetweenVertsMultiplier, perlinvalue, z * spaceBetweenVertsMultiplier));
            }
        }

        //Generates the tri's  for the groundmesh
        for (int z = 0; z < gridHeight - 1; z++) 
        {
            for (int x = 0; x < gridWidth - 1; x++)
            {
                int bottomLeft = z * gridWidth + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + gridWidth;
                int topRight = topLeft + 1;

                tris.Add(bottomLeft);
                tris.Add(topRight);
                tris.Add(topLeft);

                tris.Add(bottomLeft);
                tris.Add(bottomRight);
                tris.Add(topRight);
            }
        }
        if (groundMesh == null)
        {
            groundMesh = new Mesh();
        }
        groundMesh.vertices = verts.ToArray();
        groundMesh.triangles = tris.ToArray();
        groundMeshFilter.mesh = groundMesh;
    }
    public void OnDrawGizmos()
    {
        if (debugVerts)
        {
            foreach (Vector3 v3 in groundMesh.vertices)
            {
                Gizmos.DrawCube(v3, new Vector3(0.5f, 0.5f, 0.5f));
            }
        }
    }
}
