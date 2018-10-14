using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour {
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
	// Use this for initialization
	void Start () {
        InvokeRepeating("Repeat", 0.5f, speed);
	}
	
	// Update is called once per frame
	void Update () {
        foreach (char c in Input.inputString) {
            if (c ==  keyBind.downX) {
                direction = Vector3.left;
            } else if (c == keyBind.upX) {
                direction = Vector3.right;
            } else if (c == keyBind.downY) {
                direction = Vector3.down;
            } else if (c == keyBind.upY) {
                direction = Vector3.up;
            } else if (c == keyBind.downZ) {
                direction = Vector3.back;
            } else if (c == keyBind.upZ) {
                direction = Vector3.forward;
            }
        }
    }

    void Repeat() {
        Move();
        CheckCollision();
    }

    void Move() {
        GameObject new_object = Instantiate(prefab, head.transform.position, head.transform.rotation);
        head.transform.Translate(direction);
    }
    
    void CheckCollision() {

    }

    private void OnTriggerEnter(Collider other) {

        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1)%2);
    }
}
