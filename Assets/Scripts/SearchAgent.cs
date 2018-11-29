using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SearchAgent : Agent
{
    private MatchManager m;
    private Agent opponent;

    public void Start()
    {
        // obtain reference to match manager script to access game state
        GameObject managerObject = GameObject.Find("MatchManager");
        m = managerObject.GetComponent<MatchManager>();
        // Debug.Log(m.ToString());
    }

    bool IsOpen(Vector3 pos)
    {
        return !opponent.positions.Contains(pos) && // other player
               pos != opponent.NextMove() && // head collisions
               !this.positions.Contains(pos) && // self
               !m.wallPositions.Contains(pos) && // walls
               (0 <= (pos).y) && (pos).y <= 1; // within layer boundaries
    }

    // filter to create list of valid moves
    Vector3[] FindValidMoves()
    {
        Vector3 head = this.head.transform.position;
        Vector3[] moves = new[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        moves = moves.Where(move =>
                IsOpen(head + move) &&
                this.direction_prev != -move).ToArray<Vector3>();
        return moves;
    }

    // identify closest goal as target
    Vector3 FindTarget()
    {
        Vector3 target = new Vector3(0, 0, 0);
        Vector3 head = this.head.transform.position;
        HashSet<Vector3> goals = new HashSet<Vector3>(m.foodPositions);
        goals.UnionWith(m.powerUpPositions);
        foreach (Vector3 goal in goals)
        {
            if (target == Vector3.zero || SearchDist(target, head) > SearchDist(goal, head))
            {
                target = goal;
            }
        }
        return target;
    }

    // returns a Vector3 of direction that agent wants to move next
    public override Vector3 DecideMove(Agent otherplayer)
    {
        // save reference to opponent
        opponent = otherplayer;

        // identify goal
        Vector3 head = this.head.transform.position;
        Vector3 target = FindTarget();
        // filter out valid moves
        Vector3[] moves = FindValidMoves();
        // select move on path to target
        Vector3 bestMove = moves[0];
        float bestDist = SearchDist(head + bestMove, target);
        foreach (Vector3 move in moves)
        {
            float dist = SearchDist(head + move, target);
            if (dist <= bestDist)
            {
                bestMove = move;
                bestDist = dist;
            }
        }
        return bestMove;
    }

    // return length of shortest open path between two points (using A* search)
    private float SearchDist(Vector3 start, Vector3 end)
    {
        // use manhattan distance as a heuristic
        float heuristic = this.MDist(start, end);
        return heuristic;
    }
}
