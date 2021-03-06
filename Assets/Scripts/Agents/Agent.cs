﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Agent : MonoBehaviour {
    // prefabs
    public GameObject head;
    public GameObject bodyPrefab;
    public Material powerUpMaterial;
    public Material headMarkerMaterial;
    public GameObject territoryPrefab;
    public MatchManager matchManager;

    // data
    public List<GameObject> body = new List<GameObject>();
    public HashSet<Vector3> positions = new HashSet<Vector3>();
    // TERRITORY NOT BEING USED RIGHT NOW
    public HashSet<Vector3> territory = new HashSet<Vector3>();
    public HashSet<GameObject> territoryBlocks = new HashSet<GameObject>();

    // state variables
    public int length = 5;
    public int powerTurns = 0;

    // variables to manage movement
    protected Vector3 direction;
    protected int layer = 0;
    protected Vector3 direction2D;
    protected Vector3 direction_prev;

    // reference to opponent
    protected Agent opponent;

    void OnEnable () {
        direction2D = direction;
        body = new List<GameObject>();
   
        // obtain reference to match manager script to access game state
        GameObject managerObject = GameObject.Find("MatchManager");
        matchManager = managerObject.GetComponent<MatchManager>();
    }
	
    // process a movement command, changes the direction and direction2D variables
    public void MoveCommand(Vector3 c)
    {
        // allow direction if it is not in same axis of previous move, or if no length
        if (length == 1 || (c != -1 * direction_prev && c != direction_prev))
        {
            direction = c;
            // also set 2D direction if this is a planar move
            if (c.y == 0)
            {
                direction2D = direction;
            }
        }
    }

    // manages agent properties before moving - to be called once before every move
    public void PrepareNextMove()
    {
        // decrement powered-up turns (show yellow marker w/ flash at end for visual warning)
        powerTurns = Mathf.Max(powerTurns - 1, 0);
        if (powerTurns > 0 && powerTurns != 2)
        {
            this.transform.Find("HeadMarker").GetComponent<MeshRenderer>().material = powerUpMaterial;
        }
        else
        {
            this.transform.Find("HeadMarker").GetComponent<MeshRenderer>().material = headMarkerMaterial;
        }

        // handle length reductions
        while (body.Count > length)
        {
            GameObject end = (GameObject)body[0];
            body.RemoveAt(0);
            positions.Remove(end.transform.position);
            Destroy(end);
        }
    }

    // return where the player will go next
    public Vector3 NextMove()
    {
        if ((direction == Vector3.up && layer == 1) ||
            (direction == Vector3.down && layer == 0))
        {
            return head.transform.position + direction2D;
        }
        return head.transform.position + direction;
    }

    // execute the actual move
    public void Move() {
        if (direction == Vector3.up)
        {
            if (layer == 0)
            {
                layer = 1;
            }
            else if (layer >= 1)
            {
                direction = direction2D;
            }
        }
        else if (direction == Vector3.down)
        {
            if (layer == 1)
            {
                layer = 0;
            }
            else if (layer <= 0)
            {
                direction = direction2D;
            }
        }
        Vector3 oldPosition = head.transform.position;
        head.transform.Translate(direction);
        direction_prev = direction;
        GameObject new_object = Instantiate(bodyPrefab, oldPosition, head.transform.rotation);
        body.Add(new_object);
    }


    // recieve movement commands at every timestep (only used for human input)
    public virtual void Update()
    {
        return;

    }

    // give valid moves that will not result in crash
   protected Vector3[] FindSafeMoves() {
        Vector3 head = this.head.transform.position;
        Vector3[] moves = new[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        moves = moves.Where(move =>
                IsOpen(head + move) &&
                IsSafe(head + move) &&
                this.direction_prev != -move).ToArray<Vector3>();
        if (moves.Count() == 0) {
            // Debug.Log("No valid moves");
            return new[] { Vector3.left };
        }
        return moves;
    }

    protected bool IsOpen(Vector3 pos) { //checks if anything is in the square
        return !this.positions.Contains(pos) && // self
               !matchManager.wallPositions.Contains(pos) && // walls
               (0 <= (pos).y) && (pos).y <= 1; // within layer boundaries
    }

    protected bool IsSafe(Vector3 pos) {
        return this.powerTurns > 1 ||
               (!opponent.positions.Contains(pos) && // other player
               pos != opponent.NextMove()); // head collisions
    }

    // helper function for manhattan distance
    protected static float MDist(Vector3 a, Vector3 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }

    // decide what move to take, returns a Vector3
    public abstract Vector3 DecideMove(Agent otherplayer);
}
