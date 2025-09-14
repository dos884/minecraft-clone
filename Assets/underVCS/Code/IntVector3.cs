

using UnityEngine;

public class IntVector3
{
    public int x, y, z;
    public IntVector3(float _x, float _y, float _z)
    {

        x = Mathf.FloorToInt(_x);
        y = Mathf.FloorToInt(_y);
        z = Mathf.FloorToInt(_z);
    }
    public IntVector3(Vector3 v)
    {
        x = Mathf.FloorToInt(v.x);
        y = Mathf.FloorToInt(v.y);
        z = Mathf.FloorToInt(v.z);
    }
    public Vector3 ToVec3()
    {
        return new Vector3(x, y, z);
    }
    public int distanceXZ(IntVector3 other)
    {
        return Mathf.Abs(x - other.x) + Mathf.Abs(z - other.z);
    }
}
