using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyAgent : Agent
{
    private MatchManager manager;
    public void Start() {
        GameObject managerObject = GameObject.Find("MatchManager");
        manager = managerObject.GetComponent<MatchManager>();
        Debug.Log(manager.ToString());
    }

    public override Vector3 DecideMove( //returns a Vector3
        Agent otherplayer,
        HashSet<Vector3> wallPositions,
        HashSet<Vector3> foodPositions,
        HashSet<Vector3> powerUpPositions)
    {
        Vector3 target = new Vector3(0,0,0);
        Vector3 head = this.head.transform.position;
        foreach (Vector3 foodPosition in (foodPositions)) {
            if (target == Vector3.zero || dist(target, head) > dist(foodPosition, head)) {
                target = foodPosition;
            }
        }
        Vector3[] moves = new[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        Vector3 moveToTake = moves[0];
        foreach (Vector3 move in moves) {
            if (dist(head + move, target) <= dist(head + moveToTake, target) && manager.IsOpen(head + move, false)) {
                moveToTake = move;
            }
            if (dist(head + move, target) <= 0.1) {
                moveToTake = move;
                Debug.Log("next  is " +  (head + move).ToString() +  " and head is " + head.ToString() + " and direction is " + move.ToString());
                break;
            }
        }
        Debug.Log("Direction is " + moveToTake.ToString() + " and target is " + target.ToString() + " and head is " + head.ToString());
        return moveToTake;
    }
    private float dist(Vector3 a, Vector3 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
}
