using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    private Transform pos1;
    private Transform pos2;
    private Transform platform;
    Rigidbody prb;
    public float speed = 1f;
    bool move = false;
    Transform foodContainer;

    void Start() {
        foodContainer = GameObject.Find("/Stage/Food").transform;
        platform = transform;
        pos1 = transform.parent.GetChild(1);
        pos2 = transform.parent.GetChild(2);
        prb = platform.GetComponent<Rigidbody>();
        platform.position = pos1.position;
    }

    void FixedUpdate() {
        if (platform.position == pos2.position || move) {
            //platform.position = Vector3.MoveTowards(platform.position, pos1.position, Time.deltaTime * speed);
            prb.MovePosition(Vector3.MoveTowards(platform.position, pos1.position, Time.deltaTime * speed));
            move = true;
        }
        if (platform.position == pos1.position || !move) {
            //platform.position = Vector3.MoveTowards(platform.position, pos2.position, Time.deltaTime * speed);
            prb.MovePosition(Vector3.MoveTowards(platform.position, pos2.position, Time.deltaTime * speed));
            move = false;
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Pickable>() != null) {
            other.transform.parent = this.transform;
            other.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<Pickable>() != null) {
            other.transform.parent = foodContainer;
            other.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}

