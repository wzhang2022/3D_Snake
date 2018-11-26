using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        HashSet<Vector3> goals = new HashSet<Vector3>(foodPositions);
        goals.UnionWith(powerUpPositions);
        foreach (Vector3 goal in goals) {
            if (target == Vector3.zero || dist(target, head) > dist(goal, head)) {
                target = goal;
            }
        }
        Vector3[] moves = new[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        moves = moves.Where(move =>
                !otherplayer.positions.Contains(head + move) && //prevent running into other player
                !this.positions.Contains(head + move) && //prevent running into self
                !wallPositions.Contains(head + move) && //prevent running into walls
                head + move != otherplayer.NextMove() && //prevent head collisions
                (0 <= (move + head).y) && (move + head).y <= 1 && //ensure I stay within layer boundaries
                this.direction_prev != -move).ToArray<Vector3>();
        if (moves.Count() == 0) {
            Debug.Log("No valid moves");
            return Vector3.left;
        }
        Vector3 moveToTake = moves[0];
        foreach (Vector3 move in moves) {
            if (dist(head + move, target) <= dist(head + moveToTake, target)) {
                moveToTake = move;
            }
        }
        //Debug.Log("Direction is " + moveToTake.ToString() + " and target is " + target.ToString() + " and head is " + head.ToString());
        return moveToTake;
    }
    private float dist(Vector3 a, Vector3 b) {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
}
