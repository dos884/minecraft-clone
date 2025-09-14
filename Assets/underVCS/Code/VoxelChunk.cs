using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelChunk
{
    private GameObject chunkGO;
    private WorldManager worldManager;
    IntVector3 posInChunks;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private MeshFilter meshFilter;
    private int vertexIndex = 0;
    private List<int> tris = new List<int>();
    private List<Vector3> verts = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    //private byte[,,] voxelMap;
    
    public VoxelChunk(IntVector3 posInChunks, WorldManager world)
    {
        worldManager = world;
        this.posInChunks = posInChunks;
        chunkGO = new GameObject("Chunk: " + posInChunks.ToVec3().ToString());
        chunkGO.transform.SetParent(world.transform);
        chunkGO.transform.position = posInChunks.ToVec3() * VoxelData.chunkSize;
        meshRenderer = chunkGO.AddComponent<MeshRenderer>();
        meshRenderer.material = world.voxelMaterial;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshFilter = chunkGO.AddComponent<MeshFilter>();
        meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshCollider = chunkGO.AddComponent<MeshCollider>();
        CreateMeshData();
        PopulateMesh();
    }

    private void CreateMeshData()
    {
        // calculate position of chunk in global coords
        int globX = posInChunks.x * VoxelData.chunkSize;
        int globY = posInChunks.y * VoxelData.chunkSize;
        int globZ = posInChunks.z * VoxelData.chunkSize;
        for (int y = globY; y < globY + VoxelData.chunkSize; y++)
        {
            for (int x = globX; x < globX + VoxelData.chunkSize; x++)
            {
                for (int z = globZ; z < globZ + VoxelData.chunkSize; z++)
                {
                    if (WorldManager.voxelMap[x, y, z] > 0 && IsVoxelVisible(x, y, z))
                    {
                        // print("adding voxel at: " + (new Vector3(x, y, z)).ToString());
                        // calculate local position in chunk
                        int locX = x % VoxelData.chunkSize;
                        int locY = y % VoxelData.chunkSize;
                        int locZ = z % VoxelData.chunkSize;
                        AddVoxelToChunk(new Vector3(locX, locY, locZ), WorldManager.voxelMap[x, y, z]);
                    }
                }
            }
        }
    }
    private void ResetMeshData()
    {
        verts.Clear();
        tris.Clear();
        uvs.Clear();
        vertexIndex = 0;
    }
    private bool IsVoxelVisible(int x, int y, int z)
    {
        // check edge cases
        if (x <= 0 || x >= VoxelData.worldWidthInVoxels-1 || y <= 0 || y >= VoxelData.worldHeightInVoxels- 1 || z <= 0 || z >= VoxelData.worldWidthInVoxels- 1)
        {
            return true;
        } else if(WorldManager.voxelMap[x+1,y,z]>0 && WorldManager.voxelMap[x - 1, y, z] > 0 &&
                    WorldManager.voxelMap[x, y+1, z] > 0 && WorldManager.voxelMap[x, y-1, z] > 0 &&
                    WorldManager.voxelMap[x, y, z+1] > 0 && WorldManager.voxelMap[x, y, z-1] > 0)
        {
            return false;
        }
        return true;
    }

    private void AddVoxelToChunk(Vector3 pos, byte voxelType)
    {
        for (int faceIndex = 0; faceIndex < 6; faceIndex++)
        {
            for (int j = 0; j < 6; j++)
            {
                int triangleIndex = VoxelData.tris[faceIndex, j];
                verts.Add(VoxelData.verts[triangleIndex] + pos);
                tris.Add(vertexIndex);
                Vector2 uvShift = worldManager.blockTypes[voxelType].GetTextureCoord(faceIndex);
                uvs.Add((VoxelData.uvs[j]+uvShift)/VoxelData.atlasSize);

                vertexIndex++;
            }
        }
    }

    private void PopulateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public void UpdateMesh()
    {
        ResetMeshData();
        CreateMeshData();
        PopulateMesh();
    }
}
