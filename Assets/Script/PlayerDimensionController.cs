using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerDimensionController : MonoBehaviour {
    public const int WALL_LAYER = 6;
    public const float WALL_DRAW_OFFSET = 0.001f;

    [SerializeField] private float raycastLength = 50f;
    [SerializeField] private GameObject projectionEntry;
    [SerializeField] private GameObject projectionNoEntry;
    [SerializeField] private GameObject dog3D;
    private float cameraTransitionSpeed = 4f;

    //flag for Ranged version of device where you hold down space and release to transfer
    public bool RangedDOGEnabled = false;



//    [SerializeField] private CameraControllerBeta cameraControllerScript;
    private PlayerControllerBeta playerControllerScript;
    [SerializeField] private InterfaceBehaviour interfaceScript;

    private bool canTransitionTo2D = false;
    private Vector3 spritePosition;
    private bool isTransitioningTo2D = false;

    //speed at which the 2d projection moves when rotating the 3d camera
    [SerializeField] private float projectionMoveSpeed = 1.0f;

    //for flying the camera to the 2d sprite
    private readonly float cameraOffset2D = 20f;
    private Vector3 cameraTranstionTarget;
    private Vector3 transferLocation;

    //vars for toggle mode DOG

    //flag for if the DOG is toggled on or not
    public bool DOGEnabled = true;

    public float DOGProjectionRange = 25f;
    [SerializeField] private GameObject projectionOutOfRange;

    private void Start() {
        playerControllerScript = GetComponent<PlayerControllerBeta>();
        interfaceScript.SetDogToggleText(RangedDOGEnabled);
    }

    private void Update() {

        if (Keyboard.current.f1Key.wasPressedThisFrame) {
            SwapDOGModes();
        }

        //manual dog mode
        if (RangedDOGEnabled) {
            HandleManualTransition();

            //update the position of the projected sprite if the camera is being rotated
            if (Input.GetKey(KeyCode.Space) && playerControllerScript.IsIn3D()) {
                MoveProjectionWithCamera();
            }
        }
        //auto mode
        //TODO: add ability to toggle off DOG to leave wall
        else if (DOGEnabled) {
            HandleAutoDOG();


        }
    }
    //swap between manual and auto mode
    //TODO: test how this behaves when a projection is currently active on a wall
    private void SwapDOGModes() {
        RangedDOGEnabled = !RangedDOGEnabled;
        interfaceScript.SetDogToggleText(RangedDOGEnabled);
        DisableProjections();

    }
    //handles collisions
    private void OnCollisionEnter(Collision collision) {
        //only handle 3d collisions while in 3d mode
        if (playerControllerScript.IsIn3D()) {
            //handle a collision with a wall to potentially trnasfer to 2d
            if (collision.collider.gameObject.layer == WALL_LAYER) {
                if (collision.collider.GetComponent<WallBehaviour>().AllowsDimensionTransition && DOGEnabled) {
                    TransitionTo2D();
                }
            }
        }
    }
    //handles the auto mode of the Dimension device
    public void HandleAutoDOG() {
        
        GameObject nearestWall;
        //get wall and projection draw location (closest point to dog on the wall)
        if (isTransitioningTo2D)
            TransitionCamera();
        else if (playerControllerScript.IsIn3D()) {
            //get the nearest wall location
            //also store the location on the wall where we wawnt to draw the projection
            if ((nearestWall = FindNearbyWall(out Vector3 projectionDrawLocation)) != null) {
                if (projectionOutOfRange.activeInHierarchy == false) {

                    //activate and set position and rotation of the projection
                    projectionOutOfRange.transform.position = projectionDrawLocation;
                    projectionOutOfRange.transform.forward = nearestWall.transform.up;
                    projectionOutOfRange.SetActive(true);
                }
                //always move the projection while it is enabled 
                projectionOutOfRange.transform.position = projectionDrawLocation + (nearestWall.transform.up * WALL_DRAW_OFFSET);
                projectionOutOfRange.transform.forward = nearestWall.transform.up;
            }
            else {
                projectionOutOfRange.SetActive(false);
            }
            
        }
    }
    //finds the nearest wall to this game object by performing an Overlap Sphere test
    //returns the wall which is the shorest distance away and sets the out variable to the closest point on the wall to the player
    public GameObject FindNearbyWall(out Vector3 closestPoint) {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DOGProjectionRange);


        GameObject closestObject = null;
        float closestDistance = DOGProjectionRange; // Initialize with the maximum possible distance
        closestPoint = Vector3.zero; 

        foreach (var hitCollider in hitColliders) {
            // Ensure not to return the game object itself and only check objects on layer 6 (wall layer)
            if (hitCollider.gameObject == gameObject || hitCollider.gameObject.layer != WALL_LAYER || !hitCollider.GetComponent<WallBehaviour>().AllowsDimensionTransition)
                continue;

            var closestPointOnWall = hitCollider.ClosestPoint(transform.position); 

            float distance = Vector3.Distance(transform.position, closestPointOnWall);

            if (distance < closestDistance) {
                closestDistance = distance;
                closestObject = hitCollider.gameObject;
                // Get the closest point on the collider to the game object
                closestPoint = closestPointOnWall;

                //update transfer location and camera transition target for when the space bar is released 
                transferLocation = closestPoint + hitCollider.transform.up * WALL_DRAW_OFFSET;
                cameraTranstionTarget = transferLocation + cameraOffset2D * hitCollider.transform.up;

            }

        }

        return closestObject;
    }
    //handles a manual transition to 2d
    private void HandleManualTransition() {
        //checks for player input with space
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (playerControllerScript.IsIn3D())
                PerformRaycast();
            else
                dog3D.SetActive(true);
        }
        //release space to initiate trasition to 2d
        if (Input.GetKeyUp(KeyCode.Space) && playerControllerScript.IsIn3D()) {
            if (canTransitionTo2D && projectionEntry.activeInHierarchy)
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
            if (hit.collider.gameObject.layer == WALL_LAYER) {
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
    private void MoveProjectionWithCamera() {
        Ray ray = new(transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastLength)) {
            if (hit.collider.gameObject.layer == WALL_LAYER) {
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
        projection.transform.SetPositionAndRotation(hit.point + positionAdjustment * WALL_DRAW_OFFSET, Quaternion.LookRotation(hit.normal, Vector3.up));

        //update transfer location and camera transition target for when the space bar is released 
        transferLocation = hit.point + positionAdjustment * WALL_DRAW_OFFSET;
        cameraTranstionTarget = transferLocation + cameraOffset2D * hit.collider.transform.up;
    }
    //disable all projections
    private void DisableProjections() {
        projectionEntry.SetActive(false);
        projectionNoEntry.SetActive(false);
        projectionOutOfRange.SetActive(false);
    }
    //perform the necessary steps to begin the transfer to 2d
    private void TransitionTo2D() {
        dog3D.SetActive(false);
        isTransitioningTo2D = true;
        //swap the projections to the colored active one if the auto mode is enabled 
        if (!RangedDOGEnabled) {
            projectionEntry.transform.position = projectionOutOfRange.transform.position;
            projectionEntry.transform.forward = projectionOutOfRange.transform.forward;

            projectionEntry.SetActive(true);
            projectionOutOfRange.SetActive(false);

        }

        spritePosition = projectionEntry.transform.position;


        //toggle camera rotation controls and tell player script to swap to 2d movement
        
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
