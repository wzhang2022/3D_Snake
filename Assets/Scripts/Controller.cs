using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    public Vector3 direction;
    public GameObject head;
    public GameObject prefab;
	// Use this for initialization
	void Start () {
        Invoke("Repeat", 0.5f);
	}
	
	// Update is called once per frame
	void Update () {

    }

    void Repeat() {
        Move();
    }

    void Move() {
        GameObject new_object = Instantiate(prefab);
        new_object.transform.Translate(direction);
        head = new_object;
    }
}
