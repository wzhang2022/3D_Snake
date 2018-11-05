using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    public Vector3 direction;
    public GameObject head;
    public GameObject bodyPrefab;
    public ArrayList body;
    public int length = 5;
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
    public MatchManager manager;

    private int layer = 0;
    private Vector3 direction2D;
    private Vector3 direction_prev;
    // Use this for initialization

    
    void Start () {
        direction2D = direction;
        body = new ArrayList();
    }
	
	// Update is called once per frame
	void Update () {
        foreach (char c in Input.inputString) {
            if (c == keyBind.downX && direction_prev != Vector3.right) {
                direction = Vector3.left;
                direction2D = direction;
            } else if (c == keyBind.upX && direction_prev != Vector3.left) {
                direction = Vector3.right;
                direction2D = direction;
            } else if (c == keyBind.downZ && direction_prev != Vector3.forward) {
                direction = Vector3.back;
                direction2D = direction;
            } else if (c == keyBind.upZ && direction_prev != Vector3.back) {
                direction = Vector3.forward;
                direction2D = direction;
            } else if (c == keyBind.downY && direction_prev != Vector3.up && layer == 1) {
                direction = Vector3.down;
            } else if (c == keyBind.upY && direction_prev != Vector3.down && layer == 0) {
                direction = Vector3.up;
            }
        }
    }

    public Vector3 NextMove()
    {
        if ((direction == Vector3.up && layer == 1) ||
            (direction == Vector3.down && layer == 0))
        {
            return head.transform.position + direction2D;
        }
        while (body.Count >= length)
        {
            GameObject end = (GameObject)body[0];
            body.RemoveAt(0);
            manager.playerPositions.Remove(end.transform.position);
            Destroy(end);
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
    }
}
