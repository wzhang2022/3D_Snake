using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// NOTE: this agent is still greedy in the sense that wants to go to the nearest beneficial position,
// but it uses search within that framework to evaluate costs and get to the local goal quickest
public class SearchAgent : Agent
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

    // identify all potential goals
    HashSet<Vector3> FindGoals()
    {
        // food and powerups are goals
        HashSet<Vector3> goals = new HashSet<Vector3>(m.foodPositions);
        goals.UnionWith(m.powerUpPositions);
        // if currently powered up, so is the other player's body
        if (this.powerTurns > 1)
        {
            goals.UnionWith(opponent.positions);
        }
        return goals;
    }

    // returns a Vector3 of direction that agent wants to move next
    public override Vector3 DecideMove(Agent otherplayer)
    {
        // save reference to opponent
        opponent = otherplayer;

        // identify goals
        Vector3 head = this.head.transform.position;
        HashSet<Vector3> goals = FindGoals();
        // filter out valid moves
        Vector3[] moves = FindSafeMoves();
        // select move on shortest path to goal
        Vector3 bestMove = moves[0];
        float bestDist = this.SearchDist(head + bestMove, goals);
        foreach (Vector3 move in moves)
        {
            float dist = this.SearchDist(head + move, goals);
            if (dist <= bestDist)
            {
                bestMove = move;
                bestDist = dist;
            }
        }
        return bestMove;
    }

    // return length of shortest open path from a start point to any goal (using A* search)
    private float SearchDist(Vector3 start, HashSet<Vector3> goals)
    {
        // if there are no goals, return arbitrary value
        if (goals.Count() == 0)
        {
            return 0;
        }
        // use manhattan distance to nearest goal as a heuristic
        Vector3 closest = goals.First();
        foreach (Vector3 goal in goals)
        {
            if (this.MDist(closest, start) > this.MDist(goal, start))
            {
                closest = goal;
            }
        }
        float heuristic = this.MDist(start, closest);
        // TODO!
        return heuristic;
    }
}
