using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerBeta : MonoBehaviour {
    public Transform playerTransform;
    [SerializeField] private PlayerControllerBeta playerControllerScript;
    public float rotationSpeed = 5.0f;
    public bool lockVerticalRotation = false;

    private bool canRotateCamera = true;

    private Vector3 previousPlayerPosition;
   // private bool is3D = true;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        previousPlayerPosition = playerTransform.position;
    }

    void Update() {
        //allow camera rotation only in 3d mode
        if (playerControllerScript.IsIn3D() && canRotateCamera) {
            RotateCamera();
        }
    }

    private void LateUpdate() {
        FollowPlayer();
    }

    void RotateCamera() {
        float mouseX = Input.GetAxis("Mouse X")
            * rotationSpeed
            * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y")
            * rotationSpeed
            * Time.deltaTime;

        RotateHorizontal(mouseX);

        if (!lockVerticalRotation) {
            RotateVertical(mouseY);
        }

        transform.LookAt(playerTransform);
    }

    void RotateHorizontal(float mouseX) {
        transform.RotateAround(playerTransform.position, Vector3.up, mouseX);
    }

    void RotateVertical(float mouseY) {
        transform.RotateAround(playerTransform.position, -transform.right, mouseY);
    }

    void FollowPlayer() {
        transform.position += playerTransform.position - previousPlayerPosition;
        previousPlayerPosition = playerTransform.position;
    }

    public void ToggleCameraRotation(bool enable) {
        canRotateCamera = enable;
    }


}
