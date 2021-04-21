using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [Header("Referencias")]
    public CharacterController controller;
    public Transform groundCheck;

    [Header("Variables publicas")]
    [Range(1,20)]public float speed = 10;
    [Range(-20,0)]public float gravity = -9.8f;
    [Range(0.01f,1)]public float groundDistance = 0.4f;
    public LayerMask groundMask;
    [Range(0.1f,10)]public float jumpHeight = 2;
    [Range(1,10)]public int maxJumps = 2;
    [Range(1,30)]public float dashSpeed = 15f;
    [Range(0.01f,1)]public float dashTime = 0.25f;
    [Range(0.1f,20)]public float dashCooldown = 2f;
    [Range(0.01f,0.8f)]public float dashSmoothOn = 0.2f;
    [Range(0.01f,0.1f)]public float dashSmoothOff = 0.05f;

    // Variables privadas
    private Vector3 velocity;
    private bool isGrounded;
    private int jumps;
    private bool jumping = false;
    private bool dashing = false;
    float currentFOV;

    private void Start() {
    currentFOV = Camera.main.fieldOfView;    
    }

    void Update() {
        // Obtener input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool y = Input.GetButton("Jump");
        bool dashBut = Input.GetButton("Fire 3");
        
        // Calcular si esta en el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0) {
            velocity.y = -2;
            controller.slopeLimit = 45.0f;
            jumps = 0;
            jumping = false;
        }

        // Evitar transpasar objetos desde abajo
        if ((controller.collisionFlags & CollisionFlags.Above) != 0) {
            velocity.y = 0f;
        }

        // Calcular input
        Vector3 inputMove = Vector3.ClampMagnitude(transform.right * x + transform.forward * z, 1f) * speed * Time.deltaTime;

        // Salto
        if(y && !jumping && jumps < maxJumps) {
            jumping = true;
            controller.slopeLimit = 100.0f;
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            jumps++;
        }
        if (!y) { jumping = false; }

        // Dash
        if (!dashing && dashBut) {
            StartCoroutine(Dash());
        }

        // Gravedad
        velocity.y += gravity * Time.deltaTime;

        // Aplicar movimientos
        controller.Move((velocity * Time.deltaTime) + inputMove);
    }
    IEnumerator Dash() {
        dashing = true;
        float startTime = Time.time;
        float t = 0;
        Vector3 speed = transform.forward * dashSpeed * Time.deltaTime;
        Vector3 lowSpeed = new Vector3(speed.x / 4, speed.y, speed.z / 4);
        while (Time.time < startTime + dashTime) {
            Vector3 currentSpeed = Vector3.Lerp(lowSpeed, speed, t);
            controller.Move(currentSpeed);
            t += dashSmoothOn;
            yield return null;
        }
        t = 0;
        while (t < 1f) { 
            Vector3 currentSpeed = Vector3.Lerp(speed, lowSpeed, t);
            controller.Move(currentSpeed);
            t += dashSmoothOff;
            yield return null;
        }
        yield return new WaitForSeconds(dashCooldown);
        dashing = false;
    }
}
