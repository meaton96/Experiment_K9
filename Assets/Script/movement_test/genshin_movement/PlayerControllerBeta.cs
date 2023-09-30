using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerBeta : MonoBehaviour {

    public float moveSpeed3D = 5.0f;
    public float moveSpeed2D = 10.0f;
    public Transform cameraTransform;
    private bool Is3D = true;
    private bool CanMove = true;

    [SerializeField] private GameObject player2D;

    void Update() {
        if (CanMove) {
            if (Is3D) {
                Move3D();
            }
            else {
                Move2D();
            }
        }
    }
    void Move3D() {
        Vector2 input = GetInput();

        if (input != Vector2.zero) {  // Check if there's any input
            Vector3 cameraForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            Vector3 cameraRight = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

            Vector3 direction = cameraForward * input.y + cameraRight * input.x;

            transform.forward = direction.normalized;  // Only set forward direction if there is input
            transform.position += moveSpeed3D * Time.deltaTime * direction;
        }
    }

    void Move2D() {
        Vector2 input = GetInput();

        Vector3 up = player2D.transform.up;
        Vector3 left = -player2D.transform.right;

        Vector3 direction = up * input.y + left * input.x;

        transform.position += moveSpeed2D * Time.deltaTime * direction;
    }

    Vector2 GetInput() {
        var keyboard = Keyboard.current;
        return new Vector2(keyboard.dKey.isPressed ? 1 : keyboard.aKey.isPressed ? -1 : 0,
                           keyboard.wKey.isPressed ? 1 : keyboard.sKey.isPressed ? -1 : 0);
    }

    
    public void ChangeDimension() {

        Is3D = !Is3D;

    }
    public void ToggleMovement() {
        CanMove = !CanMove;
    }
    public bool IsIn3D() { return Is3D; }
}

