using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour {
    // Public variables to set the player transform, rotation speed, and transition smoothness from the editor
    public Transform playerTransform;
    public float rotationSpeed = 5.0f;
    public float smoothness = 0.5f;

    // Private variables to track if the camera is rotating, the offset from the player, and the original offset
    private bool isRotating;
    private Vector3 offset;
    private Vector3 originalOffset;

    void Start() {
        // Calculate the initial offset between the camera and the player at the start of the game
        offset = transform.position - playerTransform.position;
        originalOffset = offset;
    }

    void LateUpdate() {
        //isRotating = Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed;    
        // Check if the right mouse button is pressed for rotating the camera
        if (Mouse.current.rightButton.isPressed) {
            // Calculate the horizontal movement of the mouse and set the rotating flag to true
            float horizontalInput = Mouse.current.delta.x.ReadValue() * rotationSpeed * Time.deltaTime;
            isRotating = true;

            // Rotate the camera around the player based on the mouse movement
            offset = Quaternion.AngleAxis(horizontalInput, Vector3.up) * offset;
        }
        // Check if the right mouse button is released and the camera was rotating
        else if (!Mouse.current.rightButton.isPressed && isRotating) {
            // Check if any movement keys are pressed
            if (Keyboard.current.wKey.isPressed || Keyboard.current.sKey.isPressed
                || Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed) {
                // Calculate the target offset based on the player's forward direction
                Vector3 playerForward = playerTransform.forward;
                Vector3 targetOffset = -playerForward * originalOffset.magnitude;
                float step = smoothness * Time.deltaTime;

                // Transition the camera back to its default position smoothly
                offset = Vector3.Slerp(offset, targetOffset, step);

                // Reset the rotating flag if the camera has reached the target position
                if (Vector3.Distance(offset, targetOffset) < 0.01f) {
                    isRotating = false;
                }
            }
        }

        // Calculate the desired position of the camera based on the current offset
        Vector3 desiredPosition = playerTransform.position + offset;

        // Move the camera to the desired position smoothly
        transform.position = Vector3.Slerp(transform.position, desiredPosition, smoothness);

        // Ensure the camera is always facing the player
        transform.LookAt(playerTransform);
    }
}
