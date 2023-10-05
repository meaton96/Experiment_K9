using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Windows;

public class MovementController_2D : MonoBehaviour {
    [SerializeField] PlayerControllerBeta playerController;
    [SerializeField] BoxCollider movementCollider;
    // GameObject player2D;
    [SerializeField] List<float> currentWallBounds;
    Vector3 forward;                                    //used to check which wall object is in the foreground to use that as the movement override

    public float moveSpeed2D = 10.0f;
    //  public float pushForce = .1f;

    public const float AXIS_CHANGE_MIN = 1000f;


    private bool cameraTransitioning = false;
    private Vector3 newSpritePos;
    private Vector3 newCamTargetPos;
    private float cameraTransitionSpeed = 4f;

    //up, right, down, left
    private bool[] moveDirEnabled = { true, true, true, true };


    // Start is called before the first frame update
    void Awake() {

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
    }
    void Move2D() {
        var input = playerController.GetInput();
        var up = transform.up;
        var left = -transform.right;
        var direction = up * input.y + left * input.x;


        //checks if the player is allowed to move in a direction and cancels out movement in that direction if applicable
        if (!moveDirEnabled[0] && direction.y > 0) {
            direction.y = 0;
        }
        else if (!moveDirEnabled[1] && (direction.x > 0 || direction.z > 0)) {
            direction.x = 0;
            direction.z = 0;
        }
        else if (!moveDirEnabled[2] && direction.y < 0) {
            direction.y = 0;
        }
        else if (!moveDirEnabled[3] && (direction.x < 0 || direction.z < 0)) {
            direction.x = 0;
            direction.z = 0;
        }

        playerController.transform.position += moveSpeed2D * Time.deltaTime * direction;
    }



    void TransitionToNewAxis(Vector3 pos, WallBehaviour wall) {
        //rotate first to get correct transform.right
        transform.forward = wall.transform.up;
        

        //only supports changing x/z plane not y (ceiling/floor)
        var offsetDirection = GetDirection(wall) == 1 ? -transform.right : transform.right;

        newSpritePos = pos + offsetDirection * movementCollider.size.x;
        
        //move to offset position
      //  transform.position = newSpritePos;
        playerController.transform.position = newSpritePos;
        transform.localPosition = Vector3.zero;

        newCamTargetPos = newSpritePos + wall.transform.up * PlayerDimensionController.cameraOffset2D;

        cameraTransitioning = true;
    }

    private void TransitionCamera() {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newCamTargetPos, cameraTransitionSpeed * Time.deltaTime);

        Vector3 lookDirection = newSpritePos - Camera.main.transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, rotation, cameraTransitionSpeed * Time.deltaTime);

        if (Vector3.Distance(Camera.main.transform.position, newCamTargetPos) < 0.15f) {
            cameraTransitioning = false;
            //reset any weird player object movements 
          //  playerController.transform.position = transform.position;
            
            Debug.Log("camera done moving");
        }


    }
    void HandleNewPlaneCollision(Collider other) {
        if (other.TryGetComponent(out WallBehaviour wallB)) {
            if (wallB.IsWalkThroughEnabled) {
                TransitionToNewAxis(other.ClosestPointOnBounds(transform.position), wallB);
                Debug.Log(wallB.gameObject.name);


            }
            else if (!wallB.IsPassthrough) {
                moveDirEnabled[GetDirection(other)] = false;
            }
            else {
                //allow player to pass through wall maybe do something with camera?
            }
        }
    }
    public void CallOnTriggerEnter(Collider other) {
        if (cameraTransitioning)
            return;
        if (other.transform.up == transform.forward)
            moveDirEnabled[GetDirection(other)] = false;
        else {
            HandleNewPlaneCollision(other);
        }

    }
    public void CallOnTriggerExit(Collider other) {
        if (cameraTransitioning)
            return;
        moveDirEnabled[GetDirection(other)] = true;

    }

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


}
