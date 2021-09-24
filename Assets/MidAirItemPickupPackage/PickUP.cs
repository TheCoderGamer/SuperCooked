using UnityEngine;
using System.Collections;

public class PickUP : MonoBehaviour {
	private Vector3 Angle;
	private SpringJoint Spring;
	private bool IsHolding = false;
	private bool SpringOverMove = false;
	public GameObject Holder;
	private GameObject HoldedItem;
	public RoundTo Round;
	private CollisionDetector Detect;
	public float MaxItemMass = 2f;
	public float PickupDistance = 3f;
	public float NormalBreakForce = 16f;
	public float RigidbodyBreakForce = 8f;
	public float ThrowingForce = 2f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Controls ();
		if (IsHolding) {
			Move();
			CheckForSpring();
			SpringManager();
		}

	}
	public void SpringManager(){
		Detect = HoldedItem.GetComponent (typeof(CollisionDetector)) as CollisionDetector;
		if (Spring) {
			if (Detect.IsHittingRB) {
				Spring.breakForce = RigidbodyBreakForce;
				SpringOverMove = true;
			}
			else if (Detect.IsHittingNOTRB){
				Spring.breakForce = NormalBreakForce;
				SpringOverMove = true;
			}
			else {
				SpringOverMove = false;
			}
				
		}

	}
	public void Raycast (){
		RaycastHit HitInfo;

		if (Physics.Raycast (transform.position, transform.forward, out HitInfo, PickupDistance)) {
			if (HitInfo.collider.GetComponent<Rigidbody>() && HitInfo.collider.GetComponent<Rigidbody>().mass < MaxItemMass){
				HoldedItem = HitInfo.collider.gameObject;
				Angle = HitInfo.collider.transform.eulerAngles;
				Angle = new Vector3(Round.RoundTo45(Angle.x),Round.RoundTo45(Angle.y),Round.RoundTo45(Angle.z));
				AddSpring ();
				IsHolding = true;
			}
			else {
				IsHolding = false;

			}

		}
	}
	public void Move(){
		if (!SpringOverMove) {
			HoldedItem.GetComponent<Rigidbody>().useGravity = false;
			HoldedItem.transform.position = Holder.transform.position;
			HoldedItem.transform.eulerAngles = Angle;
		}
	}
	public void AddSpring(){
		HoldedItem.AddComponent<CollisionDetector> ();
		Spring = Holder.AddComponent<SpringJoint> () as SpringJoint;
		Spring.breakForce = 17f;
		Spring.spring = 1000f;
		Spring.minDistance = 0f;
		Spring.maxDistance = 0f;
		Spring.autoConfigureConnectedAnchor = false;
		Spring.connectedAnchor = Vector3.zero;
		Spring.anchor = Vector3.zero;
		Spring.connectedBody = HoldedItem.GetComponent<Rigidbody>();
	}

	public void Controls () {
		if (Input.GetKeyDown (KeyCode.E)) {
			if (!IsHolding){
				Raycast();
			}
			else {
				IsHolding = false;
				Destroy(Spring);
				HoldedItem.GetComponent<Rigidbody>().useGravity = true;
				HoldedItem.GetComponent<Rigidbody>().velocity = Vector3.zero;
				HoldedItem.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			}
		}
		if (Input.GetMouseButtonDown(0) && IsHolding){
			IsHolding = false;
			Destroy(Spring);
			HoldedItem.GetComponent<Rigidbody>().useGravity = true;
			HoldedItem.GetComponent<Rigidbody>().velocity = Vector3.zero;
			HoldedItem.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			HoldedItem.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward*ThrowingForce*50f);
		}
	}
	public void CheckForSpring(){
		if (!Spring) {
			IsHolding = false;
			HoldedItem.GetComponent<Rigidbody>().useGravity = true;
			HoldedItem.GetComponent<Rigidbody>().velocity = Vector3.zero;
			HoldedItem.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
	}
}
