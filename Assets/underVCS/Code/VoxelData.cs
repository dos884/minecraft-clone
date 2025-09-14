using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
    public static readonly int chunkSize = 16;
    public static readonly int worldWidthInChunks = 5, worldHeightInChunks = 4;
    public static int worldWidthInVoxels
    {
        get { return worldWidthInChunks * chunkSize; }
    }
    public static int worldHeightInVoxels
    {
        get { return worldHeightInChunks * chunkSize; }
    }
    public static readonly int atlasSize = 4;
    public static readonly Vector3[] verts =
    {
        new Vector3(0,0,0),
        new Vector3(0,0,1),
        new Vector3(0,1,0),
        new Vector3(0,1,1),
        new Vector3(1,0,0),
        new Vector3(1,0,1),
        new Vector3(1,1,0),
        new Vector3(1,1,1),
    };
    public static readonly int[,] tris = new int[6, 6]
    {
        // Lets assume the ZY plane is the back face
        {5, 4, 7, 7, 4, 6}, // front face
        {1, 5, 3, 3, 5, 7}, // right face
        {0, 1, 2, 2, 1, 3}, // back face
        {4, 0, 6, 6, 0, 2}, // left face
        {5, 1, 4, 4, 1, 0}, // bottom face
        {7, 6, 3, 3, 6, 2} // top face

    };

    public static readonly Vector2[] uvs = new Vector2[6]
    {
        new Vector2(1,0),
        new Vector2(0,0),
        new Vector2(1,1),
        new Vector2(1,1),
        new Vector2(0,0),
        new Vector2(0,1)
    };
}
