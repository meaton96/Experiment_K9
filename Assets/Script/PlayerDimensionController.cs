using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

public class PlayerDimensionController : MonoBehaviour {
    //  public const int WALL_LAYER = 6;
    public const float WALL_DRAW_OFFSET = .21f;

    [SerializeField] private float raycastLength = 50f;
    [SerializeField] private float wall_intersect_radius_2d = 10f;
    [SerializeField] private GameObject projectionEntry;
    [SerializeField] private GameObject projectionNoEntry;
    [SerializeField] private GameObject player3D;
    [SerializeField] private GameObject player2D;
    [SerializeField] private MovementController_2D movementController_2D;
    [SerializeField] private Collider dog2DHitbox;

    [SerializeField] private GameObject Camera3D;
    [SerializeField] private GameObject Camera2D;


    private float cameraTransitionSpeed = 4f;
    private float projectionDrawRadius;

    private float wallDrawOffset = WALL_DRAW_OFFSET;
    //flag for Ranged version of device where you hold down space and release to transfer
    public bool RangedDOGEnabled = false;



    [SerializeField] private CameraControllerBeta cameraControllerScript;
    private PlayerBehaviour playerController;
    [SerializeField] private InterfaceBehaviour interfaceScript;

    private bool canTransitionTo2D = false;
    private Vector3 spritePosition;
    private bool isTransitioningTo2D = false;
    private bool isTransitioningTo3D = false;

    //speed at which the 2d projection moves when rotating the 3d camera
    [SerializeField] private float projectionMoveSpeed = 1.0f;

    //for flying the camera to the 2d sprite
    public static float cameraOffset2D = 70f;
    private Vector3 cameraTranstionTarget;
    private Vector3 projectionTransferLocation;

    private KeyControl DOGToggleKey;

    public bool IsProjecting = false;



    //vars for toggle mode DOG

    //flag for if the DOG is toggled on or not
    public bool DOGEnabled = true;

    public float DOGProjectionRange = 25f;
    [SerializeField] private GameObject projectionOutOfRange;


    //3d->2d
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    private void Start() {
        playerController = GetComponent<PlayerBehaviour>();
        interfaceScript.SetDogToggleText(RangedDOGEnabled);
        DOGToggleKey = Keyboard.current.fKey;
       // Debug.Log(wallDrawOffset);
        //store the size of the projection for drawing objects around it
        //   projectionDrawRadius = projectionOutOfRange.GetComponent<SpriteRenderer>().sprite.rect.height;
    }

    private void Update() {


        //if (isTransitioningTo2D) {
        //    TransitionCameraTo2D();
        //}
        //else if (isTransitioningTo3D) {
        //    TransitionCameraTo3D();
        //}
        //else if (Keyboard.current.f1Key.wasPressedThisFrame) {
        //    //only allow mode swapping while in 3d for now
        //    if (playerController.IsIn3D())
        //        SwapDOGModes();
        //}
        //else {

        //    //manual dog mode
        //    if (RangedDOGEnabled) {
        //        HandleManualModeAndCamera();
        //    }
        //    //auto mode
        //    else {
        //        HandleAutoDOG();
        //    }
        //}
        if (playerController.IsIn3D())
            HandleAutoModeInput();
    }


    public void EnableProjection(Collider collider, Vector3 position) {
        if (!IsProjecting) {
            //get the closest point on the outside of the hitbox to draw the 2d dog
            //  var pos = position;

            //offset the drawing a bit
            //goal should be to set it just outside the moveable wall collider 
            position += collider.transform.up * wallDrawOffset;

            IsProjecting = true;

            //move 2d player to this position

            player2D.transform.position = position;
            player2D.transform.forward = collider.transform.up;

            Set2DSprite(collider);


            player2D.SetActive(true);




            ////update transfer location and camera transition target 

            projectionTransferLocation = player2D.transform.position;
            cameraTranstionTarget = projectionTransferLocation + cameraOffset2D * collider.transform.up;
        }
        else {
            //UpdateProjectionPosition(collider);
            //handle potentially changing the projection to the other wall
        }
    }
    void Set2DSprite(Collider collider) {
        if (collider.TryGetComponent(out WallBehaviour wallB)) {
            //player is allowed to transition to the wall
            if (wallB.AllowsDimensionTransition) {
                //player can transition and is holding an object
                if (playerController.IsHoldingObject) {
                    movementController_2D.SetProjectionState(MovementController_2D.ProjectionState.HoldingObject);
                }
                else {
                    movementController_2D.SetProjectionState(MovementController_2D.ProjectionState.OutOfRange);
                }
            }
            //player cant transition
            else {
               //dont show anything
               DisableProjections();
            }
        }
    }
    //activate the 2d player
    void SetWallProjectionToActive() {
        if (playerController.IsHoldingObject) {
            movementController_2D.SetProjectionState(MovementController_2D.ProjectionState.In2DHoldingObject);
        }
        else {
            movementController_2D.SetProjectionState(MovementController_2D.ProjectionState.In2D);
        }
    }
    public void UpdateProjectionPosition(Collider collider, Vector3 position) {
        position += collider.transform.up * wallDrawOffset;
        
        //perform a physics overlap test to see if the space is free of walls that arent transferable
        var boxHits = Physics.OverlapBox(position, dog2DHitbox.bounds.extents, Quaternion.identity, LayerMask.GetMask("Walls"));

        //iterate through anything that was hit
        if (boxHits.Length > 0) {
            foreach (var hit in boxHits) {
                
                //make sure its a wall
                if (hit.TryGetComponent(out WallBehaviour wallB)) {
                    //check if the wall doesnt allow transitioning or walking
                    if (!wallB.AllowsDimensionTransition || !wallB.IsWalkThroughEnabled) {
                        //disable the projects and quit out of the method
                        DisableProjections();
                        return;
                    }
                }
            }
        }


        player2D.transform.position = position;
        player2D.transform.forward = collider.transform.up;

        Set2DSprite(collider);

        projectionTransferLocation = player2D.transform.position;
        cameraTranstionTarget = projectionTransferLocation + cameraOffset2D * collider.transform.up;
    }
    public void TransitionTo2D() {
        SetWallProjectionToActive();
        player3D.SetActive(false);
        playerController.ChangeDimension();

      //  Camera2D.transform.position = player2D.transform.position + player2D.transform.forward * cameraOffset2D;

        Camera3D.SetActive(false);
        Camera2D.SetActive(true);
        //tell the movement controller to lock axes
        movementController_2D.ProcessAxisChange();



    }


    //private void HandleManualModeAndCamera() {
    //    HandleManualMode();

    //    //update the position of the projected sprite if the camera is being rotated
    //    if (DOGToggleKey.isPressed && playerController.IsIn3D()) {
    //        MoveProjectionWithCamera();
    //    }
    //}
    //swap between manual and auto mode
    private void SwapDOGModes() {
        RangedDOGEnabled = !RangedDOGEnabled;
        interfaceScript.SetDogToggleText(RangedDOGEnabled);
        DisableProjections();

    }
    //handles collisions
    //private void OnCollisionEnter(Collision collision) {
    //    //only handle 3d collisions while in 3d mode
    //    if (playerController.IsIn3D()) {
    //        //handle a collision with a wall to potentially trnasfer to 2d
    //        if (collision.collider.gameObject.layer == LayerInfo.WALL) {
    //            if (collision.collider.GetComponent<WallBehaviour>().AllowsDimensionTransition && DOGEnabled) {
    //                TransitionTo2D();
    //            }
    //        }
    //    }
    //}
    #region Auto Mode
    //handles the auto mode of the Dimension device
    //public void HandleAutoDOG() {
    //    //check for player input when in Auto mode (currently enable/disable device)
    //    HandleAutoModeInput();

    //    //get wall and projection draw location (closest point to dog on the wall)
    //    if (playerController.IsIn3D()) {
    //        Handle3DAutoDOG();
    //    }
    //    else {
    //        Handle2DAutoDog();
    //    }
    //}
    private void HandleObjectProjection(GameObject nearestWall, Vector3 projectionDrawCenter) {

        //  playerController.HeldObject.ProjectOntoWallAtLocation(nearestWall, projectionDrawCenter, projectionDrawRadius, WALL_DRAW_OFFSET);
    }
    //private void Handle3DAutoDOG() {


    //    //only check for walls and stuff is the device is enabled
    //    if (DOGEnabled) {
    //        GameObject nearestWall;
    //        //get the nearest wall location
    //        //also store the location on the wall where we wawnt to draw the projection
    //        if ((nearestWall = FindNearbyWall(out Vector3 projectionDrawLocation)) != null) {
    //            //check if the wall allows dimension transfer before drawing the projection
    //            if (nearestWall.GetComponent<WallBehaviour>().AllowsDimensionTransition) {

    //                if (projectionOutOfRange.activeInHierarchy == false) {

    //                    //activate and set position and rotation of the projection
    //                    projectionOutOfRange.transform.position = projectionDrawLocation;
    //                    projectionOutOfRange.transform.forward = nearestWall.transform.up;
    //                    projectionOutOfRange.SetActive(true);
    //                    IsProjecting = true;
    //                }
    //                //always move the projection while it is enabled 
    //                projectionOutOfRange.transform.position = projectionDrawLocation + (nearestWall.transform.up * WALL_DRAW_OFFSET);
    //                projectionOutOfRange.transform.forward = nearestWall.transform.up;

    //                //if the player is holding an item, project it onto the wall
    //                if (playerController.IsHoldingObject)
    //                    HandleObjectProjection(nearestWall, projectionDrawLocation);
    //            }
    //            else {
    //                //non transferable wall is the closest
    //                DisableProjections();
    //            }

    //        }
    //        else {
    //            //disable projection if a wall wasnt found
    //            DisableProjections();

    //        }
    //    }
    //}
    //private void Handle2DAutoDog() {

    //    if (!DOGEnabled) {
    //        Collider[] hitColliders = Physics.OverlapSphere(transform.position, wall_intersect_radius_2d);


    //        foreach (var hitCollider in hitColliders) {
    //            if (hitCollider.gameObject == gameObject || hitCollider.gameObject.layer != LayerInfo.WALL) {
    //                continue;
    //            }
    //            //   Debug.Log(hitCollider.gameObject.name);
    //            if (hitCollider.GetComponent<WallBehaviour>().AllowsDimensionTransition) {
    //                TransitionTo3D();
    //                return;
    //            }
    //        }
    //    }
    //}

    //handle enable/disasble of DOG device while in auto mode
    private void HandleAutoModeInput() {
        if (DOGToggleKey.wasPressedThisFrame) {
            DOGEnabled = !DOGEnabled;
            interfaceScript.SetDogAutoEnabledText(DOGEnabled);
            if (playerController.IsIn3D())
                DisableProjections();
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
            if (hitCollider.gameObject == gameObject || hitCollider.gameObject.layer != LayerInfo.WALL)
                continue;

            var closestPointOnWall = hitCollider.ClosestPoint(transform.position);

            float distance = Vector3.Distance(transform.position, closestPointOnWall);

            if (distance < closestDistance) {
                closestDistance = distance;
                closestObject = hitCollider.gameObject;
                // Get the closest point on the collider to the game object
                closestPoint = closestPointOnWall;

                //update transfer location and camera transition target for when the player walks into the wall
                projectionTransferLocation = closestPoint + hitCollider.transform.up * wallDrawOffset;
                cameraTranstionTarget = projectionTransferLocation + cameraOffset2D * hitCollider.transform.up;



            }

        }

        return closestObject;
    }
    #endregion

    #region Manual Mode
    //handles a manual transition to 2d
    private void HandleManualMode() {
        if (playerController.IsIn3D())
            HandleManualMode3D();
        else
            HandleManualMode2D();

    }

    private void HandleManualMode2D() {
        if (DOGToggleKey.wasPressedThisFrame) {
            TransitionTo3D();
        }
    }

    private void HandleManualMode3D() {
        //checks for player input with space
        if (DOGToggleKey.isPressed) {
            PerformRaycast();
        }
        //release space to initiate trasition to 2d
        if (DOGToggleKey.wasReleasedThisFrame && playerController.IsIn3D()) {
            if (canTransitionTo2D && projectionEntry.activeInHierarchy) { }
              //  TransitionTo2D();
            else
                DisableProjections();
        }
    }

    //performs a raycast in the transform's forward direction
    private void PerformRaycast() {
        Ray ray = new(transform.position, transform.forward);

        //hit
        if (Physics.Raycast(ray, out RaycastHit hit, raycastLength)) {
            //check if its a wall
            //all walls need to be layer 6 for this to work
            if (hit.collider.gameObject.layer == LayerInfo.WALL) {
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
            if (hit.collider.gameObject.layer == LayerInfo.WALL) {
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
    #endregion

    //enable one of the projections
    private void EnableProjection(GameObject projection, RaycastHit hit) {

        IsProjecting = true;

        Debug.Log("enabling projection " + IsProjecting);

        projection.SetActive(true);

        //get the direction to the camera and create an offset based on it
        Vector3 directionToCamera = (Camera.main.transform.position - hit.point).normalized;
        Vector3 positionAdjustment = new(
            x: Mathf.Abs(transform.forward.z) * directionToCamera.x,
            0,
            z: Mathf.Abs(transform.forward.x) * directionToCamera.z
        );
        //set the rotation and position of the projection
        projection.transform.SetPositionAndRotation(hit.point + positionAdjustment * wallDrawOffset, Quaternion.LookRotation(hit.normal, Vector3.up));

        //update transfer location and camera transition target 

        projectionTransferLocation = projection.transform.position;
        cameraTranstionTarget = projectionTransferLocation + cameraOffset2D * hit.collider.transform.up;
    }
    //disable all projections
    public void DisableProjections() {
        if (IsProjecting) {
            //  projectionEntry.SetActive(false);
            //  projectionNoEntry.SetActive(false);
            //   projectionOutOfRange.SetActive(false);
            player2D.SetActive(false);

         //   Debug.Log("Resetting projections");
            if (playerController.IsHoldingObject) {
                playerController.HeldObject.DisableProjection();
            }
            IsProjecting = false;
        }



    }
    #region 2D<->3D Transitions
    //perform the necessary steps to begin the transfer to 2d
    //private void TransitionTo2D() {

    //    // playerController.ToggleMovement();
    //    player2D.SetActive(false);
    //    isTransitioningTo2D = true;
    //    //swap the projections to the colored active one if the auto mode is enabled 
    //    if (!RangedDOGEnabled) {
    //        projectionEntry.transform.position = projectionOutOfRange.transform.position;
    //        projectionEntry.transform.forward = projectionOutOfRange.transform.forward;

    //        projectionEntry.SetActive(true);
    //        projectionOutOfRange.SetActive(false);

    //    }

    //    spritePosition = projectionEntry.transform.position;


    //    //toggle camera rotation controls and tell player script to swap to 2d movement
    //    cameraControllerScript.ToggleCameraRotation(false);

    //    if (playerController.IsHoldingObject) {
    //        var heldObject = playerController.HeldObject;
    //        if (heldObject != null) {
    //            //heldObject.Enable2D();
    //            heldObject.Disable3D();
    //        }
    //    }

    //}
    private void TransitionTo3D() {
        Debug.Log("transitioning to 3D");
        //Toggle movement to disable player controls during the transition
        // playerController.ToggleMovement();

        //Set the transitioning flag
        isTransitioningTo3D = true;

        //Enable camera rotation
        cameraControllerScript.ToggleCameraRotation(true);

        //Re-enable the 3D model
        player2D.SetActive(true);

        //Disable the 2D projection
        projectionEntry.SetActive(false);

        //disable 2d movement hitbox
        //  dog2DHitbox.SetActive(false);

        //calculate camera position
        originalCameraPosition = Camera.main.GetComponent<CameraControllerBeta>().GetUpdatedCameraPosition();
        originalCameraRotation = Quaternion.LookRotation(transform.position - originalCameraPosition, Vector3.up);

        if (playerController.IsHoldingObject) {
            var heldObject = playerController.HeldObject;
            if (heldObject != null) {
                heldObject.Enable3D();
                heldObject.Disable2D();
            }
        }

    }

    //smooth pan camera to sprite location
    private void TransitionCameraTo2D() {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraTranstionTarget, cameraTransitionSpeed * Time.deltaTime);

        Vector3 lookDirection = spritePosition - Camera.main.transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, rotation, cameraTransitionSpeed * Time.deltaTime);

        if (Vector3.Distance(Camera.main.transform.position, cameraTranstionTarget) < 0.15f) {
            isTransitioningTo2D = false;

            playerController.ChangeDimension();
            // playerController.ToggleMovement();
            //  dog2DHitbox.SetActive(true);    //enable 2d movement hitbox as last step to avoid double collision

        }


    }
    private void TransitionCameraTo3D() {
        Camera.main.transform.SetPositionAndRotation(
            Vector3.Lerp(
                Camera.main.transform.position,
                originalCameraPosition,
                cameraTransitionSpeed * Time.deltaTime),
            Quaternion.Slerp(Camera.main.transform.rotation,
                originalCameraRotation,
                cameraTransitionSpeed * Time.deltaTime));

        // Check if the transition is complete
        if (Vector3.Distance(Camera.main.transform.position, originalCameraPosition) < 0.15f) {
            isTransitioningTo3D = false;  // Clear the transitioning flag

            //Complete the transition by re-enabling player controls
            playerController.ChangeDimension();
            //  playerController.ToggleMovement();
            cameraControllerScript.ToggleCameraRotation(true);
        }
    }

    #endregion

}
