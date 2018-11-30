using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// NOTE: this agent is greedy both in goal selection (wants to go to the nearest beneficial position),
// and in it's method of evaluating and navigating to positions (decreasing manhattan distance)
public class GreedyAgent : Agent
{
    public MatchManager m;
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
        return !this.positions.Contains(pos) && // self
               !m.wallPositions.Contains(pos) && // walls
               (0 <= (pos).y) && (pos).y <= 1; // within layer boundaries
    }

    bool IsSafe(Vector3 pos)
    {
        return this.powerTurns > 1 ||
               (!opponent.positions.Contains(pos) && // other player
               pos != opponent.NextMove()); // head collisions
    }

    // filter to create list of valid moves
    Vector3[] FindSafeMoves()
    {
        Vector3 head = this.head.transform.position;
        Vector3[] moves = new[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        moves = moves.Where(move =>
                IsOpen(head + move) && 
                IsSafe(head + move) &&
                this.direction_prev != -move).ToArray<Vector3>();
        if (moves.Count() == 0)
        {
            Debug.Log("No valid moves");
            return new[] { Vector3.left };
        }
        return moves;
    }

    // identify closest goal as target
    Vector3 FindTarget()
    {
        Vector3 target = new Vector3(0, 0, 0);
        Vector3 head = this.head.transform.position;
        // food and powerups are goals
        HashSet<Vector3> goals = new HashSet<Vector3>(m.foodPositions);
        goals.UnionWith(m.powerUpPositions);
        // if currently powered up, so is the other player's body
        if (this.powerTurns > 1)
        {
            goals.UnionWith(opponent.positions);
        }
        foreach (Vector3 goal in goals)
        {
            if (target == Vector3.zero || this.MDist(target, head) > this.MDist(goal, head))
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
        // filter out invalid and unsafe moves
        Vector3[] moves = FindSafeMoves();
        // select move on path to target
        Vector3 bestMove = moves[0];
        float bestDist = this.MDist(head + bestMove, target);
        foreach (Vector3 move in moves)
        {
            float dist = this.MDist(head + move, target);
            if (dist <= bestDist)
            {
                bestMove = move;
                bestDist = dist;
            }
        }
        return bestMove;
    }
}
