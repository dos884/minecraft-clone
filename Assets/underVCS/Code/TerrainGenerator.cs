using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplexNoise;

public class TerrainGenerator
{
    public GameObject protoCube;
    private static int worldWidth = VoxelData.worldWidthInVoxels;
    private static int worldHeight = VoxelData.worldHeightInVoxels;
    private const float scale = 10f;
    private const int heightDisparity = 20, minHeight = 29;
    private static int[,] perlinHeights = new int[VoxelData.worldWidthInVoxels, VoxelData.worldWidthInVoxels];
    private static byte[,,] worldRaw = new byte[worldWidth, worldHeight, worldWidth];
    
    public static byte[,,] GetWorld()
    {
        GenerateHeightMap();
        PopulateWorld();
        return worldRaw;
    }

    private static void GenerateHeightMap()
    {
        for (int i = 0; i < worldWidth; i++)
        {
            for (int j = 0; j < worldWidth; j++)
            {
                float val = Mathf.PerlinNoise(j / scale, i / scale);
                int rounded = Mathf.RoundToInt(val * heightDisparity);
                perlinHeights[i, j] = rounded + minHeight;
                //print(rounded);
            }
        }
    }
    private static void PopulateWorld()
    {
        for (int i = 0; i < worldWidth; i++)
        {
            for (int j = 0; j < worldWidth; j++)
            {
                
                int h = perlinHeights[i, j];
                for(int k =0; k < h; k++)
                {
                    worldRaw[i, k, j] = 1;
                    // the top voxel should be grassy dirt
                    if (k == h - 1)
                    {
                        worldRaw[i, k, j] = 2;
                        // we also create a node for pathfinding
                        AiManager.pf.InitNode(new IntVector3(i, k, j));
                    }
                }
            }
        }
    }

}
