using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class ChunkGenerator
{
	public ChunkCoord cords;
	
	GameObject chunkObjekt;
	MeshRenderer meshRenderer;
	MeshFilter meshFilter;
	private MeshCollider _meshCollider;

	int vertexIndex = 0;
	List<Vector3> vertices = new List<Vector3> ();
	List<int> triangles = new List<int> ();
	List<Vector2> uvs = new List<Vector2> ();

	public byte[,,] voxelMap = new byte[VertexTable.ChunkWidth, VertexTable.ChunkHeight, VertexTable.ChunkWidth];
	private World world;

	
	public ChunkGenerator(ChunkCoord _cords, World _world)
	{
		world = _world;
		cords = _cords;
		chunkObjekt = new GameObject();
		meshFilter = chunkObjekt.AddComponent<MeshFilter>();
		meshRenderer = chunkObjekt.AddComponent<MeshRenderer>();
		_meshCollider = chunkObjekt.AddComponent<MeshCollider>();

		meshRenderer.material = world.material;
		chunkObjekt.transform.SetParent(world.transform);
		chunkObjekt.transform.position =
			new Vector3(cords.x * VertexTable.ChunkWidth, 0f, cords.z * VertexTable.ChunkWidth);
		chunkObjekt.name = "chunk: " + cords.x + " , " + cords.z;
		
		PopulateVoxelMap ();
		CreateMeshData ();
		CreateMesh ();
	}

	void PopulateVoxelMap () {

		for (int y = 0; y < VertexTable.ChunkHeight; y++) {
			for (int x = 0; x < VertexTable.ChunkWidth; x++) {
				for (int z = 0; z < VertexTable.ChunkWidth; z++)
				{
					voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);

				}
			}
		}
	}

	void CreateMeshData () {

		for (int y = 0; y < VertexTable.ChunkHeight; y++) {
			for (int x = 0; x < VertexTable.ChunkWidth; x++) {
				for (int z = 0; z < VertexTable.ChunkWidth; z++) {

					AddVoxelDataToChunk (new Vector3(x, y, z));

				}
			}
		}
	}

	public bool isActive
	{
		get { return chunkObjekt.activeSelf; }
		set{chunkObjekt.SetActive(value);}
	}

	public Vector3 position
	{
		get { return chunkObjekt.transform.position; }
	}
	
	bool isVoxelInChunk(int x, int y, int z)
	{
		if (x < 0 || x > VertexTable.ChunkWidth - 1 || y < 0 || y > VertexTable.ChunkHeight - 1 || z < 0 ||
		    z > VertexTable.ChunkWidth - 1)
			return false;
		else
			return true;
	}
	
	bool CheckVoxel (Vector3 pos) {

		int x = Mathf.FloorToInt (pos.x);
		int y = Mathf.FloorToInt (pos.y);
		int z = Mathf.FloorToInt (pos.z);

		if (!isVoxelInChunk(x, y, z))
			return world.blocktypes[world.GetVoxel(pos + position)].isSolid;

		return world.blocktypes[voxelMap[x, y, z]].isSolid;

	}

	void AddVoxelDataToChunk (Vector3 pos) {
	// Render order: Front, Back, Top, Bottum, Left, Right
		
	if(!CheckVoxel(pos)) return;
		for (int p = 0; p < 6; p++) { 

			if (!CheckVoxel(pos + VertexTable.faceChecks[p]))
			{

				byte blockID = voxelMap[(int) pos.x, (int) pos.y, (int) pos.z];
				
				vertices.Add (pos + VertexTable.voxelVerts [VertexTable.voxelTris [p, 0]]);
				vertices.Add (pos + VertexTable.voxelVerts [VertexTable.voxelTris [p, 1]]);
				vertices.Add (pos + VertexTable.voxelVerts [VertexTable.voxelTris [p, 2]]);
				vertices.Add (pos + VertexTable.voxelVerts [VertexTable.voxelTris [p, 3]]);
				
				AddTextures(world.blocktypes[blockID].GetTexturID(p));
				
				triangles.Add (vertexIndex);
				triangles.Add (vertexIndex + 1);
				triangles.Add (vertexIndex + 2);
				triangles.Add (vertexIndex + 2);
				triangles.Add (vertexIndex + 1);
				triangles.Add (vertexIndex + 3);
				vertexIndex += 4;

			}
		}
	}

	void AddTextures(int textureID)
	{
		Vector2 UVCords = TextureMapping.GetUVPos(textureID);
		uvs.Add (UVCords + TextureMapping.VoxelTextureUV[0]);
		uvs.Add (UVCords + TextureMapping.VoxelTextureUV[1]);
		uvs.Add (UVCords + TextureMapping.VoxelTextureUV[2]);
		uvs.Add (UVCords + TextureMapping.VoxelTextureUV[3]);
	}

	void CreateMesh()
	{
	
		Mesh mesh = new Mesh();
		
		mesh.indexFormat = IndexFormat.UInt32;
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;
		_meshCollider.sharedMesh = mesh;
	}
	
}
