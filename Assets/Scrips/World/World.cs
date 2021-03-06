using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using System.IO;


    public class World : MonoBehaviour
    {

        public Settings settings;
        
        [Header("World Generation Values")]
        public BiomeAttributes[] biomes;
        public BlockType[] blocktypes;
        public Material material;
        public Material transparentMaterial;

        [Header("Shader Settings")] 
        [Range(.75f, 0f)]
        public float globalLightLevel;
        public Color day;
        public Color night;

        [Header("Player")]
        public Transform player;
        public Vector3 spawnPosition;
        
        
        private readonly Chunk[,] _chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
        public List<ChunkCoord> _activeChunks = new List<ChunkCoord>();
        public ChunkCoord _playerChunkCoord;
        private ChunkCoord _playerLastChunkCoord;

        private bool _isCreatingChunks;

        [HideInInspector]public readonly Queue<Chunk> ChunksToDraw = new Queue<Chunk>();
        private readonly Queue<Queue<VoxelMod>>  _modifications = new Queue<Queue<VoxelMod>>();
        [HideInInspector]private List<Chunk> chunksToUpdate = new List<Chunk>();
        private bool _applyingModifications;

        public bool isInventoryOpen;
        public GameObject debugScreen;
        public Inventory inventoryScreen;

        [HideInInspector]private Thread ChunkUpdateThread;
        public object ChunkUpdateThreadLock = new object();
        public object ChunkListThreadLock = new object();

        private static World _instance;
        public static World Instance
        {
            get { return _instance; }
        }

        public WorldData worldData;

        public string appPath;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            appPath = Application.persistentDataPath;
        }

        private void Start()
        {
            Debug.Log("Generating new World Using Seed:" + VoxelData.seed);

            worldData = SaveSystem.LoadWorld("Prototyp");
            
            //string jsonExport = JsonUtility.ToJson(settings);
            //Debug.Log(jsonExport);
            //File.WriteAllText(Application.dataPath + "/settings.cfg", jsonExport);

            string jsonImport = File.ReadAllText(Application.dataPath + "/settings.cfg");
            settings = JsonUtility.FromJson<Settings>(jsonImport);
            
            
            Random.InitState(VoxelData.seed);
            
            Shader.SetGlobalFloat("minGlobalLightLevel", VoxelData.minLightLevel);
            Shader.SetGlobalFloat("maxGlobalLightLevel", VoxelData.maxLightLevel);
            OpenCloseInventoryUI();
            
            LoadWorld();

            SetGlobalLightValue();
            spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight -20, (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f);
            player.position = spawnPosition;
            CheckViewDistance();
            _playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
            
            if (settings.enableThreading) {
                ChunkUpdateThread = new Thread(new ThreadStart(ThreadedUpdate));
                ChunkUpdateThread.Start();
            }
        }

        public void SetGlobalLightValue()
        {
            Shader.SetGlobalFloat("GlobalLightLevel", globalLightLevel);
            Camera.main.backgroundColor = Color.Lerp(night, day, globalLightLevel);
        }

        private void Update()
        {

            ChunkCoord ok = GetChunkCoordFromVector3(player.position);
            if(ok != null)
                _playerChunkCoord = ok;
            

            // Only update the chunks if the player has moved from the chunk they were previously on.
            
            if (!_playerLastChunkCoord.Equals(_playerChunkCoord))
                CheckViewDistance();

            if (ChunksToDraw.Count > 0)
            {
                ChunksToDraw.Dequeue().CreateMesh();
            }

            if (!settings.enableThreading) {

                if (!_applyingModifications)
                    ApplyModifications();

                if (chunksToUpdate.Count > 0)
                    UpdateChunks();

            }

            if (Input.GetKeyDown(KeyCode.F3))
                debugScreen.SetActive(!debugScreen.activeSelf);
            
            if (Input.GetKeyDown(KeyCode.F1))
                SaveSystem.SaveWorld(worldData);
        }

        void LoadWorld () {

            for (int x = (VoxelData.WorldSizeInChunks / 2) - settings.loadDistance; x < (VoxelData.WorldSizeInChunks / 2) + settings.loadDistance; x++) {
                for (int z = (VoxelData.WorldSizeInChunks / 2) - settings.loadDistance; z < (VoxelData.WorldSizeInChunks / 2) + settings.loadDistance; z++)
                {
                    worldData.LoadChunk(new Vector2Int(x, z));
                }
            }
        }

        public void AddChunkToUpdate(Chunk chunk)
        {
            AddChunkToUpdate(chunk, false);
        }
        public void AddChunkToUpdate(Chunk chunk, bool insert)
        {
            lock (ChunkUpdateThreadLock)
            {
                if (!chunksToUpdate.Contains(chunk))
                {
                    if (insert)
                    {
                        chunksToUpdate.Insert(0,chunk);
                    }
                    else
                    {
                        chunksToUpdate.Add(chunk);
                    }
                }
            }
        }
    
        void UpdateChunks ()
        {

            lock (ChunkUpdateThreadLock)
            {
                chunksToUpdate[0].UpdateChunk();
                if (!_activeChunks.Contains(chunksToUpdate[0].coord))
                {
                    _activeChunks.Add(chunksToUpdate[0].coord);
                }
                chunksToUpdate.RemoveAt(0);
            }
        }

        void ThreadedUpdate()
        {

            while (true)
            {

                if (!_applyingModifications)
                {
                    ApplyModifications();
                }

                if (chunksToUpdate.Count > 0)
                {
                    UpdateChunks();
                }

            }

        }

        private void OnDisable() {

            if (settings.enableThreading) {
                ChunkUpdateThread.Abort();
            }

        }

        void ApplyModifications () {

            _applyingModifications = true;

            while (_modifications.Count > 0) {

                Queue<VoxelMod> queue = _modifications.Dequeue();

                while (queue.Count > 0) {

                    VoxelMod v = queue.Dequeue();
                    
                    worldData.SetVoxel(v.Position, v.ID);
                }
            }

            _applyingModifications = false;

        }

        ChunkCoord GetChunkCoordFromVector3 (Vector3 pos) {

            int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
            int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
            return new ChunkCoord(x, z);

        }

        public Chunk GetChunkFromVector3 (Vector3 pos) {

            int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
            int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
            return _chunks[x, z];

        }

void CheckViewDistance () {

        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        _playerLastChunkCoord = _playerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(_activeChunks);

        _activeChunks.Clear();

        // Loop through all chunks currently within view distance of the player.
        for (int x = coord.x - settings.renderDistance; x < coord.x + settings.renderDistance; x++) {
            for (int z = coord.z - settings.renderDistance; z < coord.z + settings.renderDistance; z++)
            {

                ChunkCoord thisChunkCoord = new ChunkCoord(x, z); 
                // If the current chunk is in the world...
                if (IsChunkInWorld (new ChunkCoord (x, z))) {

                    // Check if it active, if not, activate it.
                    if (_chunks[x, z] == null) {
                        _chunks[x, z] = new Chunk(thisChunkCoord);
                    }
                    _chunks[x, z].isActive = true;
                    
                    _activeChunks.Add(thisChunkCoord);
                }

                // Check through previously active chunks to see if this chunk is there. If it is, remove it from the list.
                for (int i = 0; i < previouslyActiveChunks.Count; i++) {

                    if (previouslyActiveChunks[i].Equals(thisChunkCoord))
                    {
                        previouslyActiveChunks.RemoveAt(i);
                    }
                       
                }

            }
        }

        // Any chunks left in the previousActiveChunks list are no longer in the player's view distance, so loop through and disable them.
        foreach (ChunkCoord c in previouslyActiveChunks)
            _chunks[c.x, c.z].isActive = false;

    }

        public bool CheckForVoxel (Vector3 pos)
        {
            VoxelState voxel = worldData.GetVoxel(pos);
            if (blocktypes[voxel.id].isSolid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    
        public VoxelState GetVoxelState (Vector3 pos)
        {
            return worldData.GetVoxel(pos);
        }



        public byte GetVoxel (Vector3 pos) {

            int yPos = Mathf.FloorToInt(pos.y);

            /* IMMUTABLE PASS */

            // If outside world, return air.
            if (!IsVoxelInWorld(pos))
                return 0;

            // If bottom block of chunk, return bedrock.
            if (yPos == 0)
                return 6;
        
        
            /* biome Selekt PASS */

            const int solidGroundHeight = 60;
            var sumOfHeights = 0f;
            var count = 0;
            var strongestWeight = 0f;
            var strongestBiomeIndex = 0;

            for (int i = 0; i < biomes.Length; i++) {

                float weight = Noise.GetStructurPerlin(new Vector2(pos.x, pos.z), biomes[i].offset, .07f);

                // Keep track of which weight is strongest.
                if (weight >= strongestWeight) {
                    strongestWeight = weight;
                    strongestBiomeIndex = i;

                }   
                // Get the height of the terrain (for the current biome) and multiply it by its weight.
                float height = biomes[i].terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, 
                    biomes[i].terrainScale,biomes[i].octaves,biomes[i].persistance ,biomes[i].lacunarity , biomes[i].redistribution) * weight;
            
            

                // If the height value is greater 0 add it to the sum of heights.
                if (height > 0.0f) {

                    sumOfHeights += height;
                    count++;

                }

            }

            // Set biome to the one with the strongest weight.
            var biome = biomes[strongestBiomeIndex];

            // Get the average of the heights.
            sumOfHeights /= count;


            //sumOfHeights /= count;

            int terrainHeight = Mathf.FloorToInt(sumOfHeights + solidGroundHeight);

        
            /* BASIC TERRAIN PASS */
        
        

            byte voxelValue;

            if (yPos == terrainHeight)
                voxelValue = biome.surfaceBlock;
            else if (yPos < terrainHeight && yPos > terrainHeight - 4)
                voxelValue = biome.subSurfaceBlock;
            else if (yPos > terrainHeight)
                voxelValue = 0;
            else
            {
                voxelValue = 1;
            }
        
            /* Cave Pass */

            /*if (Noise.Get3DPerlin(pos, 1234, .1f, .5f))
            {
                voxelValue = 0;
            }*/

            /* SECOND PASS */

            foreach (Lode lode in biome.lodes) { 
                
                if (lode.changeBlockID.Contains(voxelValue)) {

                    if (yPos > lode.minHeight && yPos < lode.maxHeight)
                        if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                            voxelValue = lode.blockID;

                }

            }
        
            /* Flora PASS */

            if (yPos == terrainHeight) {

                if (Noise.GetStructurPerlin(new Vector2(pos.x, pos.z), 0, biome.treeZoneScale) > biome.treeZoneThreshold)
                {
                    if (Noise.GetStructurPerlin(new Vector2(pos.x, pos.z), 0, biome.treePlacementScale) > biome.treePlacementThreshold)
                    {
                        _modifications.Enqueue(Structure.MakeBasicFlora(pos, biome.minTreeHeight, biome.maxTreeHeight, biome.treeRaduius));
                    }
                }

            }

            return voxelValue;


        }

        bool IsChunkInWorld (ChunkCoord coord) {

            if (coord.x > 0 && coord.x < VoxelData.WorldSizeInChunks - 1 && coord.z > 0 && coord.z < VoxelData.WorldSizeInChunks - 1)
                return true;
            else
                return
                    false;

        }

        bool IsVoxelInWorld (Vector3 pos) {

            if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
                return true;
            else
                return false;

        }

        public void OpenInventory(Item[,] playerInventory, Item[,] externalInventory)
        {
            inventoryScreen.SetItemSlots(playerInventory, externalInventory);
        }

        public void OpenCloseInventoryUI()
        {
            if (!isInventoryOpen)
            {
                inventoryScreen.CloseInventory();
            }
            inventoryScreen.gameObject.SetActive(isInventoryOpen);
        }

    }

    [System.Serializable]
    public class Settings
    {
        [Header("Game Data")]
        public string releaseVersion = "0.0.01";

        [Header("Performance")] 
        public int loadDistance = 10;
        public int renderDistance = 5;
        public bool enableThreading = true;

        [Header("Player")]
        [Range(0.1f, 10f)]
        public float cameraSesitivity = 2.0f;
    }

    [System.Serializable]
    public class BlockType {

        public string blockName;
        public bool isSolid;
        public bool renderNeighborFaces;
        public byte opacity;
        public Sprite icon;


        [Header("Texture Values")]
        public int backFaceTexture;
        public int frontFaceTexture;
        public int topFaceTexture;
        public int bottomFaceTexture;
        public int leftFaceTexture;
        public int rightFaceTexture;

        // Back, Front, Top, Bottom, Left, Right

        public int GetTextureID (int faceIndex) {

            switch (faceIndex) {

                case 0:
                    return backFaceTexture;
                case 1:
                    return frontFaceTexture;
                case 2:
                    return topFaceTexture;
                case 3:
                    return bottomFaceTexture;
                case 4:
                    return leftFaceTexture;
                case 5:
                    return rightFaceTexture;
                default:
                    Debug.Log("Error in GetTextureID; invalid face index");
                    return 0;


            }

        }

    }
    public class VoxelMod {

        public Vector3 Position;
        public readonly byte ID;

        public VoxelMod () {

            Position = new Vector3();
            ID = 0;

        }

        public VoxelMod (Vector3 position, byte id) {

            Position = position;
            this.ID = id;

        }

    }