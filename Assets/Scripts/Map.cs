using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Map : MonoBehaviour {

    // Use this for initialization
    abstract protected void Start();

    public abstract HashSet<Vector3> GetWallPositions();
    // Update is called once per frame
    public abstract Vector3 GetRandomPosition();
}
