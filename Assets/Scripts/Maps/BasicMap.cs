﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for creating a rectangular map
public class BasicMap : Map {
    // size parameters
    public int minX = -10;
    public int maxX = 10;
    public int minZ = -10;
    public int maxZ = 10;
    // wall cube prefab
    public GameObject wallPrefab;

    // Use this for initialization
    protected override void Start() {
        // set up all the walls
        Transform LeftWall = Instantiate(wallPrefab, this.gameObject.transform, true).transform;
        Transform RightWall = Instantiate(wallPrefab, this.gameObject.transform, true).transform;
        Transform FrontWall = Instantiate(wallPrefab, this.gameObject.transform, true).transform;
        Transform BackWall = Instantiate(wallPrefab, this.gameObject.transform, true).transform;
        minX -= 1;
        maxX += 1;
        minZ -= 1;
        maxZ += 1;

        LeftWall.localPosition = new Vector3(minX, 0, (maxZ + minZ) / 2);
        LeftWall.localRotation = Quaternion.Euler(0, 0, 0);
        LeftWall.localScale = new Vector3(1, 3, maxZ - minZ);

        RightWall.localPosition = new Vector3(maxX, 0, (maxZ + minZ) / 2);
        RightWall.localRotation = Quaternion.Euler(0, 0, 0);
        RightWall.localScale = new Vector3(1, 3, maxZ - minZ);

        FrontWall.localPosition = new Vector3((maxX + minX) / 2, 0, minZ);
        FrontWall.localRotation = Quaternion.Euler(0, 0, 0);
        FrontWall.localScale = new Vector3(maxX - minX, 3, 1);

        BackWall.localPosition = new Vector3((maxX + minX) / 2, 0, maxZ);
        BackWall.localRotation = Quaternion.Euler(0, 0, 0);
        BackWall.localScale = new Vector3(maxX - minX, 3, 1);
    }

    public override HashSet<Vector3> GetWallPositions()
    {
        HashSet<Vector3> wallPositions = new HashSet<Vector3>();
        for (int x = minX; x <= maxX; x++)
        {
            wallPositions.Add(new Vector3(x, 0, maxZ));
            wallPositions.Add(new Vector3(x, 1, maxZ));
            wallPositions.Add(new Vector3(x, 0, minZ));
            wallPositions.Add(new Vector3(x, 1, minZ));
        }
        for (int z = minZ; z <= maxZ; z++)
        {
            wallPositions.Add(new Vector3(minX, 0, z));
            wallPositions.Add(new Vector3(minX, 1, z));
            wallPositions.Add(new Vector3(maxX, 0, z));
            wallPositions.Add(new Vector3(maxX, 1, z));
        }
        return wallPositions;
    }

    public override Vector3 GetRandomPosition()
    {
        int x = Random.Range(minX+1, maxX);
        int y = Random.Range(0, 2);
        int z = Random.Range(minZ + 1, maxZ);
        return new Vector3(x, y, z);
    }


    // Update is called once per frame
}
