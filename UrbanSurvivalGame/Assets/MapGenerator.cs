using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI; // Include this for NavMesh components
using Unity.AI.Navigation;

public enum MapSection
{
    CityCenter,
    UrbanHomes,
    Industrial,
    Park
}



public class MapGenerator : MonoBehaviour
{
    public GameObject[] cityCenterBuildings;
    public GameObject[] urbanHomeBuildings;
    public GameObject[] industrialBuildings;
    public GameObject[] parkBuildings;

    public GameObject grassPrefab;
    public GameObject roadPrefab;
    public GameObject intersectionPrefab;
    public int mapWidth = 20;
    public int mapHeight = 20;
    public float blockSize = 10f;
    public int roadFrequency = 5;
    public LayerMask buildingsAndRoadsLayer;

    public Vector3 cityCenterBuildingScale = new Vector3(1, 1, 1);
    public Vector3 urbanHomeBuildingScale = new Vector3(1, 1, 1);
    public Vector3 industrialBuildingScale = new Vector3(1, 1, 1);
    public Vector3 parkBuildingScale = new Vector3(1, 1, 1);

    public NavMeshSurface navMeshSurface;

    private Dictionary<MapSection, Vector3> sectionScales;
    private int[,] buildingDensityMap;
    private PerlinNoiseGenerator perlinNoiseGenerator;
    public float noiseScale = 0.1f;
    public int seed;

    private void Start()
    {
        InitializeMap();
    }

    public void InitializeMap()
    {
        sectionScales = new Dictionary<MapSection, Vector3>
        {
            { MapSection.CityCenter, cityCenterBuildingScale },
            { MapSection.UrbanHomes, urbanHomeBuildingScale },
            { MapSection.Industrial, industrialBuildingScale },
            { MapSection.Park, parkBuildingScale }
        };

        buildingDensityMap = new int[mapWidth, mapHeight];
        perlinNoiseGenerator = new PerlinNoiseGenerator(noiseScale);

        Random.InitState(seed);
        GenerateMap();
        BakeNavMesh();
    }


    public void RegenerateMap(int width, int height, int newSeed)
    {
        mapWidth = width;
        mapHeight = height;
        seed = newSeed;
        Random.InitState(seed);
        InitializeMapParameters();
        GenerateMap();
        BakeNavMesh();
    }
    private void InitializeMapParameters()
    {
        sectionScales = new Dictionary<MapSection, Vector3>
        {
            { MapSection.CityCenter, cityCenterBuildingScale },
            { MapSection.UrbanHomes, urbanHomeBuildingScale },
            { MapSection.Industrial, industrialBuildingScale },
            { MapSection.Park, parkBuildingScale }
        };

        buildingDensityMap = new int[mapWidth, mapHeight];
        perlinNoiseGenerator = new PerlinNoiseGenerator(noiseScale);
    }
    private void BakeNavMesh()
    {
        // Ensure NavMeshSurface is attached and valid
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh(); // This bakes the NavMesh at runtime
        }
        else
        {
            Debug.LogError("NavMeshSurface component not found or not set.");
        }
    }

    private void GenerateMap()
    {
        // Clear existing map
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 position = new Vector3(x * blockSize, 0, y * blockSize);

                if (IsIntersection(x, y))
                {
                    Instantiate(intersectionPrefab, position, Quaternion.identity, transform);
                }
                else if (IsRoadTile(x, y))
                {
                    Instantiate(roadPrefab, position, Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(grassPrefab, position, Quaternion.identity, transform);
                    TryPlaceRandomBuilding(position, x, y);
                }
            }
        }
    }

    private bool IsRoadTile(int x, int y)
    {
        return x % roadFrequency == 0 || y % roadFrequency == 0;
    }

    private bool IsIntersection(int x, int y)
    {
        return x % roadFrequency == 0 && y % roadFrequency == 0;
    }

  

    private void TryPlaceRandomBuilding(Vector3 position, int x, int y)
    {
        float perlinValue = perlinNoiseGenerator.GetNoiseValue(x, y);
        MapSection section = DetermineSectionFromPerlin(perlinValue);
        GameObject[] prefabsToUse = GetPrefabsForSection(section);

        if (prefabsToUse.Length == 0) return;

        int densityScore = CalculateDensityScore(x, y);
        List<GameObject> filteredPrefabs = FilterPrefabsByDensity(prefabsToUse, densityScore);

        if (filteredPrefabs.Count == 0)
        {
            filteredPrefabs.AddRange(prefabsToUse);
        }

        while (filteredPrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredPrefabs.Count);
            GameObject selectedPrefab = filteredPrefabs[randomIndex];
            Bounds totalBounds = CalculateTotalBounds(selectedPrefab);

            if (CanPlaceBuilding(position, totalBounds))
            {
                GameObject building = Instantiate(selectedPrefab, position, Quaternion.identity);
                building.transform.localScale = new Vector3(
                    building.transform.localScale.x * sectionScales[section].x,
                    building.transform.localScale.y * sectionScales[section].y,
                    building.transform.localScale.z * sectionScales[section].z
                );
                UpdateDensityMap(x, y);
                return;
            }
            else
            {
                filteredPrefabs.RemoveAt(randomIndex); // Remove prefab that didn't fit
            }
        }
    }


    private MapSection DetermineSectionFromPerlin(float perlinValue)
    {
        if (perlinValue < 0.25) return MapSection.CityCenter;
        else if (perlinValue < 0.5) return MapSection.UrbanHomes;
        else if (perlinValue < 0.75) return MapSection.Industrial;
        else return MapSection.Park;
    }

    private GameObject[] GetPrefabsForSection(MapSection section)
    {
        switch (section)
        {
            case MapSection.CityCenter:
                return cityCenterBuildings;
            case MapSection.UrbanHomes:
                return urbanHomeBuildings;
            case MapSection.Industrial:
                return industrialBuildings;
            case MapSection.Park:
                return parkBuildings;
            default:
                return new GameObject[0];
        }
    }

    private List<GameObject> FilterPrefabsByDensity(GameObject[] prefabs, int densityScore)
    {
        List<GameObject> filteredList = new List<GameObject>();
        foreach (var prefab in prefabs)
        {
            // Example condition: Larger buildings for higher density
            Bounds prefabBounds = CalculateTotalBounds(prefab);
            if ((densityScore > 3 && prefabBounds.size.y >= 5) || (densityScore <= 3))
            {
                filteredList.Add(prefab);
            }
        }
        return filteredList;
    }


  

   

    private int CalculateDensityScore(int x, int y)
    {
        int score = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int checkX = x + i;
                int checkY = y + j;
                if (checkX >= 0 && checkX < mapWidth && checkY >= 0 && checkY < mapHeight)
                {
                    score += buildingDensityMap[checkX, checkY];
                }
            }
        }
        return score;
    }

    private void UpdateDensityMap(int x, int y)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int checkX = x + i;
                int checkY = y + j;
                if (checkX >= 0 && checkX < mapWidth && checkY >= 0 && checkY < mapHeight)
                {
                    buildingDensityMap[checkX, checkY]++;
                }
            }
        }
    }

    private bool CanPlaceBuilding(Vector3 position, Bounds bounds)
    {
        Collider[] colliders = Physics.OverlapBox(position, bounds.extents, Quaternion.identity, buildingsAndRoadsLayer);
        return colliders.Length == 0;
    }

    private Bounds CalculateTotalBounds(GameObject prefab)
    {
        MeshRenderer[] meshRenderers = prefab.GetComponentsInChildren<MeshRenderer>();
        if (meshRenderers.Length == 0) return new Bounds();

        Bounds totalBounds = meshRenderers[0].bounds;
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            totalBounds.Encapsulate(meshRenderer.bounds);
        }
        return totalBounds;
    }


   
}












































