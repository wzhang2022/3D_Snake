using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflexAgent : PlayerController
{
    public override void Update()
    {
        return;
    }

    public override void DecideMove()
    {
        base.MoveCommand(Vector3.left);
    }
}
