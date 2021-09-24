using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    private Transform pos1;
    private Transform pos2;
    private Transform platform;
    Rigidbody prb;
    public float speed = 1f;
    bool temp = false;

    void Start() {
        platform = transform;
        pos1 = transform.parent.GetChild(1);
        pos2 = transform.parent.GetChild(2);
        platform.position = pos1.position;
        prb = platform.GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        if (platform.position == pos2.position || temp) {
            //platform.position = Vector3.MoveTowards(platform.position, pos1.position, Time.deltaTime * speed);
            prb.MovePosition(Vector3.MoveTowards(platform.position, pos1.position, Time.deltaTime * speed));
            temp = true;
        }
        if (platform.position == pos1.position || !temp) {
            //platform.position = Vector3.MoveTowards(platform.position, pos2.position, Time.deltaTime * speed);
            prb.MovePosition(Vector3.MoveTowards(platform.position, pos2.position, Time.deltaTime * speed));
            temp = false;
        }
    }
}

