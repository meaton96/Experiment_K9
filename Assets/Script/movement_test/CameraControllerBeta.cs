using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerBeta : MonoBehaviour
{
    public Transform playerTransform;
    public float rotationSpeed = 5.0f;
    public bool lockVerticalRotation = false;
    public readonly Vector3 cameraOffset = new(0, 3.16f, -16.42f);

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        RotateCamera();
    }
    private void LateUpdate() {
       // transform.position = playerTransform.position + cameraOffset;
    }

    void RotateCamera() {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed*Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        if (!lockVerticalRotation) {
            transform.RotateAround(playerTransform.position, Vector3.up, mouseX);
            transform.RotateAround(playerTransform.position, -transform.right, mouseY);
        }
        else {
            transform.RotateAround(playerTransform.position, Vector3.up, mouseX);
        }

        transform.LookAt(playerTransform);
    }
}
