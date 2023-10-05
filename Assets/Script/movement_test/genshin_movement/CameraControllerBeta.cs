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
    private Vector3 prevCamPos;

  //  [SerializeField] private float cameraLookOffset = 10f;

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
    /// <summary>
    /// Handle mouse input for camera rotation using RotateHorizontal and RotateVertical, also keeps the camera looking at the player
    /// </summary>
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
    /// <summary>
    /// Handles horizontal camera rotation
    /// </summary>
    /// <param name="mouseX"></param>
    void RotateHorizontal(float mouseX) {
        transform.RotateAround(playerTransform.position,
            Vector3.up, mouseX);
    }

    /// <summary>
    /// Handles verticle camera rotaton 
    /// </summary>
    /// <param name="mouseY">float rotational speed of the mouse in the y direction</param>
    void RotateVertical(float mouseY) {
        //possibly could lock this value to prevent clipping into the ground but i think a hitbox is a better option
        //so when the player keeps trying to look up the camera should slide forward towards the player
        //possibly also making the player model see through
        transform.RotateAround(playerTransform.position,
            -transform.right, mouseY);
    }
    /// <summary>
    /// Keeps the camera moving with the player's transform
    /// </summary>
    void FollowPlayer() {
        
        transform.position += playerTransform.position - previousPlayerPosition;
        previousPlayerPosition = playerTransform.position;
        
        if (prevCamPos != transform.position) {
            Debug.Log(transform.position);
            prevCamPos = transform.position;
        }
    }

    /// <summary>
    /// Enables or disables the camera's rotation based on the provided boolean value.
    /// </summary>
    /// <param name="enable">A boolean value indicating whether to enable (true) or disable (false) the camera rotation.</param>
    public void ToggleCameraRotation(bool enable) {
        canRotateCamera = enable;
    }
    /// <summary>
    /// Resets the camera's position and orientation to its default state, typically aligning it with the player's position and orientation.
    /// </summary>
    public void ResetCameraLocation() {
     //   transform.position = playerTransform.position + defaultCameraOffset;
        transform.position =  GetUpdatedCameraPosition();
    }
    /// <summary>
    /// Calculates and returns the updated position of the camera based on the player's current position and orientation, as well as any applicable camera offsets and constraints.
    /// </summary>
    /// <returns>A Vector3 representing the updated position of the camera.</returns>
    public Vector3 GetUpdatedCameraPosition() {
        // Rotate the default camera offset by the game object's local rotation
        Vector3 rotatedOffset = playerTransform.localRotation * defaultCameraOffset;

        // Calculate the updated camera position
        Vector3 updatedCameraPosition = playerTransform.position + rotatedOffset;

        return updatedCameraPosition;
    }


}
