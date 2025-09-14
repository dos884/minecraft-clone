using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{

    public BlockType[] blockTypes;
    public Material voxelMaterial;
    private VoxelChunk[,,] chunks = new VoxelChunk[VoxelData.worldWidthInChunks,
        VoxelData.worldHeightInChunks,
        VoxelData.worldWidthInChunks];
    public static byte[,,] voxelMap;
    // Start is called before the first frame update
    void Start()
    {
        voxelMap = TerrainGenerator.GetWorld();
        for (int x = 0; x < VoxelData.worldWidthInChunks; x++)
        {
            for (int y = 0; y < VoxelData.worldHeightInChunks; y++)
            {
                for (int z = 0; z < VoxelData.worldWidthInChunks; z++)
                {
                    chunks[x, y, z] = new VoxelChunk(new IntVector3(x, y, z), this);
                }
            }
        }
    }

    public VoxelChunk GetChunkAtPositon(IntVector3 pos)
    {
        int cX = pos.x / VoxelData.chunkSize;
        int cY = pos.y / VoxelData.chunkSize;
        int cZ = pos.z / VoxelData.chunkSize;
        return chunks[cX, cY, cZ];
    }

    public void ChangeBlock(IntVector3 bPos, byte bType)
    {
        voxelMap[bPos.x, bPos.y, bPos.z] = bType;
        GetChunkAtPositon(bPos).UpdateMesh();
    }
    public static int ClosestFromAbove(IntVector3 pos)
    {
        // todo : check x,y,z are valid
        int nearestY = pos.y + 1;
        while(nearestY<VoxelData.worldHeightInVoxels-1 && voxelMap[pos.x, nearestY, pos.z] == 0)
        {
            nearestY++;
        }
        if(voxelMap[pos.x, nearestY, pos.z] > 0)
        {
            return nearestY;
        } else
        {
            return nearestY + 1;
        }
    }
}
