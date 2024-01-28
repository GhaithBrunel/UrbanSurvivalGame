using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[System.Serializable]
public class PrefabSpawnData
{
    public GameObject prefab;
    public int count;
    public List<Vector3> positions;
}

public class PrefabSpawner : MonoBehaviour
{
    public PrefabSpawnData[] spawnData;

    public void SpawnPrefabsForNewGame()
    {
        foreach (var data in spawnData)
        {
            data.positions = new List<Vector3>();
            for (int i = 0; i < data.count; i++)
            {
                Vector3 randomPosition = RandomNavmeshLocation();
                Instantiate(data.prefab, randomPosition, Quaternion.identity);
                data.positions.Add(randomPosition);
            }
        }
    }

    public void LoadPrefabs(List<PrefabSpawnData> savedData)
    {
        foreach (var data in savedData)
        {
            foreach (var position in data.positions)
            {
                Instantiate(data.prefab, position, Quaternion.identity);
            }
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

    public List<PrefabSpawnData> GetSpawnData()
    {
        return new List<PrefabSpawnData>(spawnData);
    }
}


