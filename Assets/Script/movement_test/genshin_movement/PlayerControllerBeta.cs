using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerControllerBeta : MonoBehaviour {

    //   public const int GROUND_LAYER = 8;                      //layer all ground objects should be on for gravity
    public float moveSpeed3D = 5.0f;                        //movement speed while in 3D        

    public float rotationSpeed = 400f;
    public bool oldrotation = false;
    public bool newrotation = true;
    public bool testrotation = false;//movement speed while in 2D
    public Transform cameraTransform;                       //transform of the main camera
    private bool is3D = true;                               //handles checking if the player is in 3d or 2d mode
    private bool canMove = true;                            //disable or enable player movement
    private bool canInteract = true;                        //disable or enable player interactions
    Vector3 position;
    private Vector3 moveDirection;
    public GameObject player2D;           //holds the 2d depiction of the player

    [SerializeField] private float interactDisplayRadius = 20f; //radius of the collider to determine the range at which the player can interact
    [SerializeField] private GameObject interactRadar;          //holds the game object that has the radar collider on it

    private KeyControl interactKey;     //which key to use for interaction, set in Start()

    private List<TransferableObject> objectsInInteractRange;    //a list of all the objects that are in interactable range
    private Rigidbody rigidBody;                                //holds the player's rigid body

    [HideInInspector] public bool IsHoldingObject = false;                    //if the player has something in their hands or not
    [HideInInspector] public TransferableObject HeldObject;                   //the object the player is hold
    private float rotation;
    private bool isTouchingGround;                          //if the player is on the ground, to enable movement logic
                                                            //   private CharacterController characterController;

    public const float GRAVITY = 981f;
    public const float JUMP_FORCE = 25f;

    private void Start() {
        interactKey = Keyboard.current.eKey;
        interactRadar.GetComponent<SphereCollider>().radius = interactDisplayRadius;
        objectsInInteractRange = new();
        rigidBody = GetComponent<Rigidbody>();
        // characterController = GetComponent<CharacterController>();
    }

    void Update() {
        if (canInteract) {

            //remove
            //Debug.Log(transform.forward);

            HandleInteractionInput();
        }
        if (canMove) {
            if (is3D) {
                Move3D();
            }
        }
    }
    #region Player Movement Controls
    //handles movement in 3d mode
    void Move3D() {
        //Debug.Log(isTouchingGround);
        //float ground = transform.position.y;
        //only allow move while touching the ground
        position = transform.position;
        Vector2 input = GetInput();

        if (input != Vector2.zero) {  // Check if there's any input
            Vector3 cameraForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            Vector3 cameraRight = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

            Vector3 direction = cameraForward * input.y + cameraRight * input.x;
            if (oldrotation == true) {
                transform.forward = direction.normalized;  // Only set forward direction if there is input
                transform.position += moveSpeed3D * Time.deltaTime * direction;
                newrotation = false;
            }
            else if (newrotation == true) {
                //Vector3 rotate = direction.normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

                // Interpolate between the current rotation and the desired rotation
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Update player position
                position += moveSpeed3D * Time.deltaTime * direction;
                transform.position = position;

            }
            else {

                //transform.Rotate(0, direction.normalized, 0);
                //transform.forward = direction.normalized;
                var move = Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0;
                var rotate = Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0;

                // Calculate the movement direction

                moveDirection = move * moveSpeed3D * transform.TransformDirection(direction);
                //moveDirection.y = 4.59f;
                rotation = rotate
                    * rotationSpeed
                    * Time.deltaTime;
                transform.Rotate(0, rotation, 0);
                // transform.forward= Quaternion.RotateTowards(0, rotation, 0);
                position += move * moveSpeed3D * Time.deltaTime * transform.forward;
                transform.position = position;
                //characterController.Move(moveDirection * Time.deltaTime);

            }
        }

        if (isTouchingGround) {
            //jump
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                rigidBody.AddForce(JUMP_FORCE * rigidBody.mass * Vector3.up, ForceMode.Impulse); //For some reason ForceMode.Impulse must be used here, not ForceMode.Force
        }
        else
            rigidBody.AddForce(GRAVITY * Vector3.down, ForceMode.Force);
    }

    //helper method to grab keyboard input in 3d, checks for WASD presses
    public Vector2 GetInput() {
        var keyboard = Keyboard.current;
        return new Vector2(keyboard.dKey.isPressed ? 1 : keyboard.aKey.isPressed ? -1 : 0,
                           keyboard.wKey.isPressed ? 1 : keyboard.sKey.isPressed ? -1 : 0);
    }

    //swap between dimensions
    public void ChangeDimension() {

        is3D = !is3D;
        rigidBody.isKinematic = !is3D;
        if (is3D && HeldObject != null) {
            HeldObject.transform.localPosition = HeldObject.HoldOffset3D;
        }

    }
    //enable/disable movement logic 
    public void ToggleMovement() {
        canMove = !canMove;
    }
    #endregion

    //returns true if the game is in 3d mode
    public bool IsIn3D() { return is3D; }

    //only allows one copy of each object
    public void AddObjectToInRangeList(TransferableObject tObject) {
        
        if (objectsInInteractRange.Contains(tObject)) return;
        
        objectsInInteractRange.Add(tObject);
        
    }
    public void RemoveObjectFromRangeList(TransferableObject tObject) {
        
        objectsInInteractRange.Remove(tObject);
    }

    //handles player interaction with interactable objects
    private void HandleInteractionInput() {

        if (interactKey.wasPressedThisFrame) {

            //if the player is already holding something then drop it
            if (IsHoldingObject) {
                if (IsIn3D()) {
                    HeldObject.Drop3D();
                    IsHoldingObject = false;
                    HeldObject = null;
                }
                else {
                    HeldObject.Drop2D();
                    IsHoldingObject = false;
                    HeldObject = null;
                }
            }
            //only process interact press if theres something to interact with
            else {
                if (IsIn3D()) {
                    Handle3DInteractions();
                }
                else {

                    Handle2DInteractions();
                }


            }

        }
    }
    //handle picking up objects while in 2d
    private void Handle2DInteractions() {
        var tObject = GetObjectClosestTo2DPlayer();
        Debug.Log(tObject == null);
        if (tObject != null && !tObject.Is3D) {
            HeldObject = tObject;
            //pick up the object that was found to be the closest
            HeldObject.Pickup2D(gameObject);
            IsHoldingObject = true;
        }
    }
    //handle picking up 3d objects while in 3d 
    private void Handle3DInteractions() {
        var tObject = GetObjectClosestToCameraLookAt();
        //only process interactions with 3d objects while in 3d
        if (tObject != null && tObject.Is3D) {
            HeldObject = tObject;
            //pick up the object that was found to be the closest
            HeldObject.Pickup3D(gameObject);
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

        if (!objectsInInteractRange.Any()) {
            return null;
        }
        float closestToPlayer = float.MaxValue;
        TransferableObject tObject = null;

        foreach (var obj in objectsInInteractRange) {
            var vecToObject = obj.transform.position - transform.position;
            var length = vecToObject.magnitude;
            if (length < closestToPlayer) {
                closestToPlayer = length;
                tObject = obj;
            }
        }
        return tObject;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == LayerInfo.GROUND) {
            isTouchingGround = true;
        }
    }
    private void OnCollisionExit(Collision collision) {
        if (collision.collider.gameObject.layer == LayerInfo.GROUND) {
            isTouchingGround = false;
        }
    }

}

