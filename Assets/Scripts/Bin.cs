using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bin : MonoBehaviour {
    private void OnTriggerStay(Collider other) {
        if (other.tag == "Pickable" && other.gameObject.layer == 6){
            Destroy(other.gameObject);
        }
    }
}
