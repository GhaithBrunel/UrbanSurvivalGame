using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public float terrainScale = 0.1f;
    public float terrainHeightMultiplier = 5f;
    public GameObject terrainPrefab;
    public float blockSize = 10f;

    public void GenerateTerrain(int width, int height, Vector3 startPosition)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * blockSize + startPosition.x, 0, y * blockSize + startPosition.z);

                // Rename 'height' to avoid conflict with the 'height' parameter
                float terrainHeight = Mathf.PerlinNoise(x * terrainScale, y * terrainScale) * terrainHeightMultiplier;
                Vector3 terrainPosition = new Vector3(position.x, terrainHeight, position.z);

                Instantiate(terrainPrefab, terrainPosition, Quaternion.identity);
            }
        }
    }
}



