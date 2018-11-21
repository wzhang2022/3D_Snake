using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    // prefabs
    public GameObject head;
    public GameObject bodyPrefab;
    public ArrayList body;
    public Material powerUpMaterial;
    public Material headMarkerMaterial;

    // state variables
    public int length = 5;
    public int powerTurns = 0;
    [System.Serializable]
    public struct KeyBind {
        public char upZ;
        public char downZ;
        public char upX;
        public char downX;
        public char upY;
        public char downY;
    }
    public KeyBind keyBind;

    // variables to manage movement
    public Vector3 direction;
    private int layer = 0;
    private Vector3 direction2D;
    private Vector3 queuedDirection;
    private Vector3 direction_prev;

    void Start () {
        direction2D = direction;
        queuedDirection = direction;
        body = new ArrayList();
    }
	
    // process a movement command
    void MoveCommand(Vector3 c)
    {
        // allow direction if it is not in same axis of previous move
        if (c != -1 * direction_prev && c != direction_prev)
        {
            direction = c;
            // clear queued commands when making valid move
            queuedDirection = c;
            // also set 2D direction if this is a planar move
            if (c.y == 0)
            {
                direction2D = direction;
            }
        } else
        {
            // buffer moves in same axis
            queuedDirection = c;
        }
    }

	// recieve movement commands at every timestep
	void Update () {
        foreach (char c in Input.inputString) {
            if (c == keyBind.downX) {
                MoveCommand(Vector3.left);
            } else if (c == keyBind.upX) {
                MoveCommand(Vector3.right);
            } else if (c == keyBind.downZ) {
                MoveCommand(Vector3.back);
            } else if (c == keyBind.upZ) {
                MoveCommand(Vector3.forward);
            } else if (c == keyBind.downY && layer == 1) {
                MoveCommand(Vector3.down);
            } else if (c == keyBind.upY && layer == 0) {
                MoveCommand(Vector3.up);
            }
        }
    }

    public Vector3 NextMove(HashSet<Vector3> playerPositions)
    {
        // decrement powered-up turns (show yellow marker w/ flash at end for visual warning)
        powerTurns = Mathf.Max(powerTurns - 1, 0);
        if (powerTurns > 0 && powerTurns != 2)
        {
            this.transform.Find("HeadMarker").GetComponent<MeshRenderer>().material = powerUpMaterial;
        }
        else
        {
            this.transform.Find("HeadMarker").GetComponent<MeshRenderer>().material = headMarkerMaterial;
        }

        // handle length reductions
        while (body.Count >= length)
        {
            GameObject end = (GameObject)body[0];
            body.RemoveAt(0);
            playerPositions.Remove(end.transform.position);
            Destroy(end);
        }

        // return where the player will go next
        if ((direction == Vector3.up && layer == 1) ||
            (direction == Vector3.down && layer == 0))
        {
            return head.transform.position + direction2D;
        }
        return head.transform.position + direction;
    }

    public void Move() {
        if (direction == Vector3.up)
        {
            if (layer == 0)
            {
                layer = 1;
            }
            else if (layer >= 1)
            {
                direction = direction2D;
            }
        }
        else if (direction == Vector3.down)
        {
            if (layer == 1)
            {
                layer = 0;
            }
            else if (layer <= 0)
            {
                direction = direction2D;
            }
        }
        Vector3 oldPosition = head.transform.position;
        head.transform.Translate(direction);
        direction_prev = direction;
        GameObject new_object = Instantiate(bodyPrefab, oldPosition, head.transform.rotation);
        body.Add(new_object);

        // auto-load queued moves to allow for tight turns
        MoveCommand(queuedDirection);
    }
}
