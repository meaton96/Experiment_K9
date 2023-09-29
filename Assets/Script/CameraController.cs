using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    public Transform playerTransform;
    public float rotationSpeed = 5.0f;
    public float smoothness = 0.5f;

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
        else if (!Mouse.current.rightButton.isPressed) {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.sKey.isPressed
                || Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed) {
                isRotating = true;
            }
        }

        if (isRotating && !Mouse.current.rightButton.isPressed) { // Ensures camera doesn’t snap back while right mouse button is pressed
            Vector3 playerForward = playerTransform.forward;
            Vector3 targetOffset = -playerForward * originalOffset.magnitude;
            float step = smoothness * Time.deltaTime;
            offset = Vector3.Slerp(offset, targetOffset, step);

            if (Vector3.Distance(offset, targetOffset) < 0.01f) {
                isRotating = false;
            }
        }

        Vector3 desiredPosition = playerTransform.position + offset;
        transform.position = Vector3.Slerp(transform.position, desiredPosition, smoothness);
        transform.LookAt(playerTransform);
    }
}
