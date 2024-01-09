using UnityEngine;
using UnityEngine.AI;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject[] prefabsToSpawn;
    public int numberOfInstances = 10;

    void Start()
    {
        SpawnPrefabs();
    }

    void SpawnPrefabs()
    {
        for (int i = 0; i < numberOfInstances; i++)
        {
            GameObject prefabToSpawn = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];
            Vector3 randomPosition = RandomNavmeshLocation();
            Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);
        }
    }

    Vector3 RandomNavmeshLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        int t = Random.Range(0, navMeshData.indices.Length - 3);

        Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]],
            navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
        point = Vector3.Lerp(point,
            navMeshData.vertices[navMeshData.indices[t + 2]], Random.value);

        return point;
    }
}
