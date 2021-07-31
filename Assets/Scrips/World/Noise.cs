using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
   public static float[,] GenerateNoiseMap(int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, Vector2 chunkPos) {
        float[,] noiseMap = new float[VertexTable.WorldSizeInBlocks,VertexTable.WorldSizeInBlocks];

        System.Random prng = new System.Random (seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next (-100000, 100000) + offset.x;
            float offsetY = prng.Next (-100000, 100000) + offset.y;
            octaveOffsets [i] = new Vector2 (offsetX, offsetY);
        }

        if (scale <= 0) {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = VertexTable.WorldSizeInBlocks / 2f;
        float halfHeight = VertexTable.WorldSizeInBlocks / 2f;


        for (int y = (int)chunkPos.y * VertexTable.WorldSizeInBlocks; y < VertexTable.WorldSizeInBlocks * (1 + chunkPos.y); y++) {
            for (int x = (int)chunkPos.x * VertexTable.WorldSizeInBlocks; x < VertexTable.WorldSizeInBlocks * (1 + chunkPos.x); x++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y-halfWidth) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap [x % VertexTable.WorldSizeInBlocks, y % VertexTable.WorldSizeInBlocks] = noiseHeight;
            }
        }

        for (int y = 0; y < VertexTable.WorldSizeInBlocks; y++) {
            for (int x = 0; x < VertexTable.WorldSizeInBlocks; x++) {
                noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
            }
        }

        return noiseMap;
    }
}
