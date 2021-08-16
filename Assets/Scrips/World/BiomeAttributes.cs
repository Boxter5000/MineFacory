using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "Biomes/Biome Attribute")]
public class BiomeAttributes : ScriptableObject {

    public string biomeName;

    public int solidGroundHeight;
    public int terrainHeight;
    public float terrainScale;
    public float redistribution;
    public int octaves;
    public float persistance;
    public float lacunarity;

    public Lode[] lodes;
    
    //Trees

    public float treeareaThreshold;
    public float treeareaScale;

    public float treePlacementThreshold;
    public float treePlacemetScale;

}

[System.Serializable]
public class Lode {

    public string nodeName;
    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;


}
