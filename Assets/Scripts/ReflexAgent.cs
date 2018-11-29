using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflexAgent : Agent
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

    public void Shuffle(Vector3[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int rnd = Random.Range(0, array.Length);
            Vector3 tempGO = array[rnd];
            array[rnd] = array[i];
            array[i] = tempGO;
        }
    }

    public override Vector3 DecideMove(Agent otherplayer)
    {
        // save reference to opponent
        opponent = otherplayer;

        // find random move to open space
        Vector3 pos = base.head.transform.position;
        Vector3[] moves = new[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        Shuffle(moves);
        foreach (Vector3 move in moves)
        {
            if (move != -1 * base.direction_prev && move != base.direction_prev && IsOpen(pos+move))
            {
                return move;
            }
        }
        return moves[0];
    }
}
