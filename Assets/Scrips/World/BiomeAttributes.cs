using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "Biomes/Biome Attribute")]
public class BiomeAttributes : ScriptableObject {

    public string biomeName;
    
    public int terrainHeight;
    public float terrainScale;
    public float redistribution;
    public int octaves;
    public float persistance;
    public float lacunarity;

    public int offset;

    [Header("Surface blocks")] 
    public byte surfaceBlock;
    public byte subSurfaceBlock;

    [Header(("Trees"))] 
    public float treeZoneScale = 1.3f;
    [Range(0.01f, 1f)]
    public float treeZoneThreshold = 0.6f;

    public float treePlacementScale;
    [Range(0.01f, 1f)]
    public float treePlacementThreshold;

    public int maxTreeHeight = 7;
    public int minTreeHeight = 4;

    public float treeRaduius = 5;

    public Lode[] lodes;


}

[System.Serializable]
public class Lode {

    public string nodeName;
    public byte blockID;
    public byte[] changeBlockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;



}
