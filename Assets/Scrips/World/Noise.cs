using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise  {

    public static float Get2DPerlin (Vector2 position, float offset, float scale, int octaves, float persistance, float lacunarity, float redistribution)
    {
        position.x += (offset + VoxelData.seed + 0.1f);
        position.y += (offset + VoxelData.seed + 0.1f);
        
        if (scale <= 0)
        {
            scale = 0.00001f;
        }

        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;
        
        float elevation;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = position.x / scale * frequency;
            float sampleY = position.y / scale * frequency;

            float e = Mathf.PerlinNoise(sampleX, sampleY);
            noiseHeight += e * amplitude;

            amplitude *= persistance;
            frequency *= lacunarity;
        }
        elevation = noiseHeight;

        return Mathf.Pow(elevation, redistribution);
    }
    
    public static float GetStructurPerlin (Vector2 position, float offset, float scale) {
        
        return Mathf.PerlinNoise(position.x / VoxelData.ChunkWidth * scale, position.y / VoxelData.ChunkWidth * scale);

    }

    public static bool Get3DPerlin (Vector3 position, float offset, float scale, float threshold) {

        // https://www.youtube.com/watch?v=Aga0TBJkchM Carpilot on YouTube

        float x = (position.x + offset + VoxelData.seed + 0.1f) * scale;
        float y = (position.y + offset + VoxelData.seed + 0.1f) * scale;
        float z = (position.z + offset + VoxelData.seed + 0.1f) * scale;

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