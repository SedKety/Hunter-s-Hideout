
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SceneryObjects
{
    public string name;
    public int amountToGenerate;
    public GameObject[] objects;

}
public class EnviromentGenerator : MonoBehaviour
{
    [Header("Ground mesh generation variables")]

    public int gridWidth;
    public int gridHeight;

    private int gridSize;
    //this decides the distance between each vertice
    public float vertOffset;

    //Decides The height difference in the perlin noise
    public float perlinNoiseMultiplier;

    //Increases the height of the terrain dependent on how far away you are
    //currently has no purpose but will most likely work in the future
    public float distanceHightMultiplier;


    public MeshFilter groundMeshFilter;
    private Mesh groundMesh;
    private MeshCollider groundCollider;

    //decides the scale of the noise, 
    [Range(0f, 1f)]
    public float frequency;

    [Header("Scenery generation variables")]
    public LayerMask groundLayerMask;
    public SceneryObjects[] sceneryObjects;

    public GameObject hideout;


    //Center of the groundmesh
    private Vector3 centerPoint;

    [Header("Debug bools")]
    public bool debugVerts;
    public bool debugTris;
    public bool debugNormals;

    public void Start()
    {
        centerPoint = new Vector3((gridWidth / 2) * vertOffset, 0, (gridHeight / 2) * vertOffset);
        gridSize = gridWidth * gridHeight;
        //makes sure the frequency is not a whole number since the mathf.perlin only works with fractional numbers
        if (frequency == (int)frequency)
        {
            frequency += 0.001f;
        }
        if (groundMeshFilter == null) { groundMeshFilter = gameObject.AddComponent<MeshFilter>(); }
        if (gameObject.GetComponent<MeshRenderer>() == null) { gameObject.AddComponent<MeshRenderer>(); }
        if(gameObject.GetComponent<MeshCollider>() == null) { groundCollider = gameObject.AddComponent<MeshCollider>(); }
        else
        {
            groundCollider = gameObject.GetComponent<MeshCollider>();
        }
        GenerateGroundMesh();
    }
    [ContextMenu("GenerateGroundMesh")]
    public void GenerateGroundMesh()
    {
        centerPoint = new Vector3((gridWidth / 2) * vertOffset, 0, (gridHeight / 2) * vertOffset);
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        float perlinvalue = 0f;

        //makes the terrain higher if its further away from the center point
        //Generates the verts for the ground mesh
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                perlinvalue = Mathf.PerlinNoise(x * frequency / 2, z * frequency / 2) * perlinNoiseMultiplier;
                //print(perlinvalue);
                //float distanceHeightIncrease = CalculateHeightAccordingToOffsetToCenter(new Vector3(x * spaceBetweenVertsMultiplier, 0f, z * spaceBetweenVertsMultiplier));

                verts.Add(new Vector3(x * vertOffset, perlinvalue, z * vertOffset));
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
        groundCollider.sharedMesh = groundMesh;

        //doesnt work yet, will fix tuesday
        AddScenery();
    }

    public void AddScenery()
    {
        float maxX = (gridWidth - 1) * vertOffset;
        float maxZ = (gridHeight - 1) * vertOffset;

        //Adds all the SceneryObjects
        foreach (SceneryObjects scenery in sceneryObjects)
        {
            for (int i = 0; i < scenery.amountToGenerate; i++)
            {
                float randomX = Random.Range(0, maxX);
                float randomZ = Random.Range(0, maxZ);
                var rayPos = new Vector3(randomX, 1000, randomZ);

                Ray ray = new Ray(rayPos, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
                {
                    Instantiate(scenery.objects[Random.Range(0, scenery.objects.Length)], hit.point, Quaternion.identity, transform);
                }
            }
        }
        SpawnHideout();
    }

    public void SpawnHideout()
    {
        Instantiate(hideout, CalculateCenterPoint(), Quaternion.identity);
        //GameManager.Instance.spawnPos = spawnPoint;
        //GameManager.Instance.OnMapGenerated?.Invoke();
    }
    //returns the amount needed to be added to the y value of any vert created in the mesh according to how far away it is from the center of the mesh
    public float CalculateHeightAccordingToOffsetToCenter(Vector3 vertPos)
    {
        vertPos.y = 0;
        return (gridHeight * gridWidth / Vector3.Distance(vertPos, centerPoint)) * distanceHightMultiplier;
    }

    public Vector3 GetHighestPointInMesh()
    {
        Vector3 highestVert = Vector3.zero;
        foreach (Vector3 vert in groundMesh.vertices)
        {
            if(vert.y > highestVert.y) highestVert = vert;
        }
        return highestVert;
    }
    public Vector3 CalculateCenterPoint()
    {
        Vector3 middleOfMap = centerPoint;
        middleOfMap.y += 100;
        Ray ray = new Ray(middleOfMap, Vector3.down);
        Debug.DrawRay(middleOfMap, Vector3.down, Color.red, 10f);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
        {
            return hit.point;
        }
        else
        {
            return centerPoint;
        }
    }

    public void OnDrawGizmos()
    {
        if (debugVerts)
        {
            foreach (Vector3 v3 in groundMesh.vertices)
            {
                Gizmos.DrawCube(v3, new Vector3(0.1f * vertOffset, 0.1f * vertOffset, 0.1f * vertOffset));
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
