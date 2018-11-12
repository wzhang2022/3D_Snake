using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollide : MonoBehaviour {

    void OnCollisionEnter(Collision col)
    {
        Destroy(gameObject);
    }
    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
