using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  // Make sure to install the new Input System package

public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 200.0f;

    private Vector3 moveDirection;
    private float rotation;
    private CharacterController characterController;

    void Start() {
        characterController = GetComponent<CharacterController>();
    }

    void Update() {
        // Get the Input from the new Input System
        var move = Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0;
        var rotate = Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0;

        // Calculate the movement direction
        moveDirection = move * moveSpeed * transform.TransformDirection(Vector3.forward);
        rotation = rotate * rotationSpeed * Time.deltaTime;

        // Apply gravity (optional)
       // moveDirection.y -= 9.8f * Time.deltaTime;

        // Move the player
        characterController.Move(moveDirection * Time.deltaTime);
        transform.Rotate(0, rotation, 0);
    }
}
