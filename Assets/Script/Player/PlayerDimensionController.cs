using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

public class PlayerDimensionController : MonoBehaviour {
    public const float WALL_DRAW_OFFSET = .21f;

    [SerializeField] private GameObject player3D;
    [SerializeField] private GameObject player2D;
    [SerializeField] private MovementController_2D movementController_2D;
    [SerializeField] private Collider dog2DHitbox;
    [SerializeField] private ThirdPersonController controller3D;

    [SerializeField] private GameObject Camera3D;
    [SerializeField] private GameObject Camera2D;
    [SerializeField] private float playerLeaveWallOffset = 6f;
    [SerializeField] private InteractRadarController radar;

    private float wallDrawOffset = WALL_DRAW_OFFSET;
    //flag for Ranged version of device where you hold down space and release to transfer
    public bool RangedDOGEnabled = false;

    private PlayerBehaviour playerBehaviour;
    [SerializeField] private InterfaceBehaviour interfaceScript;


    private KeyControl DOGToggleKey;
    private KeyControl DOGLeaveKey;
    private KeyControl pauseKey;

    public bool IsProjecting = false;

    //vars for toggle mode DOG

    //flag for if the DOG is toggled on or not
    public bool DOGEnabled = true;

    public float DOGProjectionRange = 25f;
    [SerializeField] private GameObject projectionOutOfRange;
    private bool paused = false;



    private void Awake() {
        playerBehaviour = GetComponent<PlayerBehaviour>();
        interfaceScript.SetDogToggleText(RangedDOGEnabled);
        DOGToggleKey = Keyboard.current.fKey;
        DOGLeaveKey = Keyboard.current.spaceKey;
        pauseKey = Keyboard.current.escapeKey;

    }
    private void Update() {

        HandlePauseInput();
        HandleAutoModeInput();
    }
    private void HandlePauseInput() {
        if (pauseKey.wasPressedThisFrame) {
            paused = !paused;
            playerBehaviour.SetPaused(paused);
            controller3D.SetPaused(paused);
            if (paused) {
                Time.timeScale = 0;
            }
            else {
                Time.timeScale = 1;
            }
            
            if (paused)
                interfaceScript.Pause();
            else
                interfaceScript.UnPause();
        }
    }
    public void EnableProjection(Collider collider, Vector3 position) {
        if (!IsProjecting || player2D.activeSelf == false) {
            //offset the drawing a bit
            //goal should be to set it just outside the moveable wall collider 
            position += collider.transform.up * wallDrawOffset;

            IsProjecting = true;

            //move 2d player to this position
            player2D.transform.position = position;
            player2D.transform.forward = collider.transform.up;

            Set2DSprite(collider);
            player2D.SetActive(true);
        }
        else {

            //handle potentially changing the projection to the other wall
        }
    }
    void Set2DSprite(Collider collider) {
        if (collider.TryGetComponent(out WallBehaviour wallB)) {
            //player is allowed to transition to the wall
            if (wallB.AllowsDimensionTransition) {
                //player can transition and is holding an object
                if (playerBehaviour.IsHoldingObject) {
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
        if (playerBehaviour.IsHoldingObject) {
            movementController_2D.SetProjectionState(MovementController_2D.ProjectionState.In2DHoldingObject);
        }
        else {
            movementController_2D.SetProjectionState(MovementController_2D.ProjectionState.In2D);
        }
    }
    public void UpdateProjectionPosition(Collider collider, Vector3 position) {
        position += collider.transform.up * wallDrawOffset;

        //perform a physics overlap test to see if the space is free of walls that arent transferable
        var boxHits = Physics.OverlapBox(position, dog2DHitbox.bounds.extents, Quaternion.identity, LayerMask.GetMask("Walls", "Doors"));


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
                //door was hit
                else {
                    DisableProjections();
                }
            }
        }

        player2D.transform.position = position;
        player2D.transform.forward = collider.transform.up;

        Set2DSprite(collider);
    }
    public void TryTransitionTo2D() {
        if (movementController_2D.IsProjectionSpaceClear(transform.position)) {
            TransitionTo2D();
        }
        else {
            Debug.Log("Transition area blocked");
        }
    }

    private void TransitionTo2D() {
        movementController_2D.GetComponent<Rigidbody>().isKinematic = false;
        SetWallProjectionToActive();
        player3D.SetActive(false);

        playerBehaviour.ChangeDimension();


        Camera3D.SetActive(false);
        Camera2D.SetActive(true);
        //tell the movement controller to lock axes
        movementController_2D.ProcessAxisChange();
        if (player3D.TryGetComponent(out StarterAssetsInputs sAssetsInput)) {
            sAssetsInput.ClearInput();
        }

    }
    public void TransitionTo3D() {


        //adjust the player 3d model to be in front of the wall offset by a small amount
        player3D.transform.position = player2D.transform.position + player2D.transform.forward * playerLeaveWallOffset;
        player2D.SetActive(false);
        playerBehaviour.ClearList();
        radar.clearsurfaces();
        //set its rotation so its not clipping into the wall hopefully
        player3D.transform.forward = player2D.transform.right;
        player3D.SetActive(true);
        playerBehaviour.ChangeDimension();
        Camera3D.SetActive(true);
        Camera2D.SetActive(false);
        if (player3D.TryGetComponent(out StarterAssetsInputs sAssetsInput)) {
            sAssetsInput.ClearInput();
        }

    }
    public void TransitionTo3DLaunch() {
        Vector3 launchDirection = player2D.transform.forward;

        //adjust the player 3d model to be in front of the wall offset by a small amount
        Vector3 launchPosition = player2D.transform.position + launchDirection * playerLeaveWallOffset;

        //  print(player3D.transform.position);
        player2D.SetActive(false);
        playerBehaviour.ClearList();
        radar.clearsurfaces();
        //set its rotation so its not clipping into the wall hopefully
        player3D.transform.position = launchPosition;
        player3D.transform.forward = player2D.transform.right;
        player3D.SetActive(true);
        playerBehaviour.ChangeDimension();
        Camera3D.SetActive(true);
        Camera2D.SetActive(false);
        if (player3D.TryGetComponent(out StarterAssetsInputs sAssetsInput)) {
            sAssetsInput.ClearInput();
        }
        Rigidbody player3DRigidbody = player3D.GetComponent<Rigidbody>();
        float launchForce = 10f;

        player3DRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);
        DOGEnabled = !DOGEnabled;
        player3DRigidbody.AddForce(launchDirection * 0, ForceMode.Impulse);
    }
    //handle enable/disasble of DOG device while in auto mode
    private void HandleAutoModeInput() {
        if (DOGLeaveKey.wasPressedThisFrame) {
            if (playerBehaviour.IsIn3D()) {
                Debug.Log("oof");
            }
            else {
                if (movementController_2D.CanTransitionOutOfCurrentWall()) {
                    DOGEnabled = !DOGEnabled;
                    TransitionTo3DLaunch();
                }
            }

        }
        if (DOGToggleKey.wasPressedThisFrame) {
            DOGEnabled = !DOGEnabled;
            interfaceScript.SetDogAutoEnabledText(DOGEnabled);
            if (playerBehaviour.IsIn3D()) {
                if (IsProjecting) {
                    DisableProjections();
                }
                else {
                    IsProjecting = true;
                }
            }
            else {
                if (movementController_2D.CanTransitionOutOfCurrentWall()) {

                    TransitionTo3D();
                }
            }
        }
    }
    //disable all projections
    public void DisableProjections() {
        if (IsProjecting) {
            player2D.SetActive(false);
            IsProjecting = false;

        }



    }
}
