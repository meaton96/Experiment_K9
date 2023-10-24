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
    public const float WALL_DRAW_OFFSET = .21f;

    [SerializeField] private GameObject player3D;
    [SerializeField] private GameObject player2D;
    [SerializeField] private MovementController_2D movementController_2D;
    [SerializeField] private Collider dog2DHitbox;

    [SerializeField] private GameObject Camera3D;
    [SerializeField] private GameObject Camera2D;
    [SerializeField] private float playerLeaveWallOffset = 6f;
    [SerializeField] private InteractRadarController radar;

    private float wallDrawOffset = WALL_DRAW_OFFSET;
    //flag for Ranged version of device where you hold down space and release to transfer
    public bool RangedDOGEnabled = false;

    private PlayerBehaviour playerController;
    [SerializeField] private InterfaceBehaviour interfaceScript;


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
    }
    private void Update() {


        HandleAutoModeInput();
    }
    public void EnableProjection(Collider collider, Vector3 position) {
        if (!IsProjecting|| player2D.activeSelf==false) {
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
       // print("this is updating");
       // player2D.SetActive(true);
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
       
        playerController.ChangeDimension();

        //  Camera2D.transform.position = player2D.transform.position + player2D.transform.forward * cameraOffset2D;

        Camera3D.SetActive(false);
        Camera2D.SetActive(true);
        //tell the movement controller to lock axes
        movementController_2D.ProcessAxisChange();
        
    }
    public void TransitionTo3D() {
       
        
        //adjust the player 3d model to be in front of the wall offset by a small amount
        player3D.transform.position = player2D.transform.position + player2D.transform.forward * playerLeaveWallOffset;
      //  print(player3D.transform.position);
        player2D.SetActive(false);
        playerController.ClearList();
        radar.clearsurfaces();
        //set its rotation so its not clipping into the wall hopefully
        player3D.transform.forward = player2D.transform.right;
        //radar.potentialProjectionSurfaces.Clear(); <----
        player3D.SetActive(true);
        playerController.ChangeDimension();
        Camera3D.SetActive(true);
        Camera2D.SetActive(false);
        //movementController_2D.GetComponent<Rigidbody>().isKinematic = true;

    }
    //handle enable/disasble of DOG device while in auto mode
    private void HandleAutoModeInput() {
        if (DOGToggleKey.wasPressedThisFrame) {
            DOGEnabled = !DOGEnabled;
            interfaceScript.SetDogAutoEnabledText(DOGEnabled);
          //  print("hitthefbutton");
            if (playerController.IsIn3D())
            {
                if (IsProjecting)
                {
                 //   print("disabling");
                    DisableProjections();
                }
                else
                {
                    IsProjecting = true;
                }
            }
            else
            {
               // print("trying to move");
                if (movementController_2D.CanTransitionOutOfCurrentWall())
                {
                    
                    TransitionTo3D();
                    //movementController_2D.currentWall = null;
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
