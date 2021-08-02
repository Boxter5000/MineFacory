using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap : MonoBehaviour
{

    public static float GenerateNoiseMap2D(int seed, float scale, Vector2 position, float offset)
    {
        float noiseHeigth;
        noiseHeigth = Mathf.PerlinNoise((position.x + 0.1f) / VertexTable.ChunkWidth * scale + offset, (position.y + 0.1f) / VertexTable.ChunkWidth * scale + offset);
        noiseHeigth = noiseHeigth / (1f + 0.5f + 0.25f);
        return Mathf.Pow(noiseHeigth, 3.0f);
    }
    public static bool Get3DPerlin (Vector3 position, float offset, float scale, float threshold) {

        // https://www.youtube.com/watch?v=Aga0TBJkchM Carpilot on YouTube

        float x = (position.x + offset + 0.1f) * scale;
        float y = (position.y + offset + 0.1f) * scale;
        float z = (position.z + offset + 0.1f) * scale;

        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);
        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        if ((AB + BC + AC + BA + CB + CA) / 6f > threshold)
            return true;
        else
            return false;

    }
}
