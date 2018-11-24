using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflexAgent : PlayerController
{
    public override void Update()
    {
        return;
    }

    public override void DecideMove(
        PlayerController otherplayer,
        HashSet<Vector3> wallPositions,
        HashSet<Vector3> foodPositions,
        HashSet<Vector3> powerUpPositions)
    {
        Vector3 pos = base.head.transform.position;
        Vector3[] moves = new[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        foreach (Vector3 move in moves)
        {
            if (!wallPositions.Contains(pos + move))
            {
                base.MoveCommand(move);
            }
        }
    }
}
