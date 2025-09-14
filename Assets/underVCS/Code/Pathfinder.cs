using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{
    // The dictionary maps (x,z) -> (y -> Node) 
    Dictionary<(int, int), Dictionary<int, PathNode>> nodeMap = new Dictionary<(int, int), Dictionary<int, PathNode>>();
    public const bool debug = false;
    
    public Pathfinder()
    {
        for(int x = 0; x < VoxelData.worldWidthInVoxels; x++)
        {
            for(int z = 0; z < VoxelData.worldWidthInVoxels; z++)
            {
                nodeMap.Add((x, z), new Dictionary<int, PathNode>());
            }
        }
    }

    public void DebugUpdate()
    {
        if (debug)
        {
            foreach (var kvp in nodeMap)
            {
                foreach (var kvp2 in kvp.Value)
                {
                    PathNode p = kvp2.Value;
                    foreach(var n in p.neigboors)
                    {
                        Debug.DrawLine(p.pos.ToVec3() + new Vector3(0.5f,1f,0.5f), n.pos.ToVec3() + new Vector3(0.5f,1f,0.5f), Color.red, 0.1f);
                    }
                }
            }
        }
    }
    private PathNode CreateAndConnect(IntVector3 pos)
    {
        PathNode node = new PathNode(pos);
        node.heightAbove = VoxelData.worldHeightInVoxels - pos.y;
        for (int x = pos.x - 1; x <= pos.x + 1; x++)
        {
            for (int z = pos.z - 1; z <= pos.z + 1; z++)
            {
                if (nodeMap.ContainsKey((x, z)))
                {
                    //iterate over the dictionary at (x,z)
                    Dictionary<int, PathNode> itDictXZ = nodeMap[(x, z)];
                    foreach (KeyValuePair<int, PathNode> kvp in itDictXZ)
                    {
                        node.TryConnect(kvp.Value);
                    }
                }

            }
        }
        return node;
    }
    public void InitNode(IntVector3 pos)
    {
        PathNode node = CreateAndConnect(pos);
        // add the node
        var dictXZ = nodeMap[(pos.x, pos.z)];
        dictXZ.Add(pos.y, node);
    }
    public void AddBlock(IntVector3 pos)
    {
        if (nodeMap.ContainsKey((pos.x, pos.z)))
        {
            Dictionary<int, PathNode> dictXZ = nodeMap[(pos.x, pos.z)];
            
            // figure out the closest block from above
            int minYAbove = WorldManager.ClosestFromAbove(pos);
            int[] ys = dictXZ.Keys.ToArray();
            foreach (int y in ys)
            {
                // recalculate heightAbove for all the column
                if(pos.y>y && pos.y - y < dictXZ[y].heightAbove)
                {
                    // if we covered the block below - remove it
                    if(pos.y - y < 2)
                    {
                        dictXZ[y].RemoveSelf();
                        dictXZ.Remove(y);
                        
                    } else
                    {
                        dictXZ[y].heightAbove = pos.y - y;
                    }
                    
                }
            }
            // if newly added block isnt covered
            if (minYAbove - pos.y > 2)
            {
                PathNode node = CreateAndConnect(pos);
                node.heightAbove = minYAbove - pos.y;
                dictXZ.Add(pos.y, node);
            }

        }
        

    }
    public void RemoveBlock(IntVector3 pos)
    {
        Dictionary<int, PathNode> dictXZ = nodeMap[(pos.x, pos.z)];
        if (dictXZ.ContainsKey(pos.y))
        {
            dictXZ.Remove(pos.y);
        }
        int nearestY = pos.y - 1;
        // find the nearset block from below
        while (WorldManager.voxelMap[pos.x, nearestY, pos.z] == 0)
        {
            nearestY--;
        }
        // add and connect new path node
        if (!dictXZ.ContainsKey(nearestY))
        {
            PathNode n = CreateAndConnect(new IntVector3(pos.x, nearestY, pos.z));
            dictXZ.Add(nearestY, n);
        }

        dictXZ[nearestY].heightAbove = WorldManager.ClosestFromAbove(new IntVector3(pos.x, nearestY, pos.z)) - nearestY;
        
        
    }
}

public class PathNode
{
    public HashSet<PathNode> neigboors = new HashSet<PathNode>();
    public IntVector3 pos;

    // the last pass of the A* algo this node participated in
    public long aStarPass = 0;
    // was this node visited by A* : if (aStarPass < A*.currentPass) visited will be considered as false 
    public bool visited = false;
    // number of free voxels above this one
    public int heightAbove;
    // debug drawers
    public GameObject nodeSphere;
    public GameObject edgeLine;
    public PathNode(IntVector3 _pos)
    {
        pos = _pos;
        if (Pathfinder.debug)
        {
            nodeSphere = GameObject.Instantiate(Resources.Load<GameObject>("NodeSphere"));
            nodeSphere.transform.position = pos.ToVec3() + 0.525f * Vector3.one;
        }
    }
    public void RemoveSelf()
    {
        if (Pathfinder.debug)
        {
            GameObject.Destroy(nodeSphere);
        }
        foreach(PathNode node in neigboors)
        {
            node.neigboors.Remove(this);
        }
    }
    public bool TryConnect(PathNode node)
    {

        if((this.pos.x, this.pos.z)!=(node.pos.x,node.pos.z) && 
            this.pos.distanceXZ(node.pos)<2 && 
            Mathf.Abs(pos.y - node.pos.y) < 2)
        {
            neigboors.Add(node);
            node.neigboors.Add(this);   
            return true;
        } else
        {
            return false;
        }
    }
}
