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

    public Vector3 queuedDirection;

    public void Start()
    {
        this.direction = Vector3.forward;
        queuedDirection = this.direction;
    }

    public override Vector3 DecideMove(Agent otherplayer) {
        Vector3 move = this.direction2D;
        // auto-load queued moves for next move (to allow for tight turns)
        MoveCommand(queuedDirection);
        return move;
    }

    public void KeyCommand(Vector3 c)
    {
        // allow direction if it is not in same axis of previous move, or if no length
        if (length == 1 || (c != -1 * this.direction_prev && c != this.direction_prev))
        {
            // clear queued commands when making valid move
            queuedDirection = c;
            this.MoveCommand(c);
        }
        else
        {
            // buffer moves in same axis
            queuedDirection = c;
        }
    }

    public override void Update()
    {
        foreach (char c in Input.inputString)
        {
            if (c == keyBind.downX)
            {
                KeyCommand(Vector3.left);
            }
            else if (c == keyBind.upX)
            {
                KeyCommand(Vector3.right);
            }
            else if (c == keyBind.downZ)
            {
                KeyCommand(Vector3.back);
            }
            else if (c == keyBind.upZ)
            {
                KeyCommand(Vector3.forward);
            }
            else if (c == keyBind.downY && layer == 1)
            {
                KeyCommand(Vector3.down);
            }
            else if (c == keyBind.upY && layer == 0)
            {
                KeyCommand(Vector3.up);
            }
        }
    }
}
