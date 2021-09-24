using UnityEngine;
using System.Collections;

public class CollisionDetector : MonoBehaviour {
	public bool IsHittingRB = false;
	public bool IsHittingNOTRB;

	public void Start(){

	}

	public void OnCollisionEnter (Collision hit){
		if (hit.collider.GetComponent<Rigidbody>()) {
			IsHittingRB = true;
			IsHittingNOTRB = false;
		}
		else if (!hit.collider.GetComponent<Rigidbody>()){
			IsHittingRB = false;
			IsHittingNOTRB = true;
		}
	}
	public void OnCollisionExit (){
		IsHittingRB = false;
		IsHittingNOTRB = false;
	}
}
