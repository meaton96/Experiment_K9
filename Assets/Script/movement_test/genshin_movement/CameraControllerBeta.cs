using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraControllerBeta : MonoBehaviour {
    public Transform playerTransform;
    [SerializeField] private PlayerControllerBeta playerControllerScript;
    [SerializeField] private Vector3 defaultCameraOffset;
    public float rotationSpeed = 5.0f;
    public bool lockVerticalRotation = false;

    private bool canRotateCamera = true;

    [SerializeField] private float cameraLookOffset = 10f;

    private Vector3 previousPlayerPosition;

    
   // private bool is3D = true;

    private void Start() {
       // transform.position = playerTransform.position + cameraDefaultOffset;
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
        transform.RotateAround(playerTransform.position + playerTransform.forward * cameraLookOffset,
            Vector3.up, mouseX);
    }

    void RotateVertical(float mouseY) {
        transform.RotateAround(playerTransform.position + playerTransform.forward * cameraLookOffset,
            -transform.right, mouseY);
    }

    void FollowPlayer() {
        transform.position += playerTransform.position - previousPlayerPosition;
        previousPlayerPosition = playerTransform.position;
    }

    public void ToggleCameraRotation(bool enable) {
        canRotateCamera = enable;
    }
    public void ResetCameraLocation() {
     //   transform.position = playerTransform.position + defaultCameraOffset;
        transform.position =  GetUpdatedCameraPosition();
    }
    public Vector3 GetUpdatedCameraPosition() {
        // Rotate the default camera offset by the game object's local rotation
        Vector3 rotatedOffset = playerTransform.localRotation * defaultCameraOffset;

        // Calculate the updated camera position
        Vector3 updatedCameraPosition = playerTransform.position + rotatedOffset;

        return updatedCameraPosition;
    }


}
