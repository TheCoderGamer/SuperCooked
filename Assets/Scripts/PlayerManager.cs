using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    #region Variables
    // Referencias
    private Camera cam;
    PlayerMovement pm;
    // Input
    public bool Fire1Down;
    private bool Fire1Up;
    private bool Fire2Down;
    private bool Action1;
    // RayCast
    private RayCaster camHit;
    // PickUp
    private Pickable pickable = null;
    private SpringJoint spring = null;
    private Transform pickupZone = null;
    private Rigidbody pickableObject = null;
    private bool move = false, finish = false, lookingPickable = false;
    public static bool slotFull = false;
    public bool holding = false;
    private float pickUpRange = 5;
    [SerializeField] private float dropFowardForce = 80, dropUpwardForce = 80, stayForce = 18;
    // Caja de comida
    private bool foodBoxActive = false;
    private Transform foodContainer;
    // Crafteo
    private Table table = null;

    #endregion

    private void Start() {
        pm = this.GetComponent<PlayerMovement>();
        // RayCast mirada
        cam = Camera.main;
        camHit = new RayCaster();
        camHit.OnRayExit += RayCastExit;
        camHit.OnRayEnter += RayCastEnter;
        camHit.OnRayStay += RayCastStay;
        // Pickable
        pickupZone = cam.transform.GetChild(0).GetComponent<Transform>();
        foodContainer = GameObject.Find("/Stage/Food").transform;
    }
    private void Update() {
        // Get input
        Fire1Down = Input.GetButton("Fire1");
        Fire1Up = Input.GetButtonUp("Fire1");
        Fire2Down = Input.GetButtonDown("Fire2");
        Action1 = Input.GetButton("Action1");

        RayCasting();
        PickUpAbility();
    }
    private void OnTriggerStay(Collider collider) {
        // Crafteo
        table = collider.GetComponent<Table>();
        if (table != null) {
            CraftingSystem();
        }
    }

    // RAYCAST
    private void RayCasting() {
        // RayCast mirada
        camHit.RayLength = pickUpRange;
        camHit.StartTransform = cam.transform;
        camHit.Direction = cam.transform.forward;
        camHit.CastRay();
        //previous = camHit.previous;
        //current = camHit.debug;
    }
    private void RayCastEnter(Collider collider) {
        if (!finish && !holding && collider.gameObject.CompareTag("Pickable")) {
            pickableObject = collider.GetComponent<Rigidbody>();
            lookingPickable = true;
        }
        else if (Fire1Down && !foodBoxActive && collider.gameObject.CompareTag("FoodBoxHolo")) {
            FoodBoxPickUp(collider);
        }
    }
    private void RayCastStay(Collider collider) {
        if (!finish && !holding && collider.gameObject.CompareTag("Pickable")) {
            pickableObject = collider.GetComponent<Rigidbody>();
            lookingPickable = true;
        }
        else if (!collider.gameObject.CompareTag("Pickable")) {
            lookingPickable = false;
        }
        if (Fire1Down && !foodBoxActive && collider.gameObject.CompareTag("FoodBoxHolo")) {
            FoodBoxPickUp(collider);
        }
    }
    private void RayCastExit(Collider collider) {
        if (collider.gameObject.CompareTag("Pickable")) {
            lookingPickable = false;
        }
    }
    
    // PICKUP
    private void PickUpAbility() {
        // Check
        if(holding && pickable != null && spring != null) {
            if (pickable.touchingRB == true) {
                spring.breakForce = 20;
            }
            else {
                spring.breakForce = 1000;
            }
        }
        if (spring == null && holding) {
            Drop(pickableObject);
        }

        // PickUp
        if (lookingPickable && !holding && !finish && Fire1Down) {
            PickUp(pickableObject);
        }
        // Drop
        if (holding && Fire1Up && pickableObject != null) {
            Drop(pickableObject);
        }
        // Throw
        if (holding && Fire2Down && pickableObject != null) {
            Throw(pickableObject);
        }
        // Keep
        if (move) {
            Vector3 pastPosition = pickableObject.gameObject.transform.position;
            //Vector3 pastTargetPosition = pickupZone.position;
            //Vector3 moveCube = SmoothApproach(pastPosition, pastTargetPosition, pickupZone.position, 20f);
            Vector3 moveCube = Vector3.Lerp(pastPosition, pickupZone.position, Time.deltaTime * stayForce);
            pickableObject.MovePosition(moveCube);
            //pickableObject.position = pickupZone.position;
            pickableObject.velocity = Vector3.zero;
            pickableObject.angularVelocity = Vector3.zero;
            
            //pickableObject.transform.eulerAngles = Vector3.zero;
        }
    }
    private void PickUp(Rigidbody m_pickableObject) {
        pickable = m_pickableObject.GetComponent<Pickable>();
        move = true;
        holding = true;
        slotFull = true;

        m_pickableObject.gameObject.layer = 0;
        m_pickableObject.useGravity = false;
        m_pickableObject.velocity = Vector3.zero;
        m_pickableObject.angularVelocity = Vector3.zero;
        m_pickableObject.isKinematic = false;
        m_pickableObject.transform.SetParent(foodContainer);
        m_pickableObject.transform.position = pickupZone.transform.position;
        
        spring = pickupZone.gameObject.AddComponent<SpringJoint>() as SpringJoint;
        spring.breakForce = 1000f;
        spring.spring = 100f;
        spring.minDistance = 0f;
        spring.maxDistance = 0f;
        spring.autoConfigureConnectedAnchor = false;
        spring.connectedAnchor = Vector3.zero;
        spring.anchor = Vector3.zero;
        spring.connectedBody = m_pickableObject;

        //m_pickableObject.transform.eulerAngles = anglePickable;
        //anglePickable = new Vector3(RoundTo45(anglePickable.x), RoundTo45(anglePickable.y), RoundTo45(anglePickable.z));
    }
    private void Drop(Rigidbody m_pickableObject) {
        pickable = null;
        holding = false;
        slotFull = false;
        move = false;

        m_pickableObject.useGravity = true;
        float velX = pm.totalVelocity.x / 2;
        float velZ = pm.totalVelocity.z / 2;
        m_pickableObject.velocity = new Vector3(velX, -0.5f, velZ);
        StartCoroutine(FinishPickUp(m_pickableObject));
        foodBoxActive = false;
        if (spring != null) {
            Destroy(spring);
        }
        m_pickableObject.AddTorque(new Vector3(50, 50, 50));
    }
    private void Throw(Rigidbody m_pickableObject) {
        Drop(m_pickableObject);
        m_pickableObject.AddForce(cam.transform.forward * dropFowardForce, ForceMode.Impulse);
        m_pickableObject.AddForce(cam.transform.forward * dropUpwardForce, ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        m_pickableObject.AddTorque(new Vector3(random, random, random) * 100);
    }
    IEnumerator FinishPickUp(Rigidbody _object) {
        // Finish (Evita Object Jumping)
        while (pickableObject.velocity != Vector3.zero) {
            _object.gameObject.tag = "Untagged";
            if (pickableObject.position.y < 0) {
                pickableObject.velocity = Vector3.zero;
            }
            yield return null;
        }
        if (pickableObject.velocity == Vector3.zero) {
            _object.gameObject.layer = 6;
            _object.gameObject.tag = "Pickable";
            finish = false;
        }
        yield break;
    }

    // CAJA COMIDA
    private void FoodBoxPickUp(Collider col) {
        foodBoxActive = true;
        if (holding && pickableObject != null) {
            Drop(pickableObject);
        }
        FoodBox fb = col.transform.parent.parent.GetComponent<FoodBox>();
        GameObject _gameObject = fb.Pick();
        pickableObject = _gameObject.GetComponent<Rigidbody>();
        PickUp(pickableObject);
    }
    
    // CRAFTEO MESA
    private void CraftingSystem() {
        if(Action1){
            table.Action();
        }
    }



    // Funciones utiles
    private Vector3 SmoothApproach(Vector3 pastPosition, Vector3 pastTargetPosition, Vector3 targetPosition, float speed) {
        float t = Time.deltaTime * speed;
        Vector3 v = (targetPosition - pastTargetPosition) / t;
        Vector3 f = pastPosition - pastTargetPosition + v;
        return targetPosition - v + f * Mathf.Exp(-t);
    }
    public float RoundTo45(float Decimal) {
        float i = Decimal % 45;
        if (i < 45 / 2) {
            Decimal -= i;
            return Decimal;
        }
        else if (i > 45 / 2) {
            Decimal += (45 - i);
            return Decimal;
        }
        else {
            return 0f;
        }
    }
}