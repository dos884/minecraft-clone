using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiManager : MonoBehaviour
{
    public static Pathfinder pf = new Pathfinder();
    public static AiManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        pf.DebugUpdate();
    }
}
