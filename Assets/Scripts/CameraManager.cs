using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float mosueSensitivity = 100f;
    private float xRotation = 0f;
    public Transform playerBody;
    public Transform camPos;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate() {
        // Get Input
        float mouseX = Input.GetAxis("Mouse X") * mosueSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mosueSensitivity * Time.deltaTime;
        
        //Calcular rotacion
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Rotate
        playerBody.Rotate(Vector3.up * mouseX);

        // Mover camara
        this.transform.position = camPos.position;
        Vector3 newRotation = new Vector3(this.transform.eulerAngles.x, playerBody.transform.eulerAngles.y, this.transform.eulerAngles.z);
        gameObject.transform.eulerAngles = newRotation;
    }
}
