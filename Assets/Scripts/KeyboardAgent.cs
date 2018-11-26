using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardAgent : Agent {
    [System.Serializable]
    public struct KeyBind
    {
        public char upZ;
        public char downZ;
        public char upX;
        public char downX;
        public char upY;
        public char downY;
    }
    public KeyBind keyBind;

    public override Vector3 DecideMove(Agent otherplayer, HashSet<Vector3> wallPositions, HashSet<Vector3> foodPositions, HashSet<Vector3> powerUpPositions) {
        return this.direction2D;
    }

    public override void Update()
    {
        foreach (char c in Input.inputString)
        {
            if (c == keyBind.downX)
            {
                base.MoveCommand(Vector3.left);
            }
            else if (c == keyBind.upX)
            {
                base.MoveCommand(Vector3.right);
            }
            else if (c == keyBind.downZ)
            {
                base.MoveCommand(Vector3.back);
            }
            else if (c == keyBind.upZ)
            {
                base.MoveCommand(Vector3.forward);
            }
            else if (c == keyBind.downY && layer == 1)
            {
                base.MoveCommand(Vector3.down);
            }
            else if (c == keyBind.upY && layer == 0)
            {
                base.MoveCommand(Vector3.up);
            }
        }
    }
}
