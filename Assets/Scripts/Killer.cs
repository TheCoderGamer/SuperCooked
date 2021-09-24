using UnityEngine;

public class Killer : MonoBehaviour {
    private void OnCollisionEnter(Collision collision) {
        Destroy(collision.gameObject);
    }
}
