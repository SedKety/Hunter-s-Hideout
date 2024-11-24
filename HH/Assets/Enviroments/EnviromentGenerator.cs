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

    //Decides The height difference in the perlin noise
    public float perlinNoiseMultiplier;

    //Increases the height of the terrain dependent on how far away you are
    public float distanceHightMultiplier;
    public MeshFilter groundMeshFilter;
    private Mesh groundMesh;

    //Center of the groundmesh
    private Vector3 centerPoint;

    [Header("Debug bools")]
    public bool debugVerts;
    public bool debugTris;
    public bool debugNormals;

    public void Start()
    {
        centerPoint = new Vector3((gridWidth / 2) * spaceBetweenVertsMultiplier, 0, (gridHeight / 2) * spaceBetweenVertsMultiplier);
        //makes sure the frequency is not a whole number since the mathf.perlin only works with fractional numbers
        if (frequency == (int)frequency)
        {
            frequency += 0.001f;
        }
        if (groundMeshFilter == null) { groundMeshFilter = gameObject.AddComponent<MeshFilter>(); }
        if (gameObject.GetComponent<MeshRenderer>() == null) { gameObject.AddComponent<MeshRenderer>(); }
        GenerateGroundMesh();
    }
    [ContextMenu("GenerateGroundMesh")]
    public void GenerateGroundMesh()
    {
        centerPoint = new Vector3((gridWidth / 2) * spaceBetweenVertsMultiplier, 0, (gridHeight / 2) * spaceBetweenVertsMultiplier);
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        float perlinvalue = 0f;
        //Generates the verts for the ground mesh
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                perlinvalue = Mathf.PerlinNoise(x * frequency / 2, z * frequency / 2) * perlinNoiseMultiplier;
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
        groundMesh.RecalculateBounds();
        groundMesh.RecalculateNormals();

        groundMeshFilter.mesh = groundMesh;
    }
    public void OnDrawGizmos()
    {
        if (debugVerts)
        {
            foreach (Vector3 v3 in groundMesh.vertices)
            {
                Gizmos.DrawCube(v3, new Vector3(0.1f * spaceBetweenVertsMultiplier, 0.1f * spaceBetweenVertsMultiplier, 0.1f * spaceBetweenVertsMultiplier));
            }
        }

        if (debugTris)
        {
            for (int i = 0; i < groundMesh.triangles.Length; i++)
            {
                if (groundMesh.vertices[groundMesh.triangles[i + 1]] != null)
                {
                    Gizmos.DrawLine(groundMesh.vertices[groundMesh.triangles[i]], groundMesh.vertices[groundMesh.triangles[i + 1]]);
                }
                else
                {
                    break;
                }

            }
        }
        Gizmos.DrawCube(centerPoint, new Vector3(10, 10, 10));
    }
}
