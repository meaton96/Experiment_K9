using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PickupController : MonoBehaviour {
    [Header("Pickup Settings")]
    [SerializeField] Transform holdArea;
    [HideInInspector] public GrabbableObject HeldObject;
    [SerializeField] private PlayerBehaviour playerBehaviour;
    private Rigidbody heldObjectRigidbody;

    [Header("Physics Params")]
    [SerializeField] float pickupRange = 5f;
    [SerializeField] float pickupForce = 150f;

    KeyControl interactKey;

    private List<GrabbableObject> objectsInInteractRange;    //a list of all the objects that are in interactable range

    private Vector3 forceDir;
    private void Awake() {
        interactKey = Keyboard.current.eKey;
        objectsInInteractRange = new();
    }


    private void Update() {
        HandleInteractionInput();
    }


    private void HandleInteractionInput() {

        if (interactKey.wasPressedThisFrame) {

            //if the player is already holding something then drop it
            if (IsHoldingObject()) {
                DropHeldObject();

            }
            //only process interact press if theres something to interact with
            else {
                PickupObject();
            }
        }
        if (IsHoldingObject()) {
            MoveObject();
        }
    }
    public bool IsHoldingObject() {
        return HeldObject != null;
    }
    private void PickupObject() {
        if (playerBehaviour.IsIn3D()) {
            Handle3DInteractions();
        }
        else {
            Pickup2DObject();
        }
    }
    private void MoveObject() {
        //if (Vector3.Distance(HeldObject.transform.position, holdArea.position) > 0.1f) {
        //    var moveDirection = holdArea.position - HeldObject.transform.position;
        //    forceDir = moveDirection * pickupForce;
        //    heldObjectRigidbody.AddForce(moveDirection * pickupForce);
        //}
        if (transform.localPosition.magnitude > 0.2f && !HeldObject.isColliding) {

            var moveDirection = holdArea.position - heldObjectRigidbody.transform.position;
            heldObjectRigidbody.AddForce(moveDirection * pickupForce);
            Debug.Log("adding force " + moveDirection * pickupForce);
        }
        else if (HeldObject.isColliding) {
            Debug.Log("colliding so no force applied");
        }
    }

    private void DropHeldObject() {

        HeldObject.DropObject();
        HeldObject = null;
        heldObjectRigidbody = null;

        //set the sprite to not holding the object
        if (!playerBehaviour.IsIn3D()) {
            playerBehaviour.player2DMovementController.SetProjectionState(MovementController_2D.ProjectionState.In2D);
        }
    }

    public void ChangeDimension() {
        if (IsHoldingObject() && HeldObject is TransferableObject) {
            var tObject = HeldObject as TransferableObject;
            HeldObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            //swap parent and add offset when moving back to 3D with object
            if (playerBehaviour.IsIn3D()) {
                tObject.SetHolderAndOffset(gameObject, HeldObject.HoldOffset3D);

                tObject.Enable3D();
            }
            else {
                tObject.SetHolderAndOffset(playerBehaviour.player2D, Vector3.zero);
                tObject.Disable3D();
            }
        }
        else if (IsHoldingObject()) {
            DropHeldObject();
        }
    }

    //handle picking up objects while in 2d
    private void Pickup2DObject() {
        var tObject = GetObjectClosestTo2DPlayer();

        if (tObject != null && !tObject.Is3D) {
            HeldObject = tObject;
            //pick up the object that was found to be the closest
            (HeldObject as TransferableObject).Pickup2D(playerBehaviour.player2D);
            playerBehaviour.player2DMovementController.SetProjectionState(MovementController_2D.ProjectionState.In2DHoldingObject);

        }
    }
    //handle picking up 3d objects while in 3d 
    private void Handle3DInteractions() {
        var tObject = GetObjectClosestToCameraLookAt();
        //only process interactions with 3d objects while in 3d
        if (tObject != null && tObject.Is3D) {

            HeldObject = tObject;
            heldObjectRigidbody = HeldObject.displayObject3D_Mesh.GetComponent<Rigidbody>();
            var moveDirection = holdArea.position - heldObjectRigidbody.transform.position;
            heldObjectRigidbody.AddForce(moveDirection * pickupForce);
            //pick up the object that was found to be the closest
            HeldObject.Pickup3D(gameObject, holdArea);
        }
    }
    //returns the interactable object closest to where the player is looking at with the camera
    private GrabbableObject GetObjectClosestToCameraLookAt() {
        if (!objectsInInteractRange.Any()) {
            return null;
        }
        float closestToCameraLookDirection = float.MaxValue;
        GrabbableObject gObject = null;
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
                gObject = obj;
            }
        }
        return gObject;
    }

    //only allows one copy of each object
    public void AddObjectToInRangeList(GrabbableObject tObject) {


        if (objectsInInteractRange.Contains(tObject)) return;

        objectsInInteractRange.Add(tObject);

    }

    public void RemoveObjectFromRangeList(GrabbableObject tObject) {

        objectsInInteractRange.Remove(tObject);
    }
    //returns the object to pick up that is closest to the player transform
    //this behaviour might want to be changed later
    private TransferableObject GetObjectClosestTo2DPlayer() {

        var objectsInRange = Physics.OverlapSphere(playerBehaviour.player2D.transform.position, playerBehaviour.interactDisplayRadius, LayerMask.GetMask("Interactable Objects"));

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


    public void ClearList() {
        objectsInInteractRange.Clear();
    }


}
