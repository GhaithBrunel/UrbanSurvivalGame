

using UnityEngine;

public class CityLayoutGenerator : MonoBehaviour
{
    public GameObject[] buildingPrefabs; // Array of building prefabs
    public int cityWidth = 10;
    public int cityHeight = 10;
    public float buildingSpacing = 15f;

    // Start is called before the first frame update
    void Start()
    {
        GenerateCityLayout();
    }

    void GenerateCityLayout()
    {
        for (int x = 0; x < cityWidth; x++)
        {
            for (int z = 0; z < cityHeight; z++)
            {
                Vector3 position = new Vector3(x * buildingSpacing, 0, z * buildingSpacing);
                Instantiate(buildingPrefabs[Random.Range(0, buildingPrefabs.Length)], position, Quaternion.identity);
            }
        }
    }
}