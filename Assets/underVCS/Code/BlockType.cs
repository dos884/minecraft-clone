using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockType
{
    
    public string blockName;
    public bool isSolid;
    [Header("Atlas coords")]
    public Vector2 frontFace;
    public Vector2 rightFace;
    public Vector2 backFace;
    public Vector2 leftFace;
    public Vector2 bottomFace;
    public Vector2 topFace;

    public Vector2 GetTextureCoord(int faceIndex)
    {
        switch (faceIndex)
        {

            case 0:
                return frontFace;
            case 1:
                return rightFace;
            case 2:
                return backFace;
            case 3:
                return leftFace;
            case 4:
                return bottomFace;
            case 5:
                return topFace;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return topFace;


        }

    }
}
