using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller_Two_layers : MonoBehaviour {
    public Vector3 direction;
    public GameObject head;
    public GameObject prefab;
    public float speed = 0.2f;
    [System.Serializable]
    public struct KeyBind {
        public char upZ;
        public char downZ;
        public char upY;
        public char downY;
        public char upX;
        public char downX;
    }
    public KeyBind keyBind;

    private int layer = 0;
    private Vector3 direction2D;
	// Use this for initialization
	void Start () {
        InvokeRepeating("Repeat", 0.5f, speed);
        direction2D = direction;
	}
	
	// Update is called once per frame
	void Update () {
        foreach (char c in Input.inputString) {
            if (c == keyBind.downX && direction != Vector3.right) {
                direction = Vector3.left;
                direction2D = direction;
            } else if (c == keyBind.upX && direction != Vector3.left) {
                direction = Vector3.right;
                direction2D = direction;
            } else if (c == keyBind.downZ && direction != Vector3.forward) {
                direction = Vector3.back;
                direction2D = direction;
            } else if (c == keyBind.upZ && direction != Vector3.back) {
                direction = Vector3.forward;
                direction2D = direction;
            } else if (c == keyBind.downY && direction != Vector3.up && layer == 1) {
                direction = Vector3.down;
            } else if (c == keyBind.upY && direction != Vector3.down && layer == 0) {
                direction = Vector3.up;
            }
        }
    }

    void Repeat() {
        if (direction == Vector3.up) {
            if (layer == 0) {
                layer = 1;
            }  else if (layer >= 1){
                direction = direction2D;
            }
        } else if (direction == Vector3.down) {
            if (layer == 1) {
                layer = 0;
            } else if (layer <= 0){
                direction = direction2D;
            }
        }
        Move();
    }

    void Move() {
        GameObject new_object = Instantiate(prefab, head.transform.position, head.transform.rotation);
        head.transform.Translate(direction);

    }

    private void OnTriggerEnter(Collider other) {

        SceneManager.LoadScene(0);
    }
}
