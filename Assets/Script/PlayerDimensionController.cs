using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDimensionController : MonoBehaviour {
    [SerializeField] private float raycastLength = 5f;
    [SerializeField] private GameObject projectionEntry;
    [SerializeField] private GameObject projectionNoEntry;
    [SerializeField] private GameObject dog3D; 
    private float cameraTransitionSpeed = 4f; 

    

    [SerializeField] private CameraControllerBeta cameraControllerScript;
    private PlayerControllerBeta playerControllerScript;

    private bool canTransitionTo2D = false;
    private Vector3 spritePosition; 
    private bool isTransitioningTo2D = false; 

    //speed at which the 2d projection moves when rotating the 3d camera
    [SerializeField] private float projectionMoveSpeed = 1.0f;

    //for flying the camera to the 2d sprite
    private readonly float cameraOffset2D = 20f;
    private Vector3 cameraTranstionTarget;
    private Vector3 transferLocation;

    private void Start() {
        playerControllerScript = GetComponent<PlayerControllerBeta>();  
    }

    private void Update() {


        HandleDimensionTransition();

        //update the position of the projected sprite if the camera is being rotated
        if (Input.GetKey(KeyCode.Space) && playerControllerScript.IsIn3D()) {
            UpdateProjectionPosition();
        }
    }
    private void HandleDimensionTransition() {
        // Simplified the conditions using methods to make it more readable
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (playerControllerScript.IsIn3D())
                PerformRaycast();
            else
                dog3D.SetActive(true);
        }
        //release space to initiate trasition to 2d
        if (Input.GetKeyUp(KeyCode.Space) && playerControllerScript.IsIn3D()) {
            if (canTransitionTo2D)
                TransitionTo2D();
            else
                DisableProjections();
        }

        if (isTransitioningTo2D)
            TransitionCamera();
    }

    //performs a raycast in the transform's forward direction
    private void PerformRaycast() {
        Ray ray = new(transform.position, transform.forward);

        //hit
        if (Physics.Raycast(ray, out RaycastHit hit, raycastLength)) {
            //check if its a wall
            //all walls need to be layer 6 for this to work
            if (hit.collider.gameObject.layer == 6) {
                WallBehaviour wallBehaviour = hit.collider.GetComponent<WallBehaviour>();

                //check if the wall allows dimension transfer and enable the appropriate sprite
                if (wallBehaviour != null && wallBehaviour.AllowsDimensionTransition) {
                    EnableProjection(projectionEntry, hit);
                    canTransitionTo2D = true;
                }
                else {
                    EnableProjection(projectionNoEntry, hit);
                    canTransitionTo2D = false;
                }
            }
        }
    }
    //update the position of the projection if the player is rotating the camera around
    private void UpdateProjectionPosition() {
        Ray ray = new(transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastLength)) {
            if (hit.collider.gameObject.layer == 6) {
                WallBehaviour wallBehaviour = hit.collider.GetComponent<WallBehaviour>();

                GameObject activeProjection = wallBehaviour != null && wallBehaviour.AllowsDimensionTransition ? projectionEntry : projectionNoEntry;

                // Move the projection sprite
                Vector3 moveDirection = projectionMoveSpeed * Time.deltaTime * Camera.main.transform.forward;
                activeProjection.transform.position += moveDirection;

                // Rotate the sprite to face the camera and stay flat against the wall
                activeProjection.transform.forward = -hit.normal;

                EnableProjection(activeProjection, hit);
            }
            else {
                DisableProjections();
            }
        }
        else {
            DisableProjections();
        }
    }
    //enable one of the projections
    private void EnableProjection(GameObject projection, RaycastHit hit) {
        projection.SetActive(true);
        
        //get the direction to the camera and create an offset based on it
        Vector3 directionToCamera = (Camera.main.transform.position - hit.point).normalized;
        Vector3 positionAdjustment = new(
            x: Mathf.Abs(transform.forward.z) * directionToCamera.x,
            0,
            z: Mathf.Abs(transform.forward.x) * directionToCamera.z
        );
        //set the rotation and position of the projection
        projection.transform.SetPositionAndRotation(hit.point + positionAdjustment * 0.1f, Quaternion.LookRotation(hit.normal, Vector3.up));

        //update transfer location and camera transition target for when the space bar is released 
        transferLocation = hit.point + positionAdjustment * 0.1f;
        cameraTranstionTarget = transferLocation + cameraOffset2D * hit.collider.transform.up;
    }

    private void DisableProjections() {
        projectionEntry.SetActive(false);
        projectionNoEntry.SetActive(false);
    }

    private void TransitionTo2D() {
        dog3D.SetActive(false);
        isTransitioningTo2D = true;
        spritePosition = projectionEntry.transform.position;

        //toggle camera rotation controls and tell player script to swap to 2d movement
        cameraControllerScript.ChangeDimension();
        playerControllerScript.ToggleMovement();        
    }

    //smooth pan camera to sprite location
    private void TransitionCamera() {
        
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraTranstionTarget, cameraTransitionSpeed * Time.deltaTime);

        Vector3 lookDirection = spritePosition - Camera.main.transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, rotation, cameraTransitionSpeed * Time.deltaTime);

        if (Vector3.Distance(Camera.main.transform.position, cameraTranstionTarget) < 0.15f) {
            isTransitioningTo2D = false;
            
            playerControllerScript.ChangeDimension();
            playerControllerScript.ToggleMovement();
        }
    }
}
