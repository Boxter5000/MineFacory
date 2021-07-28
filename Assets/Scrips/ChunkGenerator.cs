using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class ChunkGenerator : MonoBehaviour
{
  public MeshRenderer meshRenderer;
	public MeshFilter meshFilter;

	int vertexIndex = 0;
	List<Vector3> vertices = new List<Vector3> ();
	List<int> triangles = new List<int> ();
	List<Vector2> uvs = new List<Vector2> ();

	bool[,,] voxelMap = new bool[VertexTable.ChunkWidth, VertexTable.ChunkHeight, VertexTable.ChunkWidth];

	[SerializeField] private float ampletude;
	[SerializeField] private float frequency;
	[SerializeField] private float minValley;

	void Start () {

		Debug.Log(-8 % 16);
		PopulateVoxelMap ();
		CreateMeshData ();
		CreateMesh ();

	}

	void PopulateVoxelMap () {
		
		for (int y = 0; y < VertexTable.ChunkHeight; y++) {
			for (int x = 0; x < VertexTable.ChunkWidth; x++) {
				for (int z = 0; z < VertexTable.ChunkWidth; z++)
				{

					voxelMap[x, y, z] = (minValley + Mathf.PerlinNoise(x * frequency, z * frequency) * ampletude) > y;

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

	bool CheckVoxel (Vector3 pos) {

		int x = Mathf.FloorToInt (pos.x);
		int y = Mathf.FloorToInt (pos.y);
		int z = Mathf.FloorToInt (pos.z);

		if (x < 0 || x > VertexTable.ChunkWidth - 1 || y < 0 || y > VertexTable.ChunkHeight - 1 || z < 0 || z > VertexTable.ChunkWidth - 1)
			return false;

		return voxelMap [x, y, z];

	}

	void AddVoxelDataToChunk (Vector3 pos) {
	// Render order: Front, Back, Top, Bottum, Left, Right
		
	if(!CheckVoxel(pos)) return;
		for (int p = 0; p < 6; p++) { 

			if (!CheckVoxel(pos + VertexTable.faceChecks[p])) {
				
				vertices.Add (pos + VertexTable.voxelVerts [VertexTable.voxelTris [p, 0]]);
				vertices.Add (pos + VertexTable.voxelVerts [VertexTable.voxelTris [p, 1]]);
				vertices.Add (pos + VertexTable.voxelVerts [VertexTable.voxelTris [p, 2]]);
				vertices.Add (pos + VertexTable.voxelVerts [VertexTable.voxelTris [p, 3]]);
				
				
				Vector2 UVCords = TextureMapping.GetUVPos(240);
				uvs.Add (UVCords + TextureMapping.VoxelTextureUV[0]);
				uvs.Add (UVCords + TextureMapping.VoxelTextureUV[1]);
				uvs.Add (UVCords + TextureMapping.VoxelTextureUV[2]);
				uvs.Add (UVCords + TextureMapping.VoxelTextureUV[3]);
				
				
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

	void CreateMesh()
	{
	
		Mesh mesh = new Mesh();
		mesh.indexFormat = IndexFormat.UInt32;
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.RecalculateNormals();

		meshFilter.mesh = mesh;
	}
	
}
