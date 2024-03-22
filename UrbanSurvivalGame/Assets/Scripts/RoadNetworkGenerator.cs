using UnityEngine;

public class RoadNetworkGenerator : MonoBehaviour
{
    public GameObject roadPrefab; // Assign road prefab
    public CityLayoutGenerator cityLayoutGenerator;

    void Start()
    {
        if (cityLayoutGenerator == null) return;

        GenerateRoadNetwork();
    }

    void GenerateRoadNetwork()
    {
        // Logic to generate roads based on the city layout
        // For simplicity, creating roads in a grid pattern around the city blocks
        float roadWidth = 2f; // Width of the road
        for (int x = 0; x <= cityLayoutGenerator.cityWidth; x++)
        {
            for (int z = 0; z <= cityLayoutGenerator.cityHeight; z++)
            {
                // Vertical roads
                Instantiate(roadPrefab, new Vector3(x * cityLayoutGenerator.buildingSpacing, 0, z * cityLayoutGenerator.buildingSpacing - roadWidth / 2), Quaternion.identity);

                // Horizontal roads
                Instantiate(roadPrefab, new Vector3(x * cityLayoutGenerator.buildingSpacing - roadWidth / 2, 0, z * cityLayoutGenerator.buildingSpacing), Quaternion.Euler(0, 90, 0));
            }
        }
    }
}

