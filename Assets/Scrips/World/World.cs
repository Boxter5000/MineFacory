using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class World : MonoBehaviour
{
    public Transform player;
    public Vector3 spawn;

    public Material material;
    public BlockType[] blocktypes;

    public byte loadChunksAtOnce;

    ChunkGenerator[,] chunks = new ChunkGenerator[VertexTable.WorldSizeInChunks, VertexTable.WorldSizeInChunks];
    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    ChunkCoord playerLastChunkCoord;
    private float[,] noiseMap;
    private List<ChunkCoord> newChunks = new List<ChunkCoord>();

    [Header("Noise")]
    [SerializeField] private int minValley;
    [SerializeField] private float ampletude;
    
    [SerializeField] private int seed;
    [SerializeField] private float scale;
    [SerializeField] private int octaves;
    [SerializeField] private float persistance;
    [SerializeField] private float lacunarity;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 chunkPos;

    private void Start()
    {

        noiseMap = Noise.GenerateNoiseMap(Random.Range(0,Int32.MaxValue), scale, octaves, persistance, lacunarity, offset, chunkPos);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.transform.position);

    }

    private void Update() {

        if (!GetChunkCoordFromVector3(player.transform.position).Equals(playerLastChunkCoord))
            CheckViewDistance();
        if(newChunks.Count <= 0) return;
        for (int x = 0; x < loadChunksAtOnce; x++)
        {
            CreateChunk(newChunks[x]);
            newChunks.Remove(newChunks[x]);
        }
    }

    ChunkCoord GetChunkCoordFromVector3 (Vector3 pos) {

        int x = Mathf.FloorToInt(pos.x / VertexTable.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VertexTable.ChunkWidth);
        return new ChunkCoord(x, z);

    }

    private void GenerateWorld () {

        for (int x = VertexTable.WorldSizeInChunks / 2 - VertexTable.VewdistanceInChunks / 2; x < VertexTable.WorldSizeInChunks / 2 + VertexTable.VewdistanceInChunks / 2; x++) {
            for (int z = VertexTable.WorldSizeInChunks / 2 - VertexTable.VewdistanceInChunks / 2; z < VertexTable.WorldSizeInChunks / 2 + VertexTable.VewdistanceInChunks / 2; z++) {

                CreateChunk(new ChunkCoord(x, z));

            }
        }

        spawn = new Vector3(VertexTable.WorldSizeInBlocks / 2, minValley + noiseMap[(int)0, (int)0] * ampletude + 2, VertexTable.WorldSizeInBlocks / 2);
        player.position = spawn;

    }

    private void CheckViewDistance () {

        int chunkX = Mathf.FloorToInt(player.position.x / VertexTable.ChunkWidth);
        int chunkZ = Mathf.FloorToInt(player.position.z / VertexTable.ChunkWidth);

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = chunkX - VertexTable.VewdistanceInChunks / 2; x < chunkX + VertexTable.VewdistanceInChunks / 2; x++) {
            for (int z = chunkZ - VertexTable.VewdistanceInChunks / 2; z < chunkZ + VertexTable.VewdistanceInChunks / 2; z++) {

                // If the chunk is within the world bounds and it has not been created.
                if (IsChunkInWorld(x, z)) {

                    ChunkCoord thisChunk = new ChunkCoord(x, z);

                    if (chunks[x, z] == null)
                    {
                        if(!newChunks.Contains(thisChunk))
                            newChunks.Add(thisChunk);
                    }
                    
                    else if (!chunks[x, z].isActive) {
                        chunks[x, z].isActive = true;
                        activeChunks.Add(thisChunk);
                    }
                    // Check if this chunk was already in the active chunks list.
                    for (int i = 0; i < previouslyActiveChunks.Count; i++) {

                        //if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        if (previouslyActiveChunks[i].x == x && previouslyActiveChunks[i].z == z)
                            previouslyActiveChunks.RemoveAt(i);

                    }

                }
            }
        }

        foreach (ChunkCoord coord in previouslyActiveChunks)
        {
            chunks[coord.x, coord.z].isActive = false;
            newChunks.Remove(coord);
            
        }


    }
    public bool CheckForVoxel (float _x, float _y, float _z) {

        int xCheck = Mathf.FloorToInt(_x);
        int yCheck = Mathf.FloorToInt(_y);
        int zCheck = Mathf.FloorToInt(_z);

        int xChunk = xCheck / VertexTable.ChunkWidth;
        int zChunk = zCheck / VertexTable.ChunkWidth;

        xCheck -= (xChunk * VertexTable.ChunkWidth);
        zCheck -= (zChunk * VertexTable.ChunkWidth);

        return blocktypes[chunks[xChunk, zChunk].voxelMap[xCheck, yCheck, zCheck]].isSolid;

    }
    
    bool IsChunkInWorld(int x, int z) {

        if (x > 0 && x < VertexTable.WorldSizeInChunks - 1 && z > 0 && z < VertexTable.WorldSizeInChunks - 1)
            return true;
        else
            return false;

    }
    

    private void CreateChunk (ChunkCoord coord) {

        chunks[coord.x, coord.z] = new ChunkGenerator(new ChunkCoord(coord.x, coord.z), this);
        activeChunks.Add(new ChunkCoord(coord.x, coord.z));
    }

    public byte GetVoxel (Vector3 pos)
    {
        byte currentBlockID = 0;
        
        if (pos.x < 0 || pos.x > VertexTable.WorldSizeInBlocks - 1 || pos.y < 0 || pos.y > VertexTable.ChunkHeight - 1 || pos.z < 0 || pos.z > VertexTable.WorldSizeInBlocks - 1)
            currentBlockID = 0;
        if(pos.y == Mathf.Floor(minValley + noiseMap[(int)pos.x, (int)pos.z] * ampletude))
            currentBlockID = 1;
					
        if (pos.y < Mathf.Floor(minValley + noiseMap[(int)pos.x, (int)pos.z] * ampletude))
            currentBlockID = 2;
					
        if (pos.y < Mathf.Floor(minValley + noiseMap[(int)pos.x, (int)pos.z] * ampletude - 2))
            currentBlockID = 3;
					
        if(pos.y == 0)
            currentBlockID = 4;
        
        
        return currentBlockID;

    }

}

public struct ChunkCoord {

    public int x;
    public int z;

    public ChunkCoord (int _x, int _z) {

        x = _x;
        z = _z;

    }

    public bool Equals(ChunkCoord other)
    {
        return other.x == x && other.z == z;
    }
}

[System.Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;

    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottumFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    public int GetTexturID(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottumFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.LogWarning("Error! invalide Texture face Index");
                return 0;
        }
    }
    
}
