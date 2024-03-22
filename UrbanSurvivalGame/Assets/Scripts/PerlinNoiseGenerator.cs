using UnityEngine;

public class PerlinNoiseGenerator
{
    private float scale;

    public PerlinNoiseGenerator(float scale)
    {
        this.scale = scale;
    }

    public float GetNoiseValue(int x, int y)
    {
        return Mathf.PerlinNoise(x * scale, y * scale);
    }
}

