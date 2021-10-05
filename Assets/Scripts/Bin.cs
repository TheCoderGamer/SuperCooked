using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bin : MonoBehaviour {
    PlayerManager pm;
    private bool occupy = false;
    private void Start() {
        pm = FindObjectOfType<PlayerManager>();
    }
    private void OnTriggerStay(Collider other) {
        if (other.GetComponent<Pickable>() != null && !pm.holding && !occupy){
            if (other.name.StartsWith("f_p")) {
                Destroy(other.gameObject);
                StartCoroutine(SummonPlato());
            }
            else if (other.name.StartsWith("f_")) {
                Destroy(other.gameObject);
            }
        }
    }
    IEnumerator SummonPlato() {
        occupy = true;
        GameObject _gameObject = Instantiate(Resources.Load("Prefabs/utils/u_plato"), new Vector3(transform.position.x, transform.position.y+1f, transform.position.z), Quaternion.identity) as GameObject;
        _gameObject.GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(2);
        occupy = false;
    }
}
