using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerControllerBeta : MonoBehaviour {

    public const int GROUND_LAYER = 8;                      //layer all ground objects should be on for gravity
    public float moveSpeed3D = 5.0f;                        //movement speed while in 3D        
    public float moveSpeed2D = 10.0f;                       //movement speed while in 2D
    public Transform cameraTransform;                       //transform of the main camera
    private bool is3D = true;                               //handles checking if the player is in 3d or 2d mode
    private bool canMove = true;                            //disable or enable player movement
    private bool canInteract = true;                        //disable or enable player interactions

    [SerializeField] private GameObject player2D;           //holds the 2d depiction of the player

    [SerializeField] private float interactDisplayRadius = 20f; //radius of the collider to determine the range at which the player can interact
    [SerializeField] private GameObject interactRadar;          //holds the game object that has the radar collider on it

    private KeyControl interactKey;     //which key to use for interaction, set in Start()

    private List<TransferableObject> objectsInInteractRange;    //a list of all the objects that are in interactable range
    private Rigidbody rigidBody;                                //holds the player's rigid body

    public bool IsHoldingObject = false;                    //if the player has something in their hands or not
    public TransferableObject HeldObject;                   //the object the player is hold

    private bool isTouchingGround;                          //if the player is on the ground, to enable movement logic

    private void Start() {
        interactKey = Keyboard.current.eKey;
        interactRadar.GetComponent<SphereCollider>().radius = interactDisplayRadius;
        objectsInInteractRange = new();
        rigidBody = GetComponent<Rigidbody>();  
    }

    void Update() {
        if (canMove) {
            if (is3D) {
                Move3D();
                if (canInteract) {

                    //remove
                    //Debug.Log(transform.forward);

                    HandleInteractionInput();   
                }
            }
            else {
                Move2D();
            }
        }
    }
    #region Player Movement Controls
    //handles movement in 3d mode
    void Move3D() {
        Debug.Log(isTouchingGround);
        //only allow move while touching the ground
        if (isTouchingGround) {
            Vector2 input = GetInput();

            if (input != Vector2.zero) {  // Check if there's any input
                Vector3 cameraForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
                Vector3 cameraRight = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

                Vector3 direction = cameraForward * input.y + cameraRight * input.x;

                transform.forward = direction.normalized;  // Only set forward direction if there is input
                transform.position += moveSpeed3D * Time.deltaTime * direction;
            }
            //jump
        }
    }
    //handles movement in 2d mode
    void Move2D() {
        Vector2 input = GetInput();

        Vector3 up = player2D.transform.up;
        Vector3 left = -player2D.transform.right;

        Vector3 direction = up * input.y + left * input.x;

        transform.position += moveSpeed2D * Time.deltaTime * direction;
    }
    //helper method to grab keyboard input in 3d, checks for WASD presses
    Vector2 GetInput() {
        var keyboard = Keyboard.current;
        return new Vector2(keyboard.dKey.isPressed ? 1 : keyboard.aKey.isPressed ? -1 : 0,
                           keyboard.wKey.isPressed ? 1 : keyboard.sKey.isPressed ? -1 : 0);
    }

    //swap between dimensions
    public void ChangeDimension() {

        is3D = !is3D;
        rigidBody.isKinematic = !is3D;

    }
    //enable/disable movement logic 
    public void ToggleMovement() {
        canMove = !canMove;
    }
    #endregion

    //returns true if the game is in 3d mode
    public bool IsIn3D() { return is3D; }
    

    public void AddObjectToInRangeList(TransferableObject tObject) {
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
                HeldObject.Drop();
                IsHoldingObject = false;
                HeldObject = null;
            } 
            //only process interact press if theres something to interact with
            else if (objectsInInteractRange.Any()) {
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
                HeldObject = tObject;
                //pick up the object that was found to be the closest
                HeldObject.Pickup(gameObject);
                
                IsHoldingObject = true;

            }

        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == GROUND_LAYER) {
            isTouchingGround = true;
        }
    }
    private void OnCollisionExit(Collision collision) {
        if (collision.collider.gameObject.layer == GROUND_LAYER) {
            isTouchingGround = false;
        }
    }

}

