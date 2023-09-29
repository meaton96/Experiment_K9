using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerBeta : MonoBehaviour {

    public float moveSpeed = 5.0f;
    public Transform cameraTransform;

    void Update() {
        PlayerMovement();
    }

    void PlayerMovement() {
        var keyboard = Keyboard.current;
        Vector3 cameraForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 cameraRight = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        if (keyboard.wKey.isPressed) {
            transform.forward = cameraForward;
            transform.position += cameraForward * moveSpeed * Time.deltaTime;
        }
        else if (keyboard.sKey.isPressed) {
            transform.forward = -cameraForward;
            transform.position -= cameraForward * moveSpeed * Time.deltaTime;
        }

        if (keyboard.aKey.isPressed) {
            Vector3 targetDirection = -cameraRight;
            transform.forward = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * moveSpeed);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else if (keyboard.dKey.isPressed) {
            Vector3 targetDirection = cameraRight;
            transform.forward = Vector3.Slerp(transform.forward, targetDirection, Time.deltaTime * moveSpeed);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }
}

