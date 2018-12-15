using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflexAgent : Agent
{
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
        Vector3[] moves = FindSafeMoves();
        Shuffle(moves);
        return moves[0];
    }
}
