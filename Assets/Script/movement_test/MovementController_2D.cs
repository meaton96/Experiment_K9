using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MovementController_2D : MonoBehaviour {
    [SerializeField] PlayerBehaviour playerController;
    [SerializeField] PlayerDimensionController playerDimensionController;
 //   [SerializeField] BoxCollider movementCollider;
    // GameObject player2D;
    [SerializeField] List<float> currentWallBounds;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Rigidbody playerRigidBody3D;
    [SerializeField] private float offSetAmount = 5.5f;

    private Vector3 gizmoDrawLoc;
   // private SpriteRenderer spriteRenderer;
    Vector3 forward;                                    //used to check which wall object is in the foreground to use that as the movement override

    public enum ProjectionState {
        OutOfRange,
        HoldingObject,
        In2D,
        In2DHoldingObject
    }
    private ProjectionState projectionState;
    public float moveSpeed2D = 10.0f;
    //  public float pushForce = .1f;

    public const float AXIS_CHANGE_MIN = 1000f;
    [SerializeField] private List<Sprite> sprites = new();

    private bool cameraTransitioning = false;
    private Vector3 newSpritePos;
    private Vector3 newCamTargetPos;
    private float cameraTransitionSpeed = 4f;

    //up, right, down, left
    private bool[] moveDirEnabled = { true, true, true, true };

    private int dirIn;

    [SerializeField] private SpriteRenderer dog2DSprite;


    // Start is called before the first frame update
    void Awake() {
       // dog2DSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if (!playerController.IsIn3D()) {
            // Move2D();
            if (cameraTransitioning) {
                TransitionCamera();
            }
            else {
                Move2D();
            }
        }
        else {
            
        }
    }
    //handles player movement in 2D
    void Move2D() {
        var input = GetInput();
        var up = transform.up;
        var left = -transform.right;
        var direction = up * input.y + left * input.x;


        ////checks if the player is allowed to move in a direction and cancels out movement in that direction if applicable
        //if (!moveDirEnabled[0] && direction.y > 0) {
        //    direction.y = 0;
        //}
        //else if (!moveDirEnabled[1] && (direction.x > 0 || direction.z > 0)) {
        //    direction.x = 0;
        //    direction.z = 0;
        //}
        //else if (!moveDirEnabled[2] && direction.y < 0) {
        //    direction.y = 0;
        //}
        //else if (!moveDirEnabled[3] && (direction.x < 0 || direction.z < 0)) {
        //    direction.x = 0;
        //    direction.z = 0;
        //}

        rb.velocity = direction * moveSpeed2D;
     //   Debug.Log(input);
        
        // Flip the sprite when the dog moves the other way
        if (input.x < 0)
        {
            dog2DSprite.flipX = true;
        }
        else if (input.x > 0)
        {
            dog2DSprite.flipX = false;
        }
    }
    public Vector2 GetInput() {
        var keyboard = Keyboard.current;
        return new Vector2(keyboard.dKey.isPressed ? 1 : keyboard.aKey.isPressed ? -1 : 0,
                           keyboard.wKey.isPressed ? 1 : keyboard.sKey.isPressed ? -1 : 0);
    }

    //locks the axes to the up/down/left/right on the wall
    //should prevent the dog from slipping into the or out of the wall
    public void ProcessAxisChange() {
        var right = transform.right;

        //crazy floating point errors
        if (right.x > 0.0001 || right.x < -0.0001) {
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
        else if (right.y > 0.0001 || right.y < -0.0001) {
            Debug.LogError("Unsupported behaviour - doggo on floor");
        }
        else if (right.z > 0.0001 || right.z < -0.0001) {
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        }
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.TryGetComponent(out WallBehaviour wallB)) {
            if (wallB.IsWalkThroughEnabled) {
                TransitionToNewAxis(collision.collider.ClosestPointOnBounds(transform.position), wallB);

            }
            else if (!wallB.IsPassthrough) {
                //DisablePlayerMovementInDirection(GetDirection(collision.collider));
            }
            else {
                //allow player to pass through wall maybe do something with camera?
            }
        }
    }
    private void OnDrawGizmos() {
    //    Gizmos.DrawCube(gizmoDrawLoc, Vector3.one * 2);
    }

    //handles transitioning to anew axis when encountering another wall at a 90 degree angle
    void TransitionToNewAxis(Vector3 pos, WallBehaviour wall) {
        
        //rotate first to get correct transform.right
        transform.forward = wall.transform.up;
        
        ProcessAxisChange();
       // Debug.Log(transform.right);


        //only supports changing x/z plane not y (ceiling/floor)
        var offsetDirection = GetDirection(wall) == 1 ? -transform.right : transform.right;

        //   Debug.Log(pos);
        //    Debug.Log(offsetDirection);


        newSpritePos = pos + offsetDirection * offSetAmount;
        newSpritePos += transform.forward * PlayerDimensionController.WALL_DRAW_OFFSET;
       // gizmoDrawLoc = newSpritePos;

        //move to offset position
        //  transform.position = newSpritePos;
        //   playerController.transform.position = newSpritePos;
        transform.position = newSpritePos;

    //    newCamTargetPos = newSpritePos + wall.transform.up * PlayerDimensionController.cameraOffset2D;

    //    cameraTransitioning = true;
    }
    //moves the camera to the new location when moving to a new wall
    //camera moves to newCamTargetPos and points at newSpritePos
    private void TransitionCamera() {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newCamTargetPos, cameraTransitionSpeed * Time.deltaTime);

        Vector3 lookDirection = newSpritePos - Camera.main.transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, rotation, cameraTransitionSpeed * Time.deltaTime);

        if (Vector3.Distance(Camera.main.transform.position, newCamTargetPos) < 0.15f) {
            cameraTransitioning = false;
            
        }


    }
    //handles collision with a wall that is not on the same plane as the player
    //supports 90 degree interactions with walls and nothing else
    //void HandleNewPlaneCollision(Collider other) {
    //    if (other.TryGetComponent(out WallBehaviour wallB)) {
    //        if (wallB.IsWalkThroughEnabled) {
    //            TransitionToNewAxis(other.ClosestPointOnBounds(transform.position), wallB);

    //        }
    //        else if (!wallB.IsPassthrough) {
    //            DisablePlayerMovementInDirection(GetDirection(other));
    //        }
    //        else {
    //            //allow player to pass through wall maybe do something with camera?
    //        }
    //    }
    //}
    //disables the movement in the passed in direction
    void DisablePlayerMovementInDirection(int dir) {
        if (dir < 0 || dir >= moveDirEnabled.Length)
            throw new ArgumentException("movement direction not supported");
        moveDirEnabled[dir] = false;
        dirIn = dir;
    }
    //called when the movement hitbox hits something 
    public void CallOnTriggerEnter(Collider other) {
        if (cameraTransitioning)
            return;
        if (other.gameObject.layer == LayerInfo.GROUND) {
            HandleGroundCollision(other);
            return;
        }

        if (other.transform.up == transform.forward) {
            DisablePlayerMovementInDirection(GetDirection(other));
        }
        else {
          //  HandleNewPlaneCollision(other);
        }

    }
    //reenable the movement direction when leaving a collision
    public void CallOnTriggerExit(Collider other) {
        if (cameraTransitioning)
            return;
        moveDirEnabled[dirIn] = true;
    }
    //handles interaction with the ground planes
    private void HandleGroundCollision(Collider other) {


        var direction = GetGroundDirection(other);

        DisablePlayerMovementInDirection(direction);
    }

    //gets the direction that the object is in relative to the player
    //returns 0 for up, 1 for right, 2 for down, 3 for left
    public int GetDirection(Collider other) {
        Vector3 toOther = other.transform.position - transform.position;

        float dotUp = Vector3.Dot(toOther.normalized, transform.up);
        float dotRight = Vector3.Dot(toOther.normalized, transform.right);
        //checks +/- 45 degrees to check if object is more above or below than left or right
        if (dotUp > 0.7071) return 0; // Above
        if (dotUp < -0.7071) return 2; // Below

        if (dotRight > 0) return 3; // Right
        if (dotRight < 0) return 1; // Left

        return -1; // Error or the object is too close
    }
    public int GetDirection(WallBehaviour other) {
        Vector3 toOther = other.transform.position - transform.position;

        float dotUp = Vector3.Dot(toOther.normalized, transform.up);
        float dotRight = Vector3.Dot(toOther.normalized, transform.right);

        //checks +/- 45 degrees to check if object is more above or below than left or right
        if (dotUp > 0.7071) return 0; // Above
        if (dotUp < -0.7071) return 2; // Below

        if (dotRight > 0) return 3; // Right
        if (dotRight < 0) return 1; // Left

        return -1; // Error or the object is too close
    }
    //gets the direction of the ground using the dot product of the vector to the object
    //with the objects transform.up property
    //returns 2 for down and 0 for up
    public int GetGroundDirection(Collider other) {
        var toOther = other.transform.position - transform.position;
        return Vector3.Dot(toOther.normalized, transform.up) < 0 ? 2 : 0;

    }


    public void SetProjectionState(ProjectionState state) {
        projectionState = state;
        switch (projectionState) {
            case ProjectionState.OutOfRange:
                dog2DSprite.sprite = sprites[1]; break;
            case ProjectionState.HoldingObject:
                dog2DSprite.sprite = sprites[2]; break;
            case ProjectionState.In2D:
                dog2DSprite.sprite = sprites[3]; break;
            case ProjectionState.In2DHoldingObject:
                dog2DSprite.sprite = sprites[4]; break;

        }
    }



}
