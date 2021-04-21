using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour {
    public Rigidbody rb;
    public Transform player, pickupZone;

    [SerializeField] private float pickUpRange;
    [SerializeField] private float dropFowardForce, dropUpwardForce;
    [SerializeField] private bool equipped; 
    private bool setup = true;
    public static bool slotFull;


    private void Start() {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").transform;
        pickupZone = GameObject.Find("PickupZone").transform;
    }

    private void Update() {
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetButtonDown("Fire1")) PickUp();
        if (equipped) { rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
        if (equipped && Input.GetButtonUp("Fire1") || equipped && distanceToPlayer.magnitude > pickUpRange) Drop();
        if (equipped && Input.GetButtonDown("Fire2")) Throw();
    }
    private void FixedUpdate() {
        if (!setup) {
            Vector3 zone = pickupZone.transform.position;
            Vector3 moveCube = Vector3.Lerp(transform.position, zone, 0.1f);
            rb.MovePosition(moveCube);
        }
    }

    private void PickUp() {
        setup = false;
        equipped = true;
        slotFull = true;
        rb.useGravity = false;
        transform.SetParent(pickupZone);
        rb.detectCollisions = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    private void Drop() {
        equipped = false;
        slotFull = false;
        rb.useGravity = true;
        transform.SetParent(GameObject.Find("Stage").transform);
        setup = true;
    }
    private void Throw() {
        equipped = false;
        slotFull = false;
        rb.useGravity = true;
        transform.SetParent(GameObject.Find("Stage").transform);

        rb.velocity = GetComponent<Rigidbody>().velocity;
        rb.AddForce(Camera.main.transform.forward * dropFowardForce, ForceMode.Impulse);
        rb.AddForce(Camera.main.transform.forward * dropUpwardForce, ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random)*10);
        setup = true;
    }
}
