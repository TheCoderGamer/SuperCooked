using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour {

    public bool touchingRB = false;
    public GameObject table;

    private void Start() {
        this.transform.parent = GameObject.Find("/Stage/Food").transform;
    }

    private void OnCollisionEnter(Collision collision) {
        /*if (collision.gameObject.tag.Equals("MovingPlatform")) {
            this.gameObject.layer = 6;
            this.gameObject.tag = "Pickable";
        }*/
        touchingRB = true;
    }
    private void OnCollisionExit() {
        touchingRB = false;
    }
}
