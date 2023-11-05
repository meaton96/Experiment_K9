using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour {

    public bool is3D = true;                               //handles checking if the player is in 3d or 2d mode
  //  private bool canMove = true;                            //disable or enable player movement
    private bool canInteract = true;                        //disable or enable player interactions
   
    public GameObject player2D;                             //holds the 2d depiction of the player
    public MovementController_2D player2DMovementController;           //holds the 2d depiction of the player
    public GameObject player3D;                             //holds the 3d depiction of the player

    private CharacterController playerController;
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private InterfaceBehaviour interfaceScript;
    [SerializeField] private PickupController pickupController;

    public float interactDisplayRadius = 20f; //radius of the collider to determine the range at which the player can interact
    [SerializeField] private GameObject interactRadar;          //holds the game object that has the radar collider on it

    private KeyControl interactKey;                             //which key to use for interaction, set in Start()
    private KeyControl resetKey;                             //which key to use for interaction, set in Start()

    [SerializeField] private GameObject spawnPoint;

    

    //[HideInInspector] public bool IsHoldingObject = false;       
   // public bool IsHoldingObject = false;       //if the player has something in their hands or not
  //  [HideInInspector] public GrabbableObject HeldObject;                   //the object the player is hold

  //  Vector3 initialPosition;                                            //store the initial position and dimension to reset the player
 //   bool initialDimension3D;
    bool paused = false;

    [SerializeField] private bool canResetLocation = true;

    private void Start() {
        //DontDestroyOnLoad(transform.parent.gameObject);
        interactKey = Keyboard.current.eKey;
        resetKey = Keyboard.current.rKey;
        interactRadar.GetComponent<SphereCollider>().radius = interactDisplayRadius;
       // objectsInInteractRange = new();

     //   initialDimension3D = is3D;
       // initialPosition = player3D.transform.position;

        if (player3D != null) {
            playerController = player3D.GetComponent<CharacterController>();
            thirdPersonController = player3D.GetComponent<ThirdPersonController>();
        }
        else {
            Debug.LogError("Missing player 3d when assigning controller scripts");
        }
    }
    
    public void SetPaused(bool paused) {
        this.paused = paused;
    }

    void Update() {
        
        if (!paused) {
            if (canInteract) {

                //HandleInteractionInput();
            }
            if (canResetLocation) {
                HandleResetInput();
            }
        }
    }
    
   private void HandleResetInput() {
        if (is3D) {
            if (resetKey.wasPressedThisFrame) {
                ResetPlayerPosition();
            }
        }
    }
    private void ResetPlayerPosition() {
        Debug.Log("resetting player to: " + spawnPoint.transform.position);
        Spawn();


    }
    public void Spawn() {
        if (spawnPoint != null) {
            Move3DPlayerToLocation(spawnPoint.transform.position);
        }
        else {
            Debug.LogWarning("Missing spawn point");
            var spawn = GameObject.FindWithTag("PlayerSpawnPoint");
            if (spawn == null) {
                throw new System.Exception("Missing spawn point in level " + SceneManager.GetActiveScene().name);
            }
            Move3DPlayerToLocation(spawn.transform.position);

        }
    }

    public void Move3DPlayerToLocation(Vector3 location) {
        Debug.Log(thirdPersonController == null);
      //  thirdPersonController.ToggleMovement(false);
        player3D.SetActive(false);
        player3D.transform.position = location;
        player3D.SetActive(true);
        StartCoroutine(EnablePlayerMovementOnNextFrame());
    }
    private IEnumerator EnablePlayerMovementOnNextFrame() {
        for (int x = 0; x < 2; x++) {
            yield return new WaitForEndOfFrame();
        }
        thirdPersonController.ToggleMovement(true);
        yield return null;
    }

    //swap between dimensions
    public void ChangeDimension() {
        //TODO
        is3D = !is3D;
        // rigidBody.isKinematic = !is3D;
        
        //handle changing the held object dimension
        pickupController.ChangeDimension();

    }
   

    //returns true if the game is in 3d mode
    public bool IsIn3D() { return is3D; }

    

    //handles player interaction with interactable objects
    //private void HandleInteractionInput() {

    //    if (interactKey.wasPressedThisFrame) {

    //        //if the player is already holding something then drop it
    //        if (IsHoldingObject) {
    //            DropHeldObject();

    //        }
    //        //only process interact press if theres something to interact with
    //        else {
    //            PickupObject();


    //        }

    //    }
    //}
    //private void PickupObject() {
    //    if (IsIn3D()) {
    //        Handle3DInteractions();
    //    }
    //    else {
    //        Pickup2DObject();
    //    }
    //}

    ////handle picking up objects while in 2d
    //private void Pickup2DObject() {
    //    var tObject = GetObjectClosestTo2DPlayer();

    //    if (tObject != null && !tObject.Is3D) {
    //        HeldObject = tObject;
    //        //pick up the object that was found to be the closest
    //        (HeldObject as TransferableObject).Pickup2D(player2D);
    //        IsHoldingObject = true;
    //        player2DMovementController.SetProjectionState(MovementController_2D.ProjectionState.In2DHoldingObject);

    //    }
    //}
    ////handle picking up 3d objects while in 3d 
    //private void Handle3DInteractions() {
    //    var tObject = GetObjectClosestToCameraLookAt();
    //    //only process interactions with 3d objects while in 3d
    //    if (tObject != null && tObject.Is3D) {
    //        HeldObject = tObject;
    //        //pick up the object that was found to be the closest
    //        HeldObject.Pickup3D(player3D);
    //        IsHoldingObject = true;
    //    }
    //}



}

