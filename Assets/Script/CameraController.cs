using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    public Transform playerTransform;
    public float rotationSpeed = 5.0f;
    public float smoothness = 0.5f;
    public float verticalOffset = 2.0f; // Added vertical offset
    public float lookDownAngle = 15.0f; // Added angle to look down

    private bool isRotating;
    private Vector3 offset;
    private Vector3 originalOffset;

    void Start() {
        offset = transform.position - playerTransform.position;
        originalOffset = offset;
    }

    void LateUpdate() {
        if (Mouse.current.rightButton.isPressed) {
            float horizontalInput = Mouse.current.delta.x.ReadValue() * rotationSpeed * Time.deltaTime;
            isRotating = true;
            offset = Quaternion.AngleAxis(horizontalInput, Vector3.up) * offset;
        }
        else if (!Mouse.current.rightButton.isPressed && isRotating) {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.sKey.isPressed
                || Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed) {
                Vector3 playerForward = playerTransform.forward;
                Vector3 targetOffset = -playerForward * originalOffset.magnitude;
                targetOffset.y += verticalOffset; // Adjust vertical position of the camera
                float step = smoothness * Time.deltaTime;
                offset = Vector3.Slerp(offset, targetOffset, step);

                if (Vector3.Distance(offset, targetOffset) < 0.01f) {
                    isRotating = false;
                }
            }
        }

        Vector3 desiredPosition = playerTransform.position + offset;
        transform.position = Vector3.Slerp(transform.position, desiredPosition, smoothness);

        // Adjust the camera's rotation to look down at the player
        if (!isRotating) {
            Quaternion lookAtRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
            Quaternion tiltedDownRotation = Quaternion.Euler(lookAtRotation.eulerAngles.x + lookDownAngle, lookAtRotation.eulerAngles.y, lookAtRotation.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, tiltedDownRotation, smoothness);
        }
        else {
            transform.LookAt(playerTransform);
        }
    }
}
