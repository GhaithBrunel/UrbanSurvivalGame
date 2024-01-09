using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PrefabSpawnData
{
    public GameObject prefab;
    public int count;
}

public class PrefabSpawner : MonoBehaviour
{
    public PrefabSpawnData[] spawnData;

    void Start()
    {
        foreach (var data in spawnData)
        {
            SpawnPrefabs(data.prefab, data.count);
        }
    }

    void SpawnPrefabs(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = RandomNavmeshLocation();
            Instantiate(prefab, randomPosition, Quaternion.identity);
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

