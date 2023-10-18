using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController_2D : MonoBehaviour {
    [SerializeField] PlayerBehaviour playerController;
    [SerializeField] PlayerDimensionController playerDimensionController;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Rigidbody playerRigidBody3D;
    [SerializeField] private float offSetAmount = 5.5f;
    private Collider dogCollider2D;

    public bool CanMove = true;

    private Vector3 gizmoDrawLoc;
    public WallBehaviour currentWall;
    Vector3 forward;                                    //used to check which wall object is in the foreground to use that as the movement override

    public enum ProjectionState {
        OutOfRange,
        HoldingObject,
        In2D,
        In2DHoldingObject
    }
    private ProjectionState projectionState;
    public float moveSpeed2D = 15.0f;
    [SerializeField] private List<Sprite> sprites = new();

    private Vector3 newSpritePos;
    private bool[] moveDirEnabled = { true, true, true, true };

    private int dirIn;

    [SerializeField] private SpriteRenderer spriteRenderer;




    // Start is called before the first frame update
    void Awake() {
        // dog2DSprite = GetComponent<SpriteRenderer>();
        dogCollider2D = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update() {
        if (!playerController.IsIn3D()) {
            // Move2D();
            if (CanMove)
                Move2D();
        }
        else {
            //HandleWallCollision
        }
    }
    //handles player movement in 2D
    void Move2D() {
        var input = GetInput();
        var up = transform.up;
        var left = -transform.right;
        var direction = up * input.y + left * input.x;
        rb.velocity = direction * moveSpeed2D;
        // Flip the sprite when the dog moves the other way
        if (input.x < 0) {
            spriteRenderer.flipX = true;
        }
        else if (input.x > 0) {
            spriteRenderer.flipX = false;
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
           
            //if (wallB.IsWalkThroughEnabled) {
              
                HandleWallCollision(collision.collider, wallB);
            //}
        }
    }
    private void OnCollisionExit(Collision collision) {
        
        if (collision.gameObject.TryGetComponent(out WallBehaviour wallB)) {

            if (currentWall == wallB && !playerController.IsIn3D() || currentWall == null && !playerController.IsIn3D()) {
                Debug.Log("leaving wall");
                currentWall = null;
                //print("test");
                //playerController.ChangeDimension();

                playerDimensionController.TransitionTo3D();

            }
            else if (playerController.IsIn3D())
            {
                currentWall = null;
            }
        }
    }

    private void HandleWallCollision(Collider collider, WallBehaviour wallB) {
        var closestPoint = collider.ClosestPointOnBounds(transform.position);
        // print("its on wall4");
        // print(currentWall);
        //projected space wasnt clear so dont move there
        //removed this because it desynced the 2d movement and radar
        //if (!IsProjectionSpaceClear(closestPoint)) {
        // print("this is stopping it");
        //  return;
        //}
        //print(currentWall == null);
        //print(currentWall == null || wallB.transform.up != currentWall.transform.up && wallB.AllowsDimensionTransition == true);
        //only if you can walk on the wall and it has a different up transform meaning its on a different axis
        if (currentWall == null || wallB.transform.up != currentWall.transform.up && wallB.AllowsDimensionTransition == true) {
            //print("its on wall5");
            currentWall = wallB;
            TransitionToNewAxis(collider.ClosestPointOnBounds(transform.position), wallB);

        }
    }
    public bool IsProjectionSpaceClear(Vector3 position) {
        if (dogCollider2D == null) { dogCollider2D = GetComponent<Collider>(); }

        var boxHits = Physics.OverlapBox(position, dogCollider2D.bounds.extents, Quaternion.identity, LayerMask.GetMask("Walls"));

        if (boxHits.Length == 0) return true;



        foreach (var hit in boxHits) {
            if (hit.TryGetComponent(out WallBehaviour wallB)) {
                if (!wallB.IsWalkThroughEnabled)
                    return false;
            }
            else {
                //something that wasnt a wall is blocking 
                return false;
            }
        }

        return true;
    }

    //handles transitioning to anew axis when encountering another wall at a 90 degree angle
    void TransitionToNewAxis(Vector3 pos, WallBehaviour wall) {

        //rotate first to get correct transform.right
        transform.forward = wall.transform.up;

        ProcessAxisChange();
        currentWall = wall;


        //only supports changing x/z plane not y (ceiling/floor)
        var offsetDirection = GetDirection(wall) == 1 ? -transform.right : transform.right;
        newSpritePos = pos + offsetDirection * offSetAmount;
        newSpritePos += transform.forward * PlayerDimensionController.WALL_DRAW_OFFSET;

        //move to offset position
        transform.position = newSpritePos;
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
    public void SetProjectionState(ProjectionState state) {
        projectionState = state;
        switch (projectionState) {
            case ProjectionState.OutOfRange:
                spriteRenderer.sprite = sprites[1]; break;
            case ProjectionState.HoldingObject:
                spriteRenderer.sprite = sprites[2]; break;
            case ProjectionState.In2D:
                spriteRenderer.sprite = sprites[3]; break;
            case ProjectionState.In2DHoldingObject:
                spriteRenderer.sprite = sprites[4]; break;

        }
    }
    public bool CanTransitionOutOfCurrentWall() {
        return currentWall.AllowsDimensionTransition;
    }
    public bool IsFlipped() {
        return spriteRenderer.flipX;
    }


}
