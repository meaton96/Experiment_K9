using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerBehaviour : MonoBehaviour {

    private bool is3D = true;                               //handles checking if the player is in 3d or 2d mode
  //  private bool canMove = true;                            //disable or enable player movement
    private bool canInteract = true;                        //disable or enable player interactions
   
    public GameObject player2D;                             //holds the 2d depiction of the player
    public MovementController_2D player2DMovementController;           //holds the 2d depiction of the player
    public GameObject player3D;                             //holds the 3d depiction of the player

    [SerializeField] private float interactDisplayRadius = 20f; //radius of the collider to determine the range at which the player can interact
    [SerializeField] private GameObject interactRadar;          //holds the game object that has the radar collider on it

    private KeyControl interactKey;                             //which key to use for interaction, set in Start()

    private List<TransferableObject> objectsInInteractRange;    //a list of all the objects that are in interactable range

    [HideInInspector] public bool IsHoldingObject = false;                    //if the player has something in their hands or not
    [HideInInspector] public TransferableObject HeldObject;                   //the object the player is hold



    private void Start() {
        interactKey = Keyboard.current.eKey;
        interactRadar.GetComponent<SphereCollider>().radius = interactDisplayRadius;
        objectsInInteractRange = new();
    }

    void Update() {
        if (canInteract) {

            HandleInteractionInput();
        }
    }
    
   

    //swap between dimensions
    public void ChangeDimension() {
        //TODO
        is3D = !is3D;
        // rigidBody.isKinematic = !is3D;
        //when leaving with a ball reset its local position

        if (IsHoldingObject) {
            HeldObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity); 
            //swap parent and add offset when moving back to 3D with object
            if (is3D) {
                HeldObject.SetHolderAndOffset(player3D, HeldObject.HoldOffset3D);
                
                HeldObject.Enable3D();
            }
            else {
                HeldObject.SetHolderAndOffset(player2D, Vector3.zero);
                HeldObject.Disable3D();
            }

            
        }

    }
   

    //returns true if the game is in 3d mode
    public bool IsIn3D() { return is3D; }

    //only allows one copy of each object
    public void AddObjectToInRangeList(TransferableObject tObject) {
        
        if (objectsInInteractRange.Contains(tObject)) return;
        
        objectsInInteractRange.Add(tObject);
        
    }
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(player2D.transform.position, interactDisplayRadius);
    }
    public void RemoveObjectFromRangeList(TransferableObject tObject) {
        
        objectsInInteractRange.Remove(tObject);
    }

    //handles player interaction with interactable objects
    private void HandleInteractionInput() {

        if (interactKey.wasPressedThisFrame) {

            //if the player is already holding something then drop it
            if (IsHoldingObject) {
                HeldObject.DropObject();
                IsHoldingObject = false;
                HeldObject = null;

                //set the sprite to not holding the object
                if (!is3D) {
                    player2DMovementController.SetProjectionState(MovementController_2D.ProjectionState.In2D);
                }

            }
            //only process interact press if theres something to interact with
            else {
                if (IsIn3D()) {
                    Handle3DInteractions();
                }
                else {

                    Pickup2DObject();
                }


            }

        }
    }
    //handle picking up objects while in 2d
    private void Pickup2DObject() {
        var tObject = GetObjectClosestTo2DPlayer();
        
        if (tObject != null && !tObject.Is3D) {
            HeldObject = tObject;
            //pick up the object that was found to be the closest
            HeldObject.Pickup2D(player2D);
            IsHoldingObject = true;
            player2DMovementController.SetProjectionState(MovementController_2D.ProjectionState.In2DHoldingObject);
            
        }
    }
    //handle picking up 3d objects while in 3d 
    private void Handle3DInteractions() {
        var tObject = GetObjectClosestToCameraLookAt();
        //only process interactions with 3d objects while in 3d
        if (tObject != null && tObject.Is3D) {
            HeldObject = tObject;
            //pick up the object that was found to be the closest
            HeldObject.Pickup3D(player3D);
            IsHoldingObject = true;
        }
    }
    //returns the interactable object closest to where the player is looking at with the camera
    private TransferableObject GetObjectClosestToCameraLookAt() {
        if (!objectsInInteractRange.Any()) {
            return null;
        }
        float closestToCameraLookDirection = float.MaxValue;
        TransferableObject tObject = null;
        //iterate each object
        foreach (var obj in objectsInInteractRange) {
            //get the vector from the object to the main camera
            var vecToObject = obj.transform.position - Camera.main.transform.position;
            //use the dot product to project the vector onto the camera's right axis
            var dist = Mathf.Abs(
                Vector3.Dot(
                    vecToObject,
                    Camera.main.transform.right));
            //compare the distance to camera and find the smallest one
            if (dist < closestToCameraLookDirection) {
                closestToCameraLookDirection = dist;
                tObject = obj;
            }
        }
        return tObject;
    }
    //returns the object to pick up that is closest to the player transform
    //this behaviour might want to be changed later
    private TransferableObject GetObjectClosestTo2DPlayer() {

        var objectsInRange = Physics.OverlapSphere(player2D.transform.position, interactDisplayRadius, LayerMask.GetMask("Interactable Objects"));

        if (!objectsInRange.Any()) return null;

        float closestToPlayer = float.MaxValue;
        TransferableObject tObject = null;

        foreach (var objectCollider in objectsInRange) {
            if (objectCollider.transform.parent.TryGetComponent(out TransferableObject transferObject)) {
                var vecToObject = objectCollider.transform.position - transform.position;
                var length = vecToObject.sqrMagnitude;
                if (length < closestToPlayer) {
                    closestToPlayer = length;
                    tObject = transferObject;
                }
            }
        }
        return tObject;
    }

    



}
