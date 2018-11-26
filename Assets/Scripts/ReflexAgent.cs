using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflexAgent : PlayerController
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

    public override void DecideMove(
        PlayerController otherplayer,
        HashSet<Vector3> wallPositions,
        HashSet<Vector3> foodPositions,
        HashSet<Vector3> powerUpPositions)
    {
        Vector3 pos = base.head.transform.position;
        Vector3[] moves = new[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        Shuffle(moves);
        foreach (Vector3 move in moves)
        {
            if (move != -1 * base.direction_prev && move != base.direction_prev &&
                !wallPositions.Contains(pos + move) &&
                !otherplayer.positions.Contains(pos + move))
            {
                base.MoveCommand(move);
                if (foodPositions.Contains(pos + move))
                {
                    break;
                }
            }
        }
    }
}
