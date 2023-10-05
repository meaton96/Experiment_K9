using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController_2D : MonoBehaviour {
    [SerializeField] PlayerControllerBeta playerController;
    // GameObject player2D;
    [SerializeField] List<float> currentWallBounds;
    Vector3 forward;                                    //used to check which wall object is in the foreground to use that as the movement override

    public float moveSpeed2D = 10.0f;
    public float pushForce = .1f;
    // Start is called before the first frame update
    void Awake() {

    }

    // Update is called once per frame
    void Update() {
        if (!playerController.IsIn3D()) {
            // Move2D();
            TryMove();
        }
    }

    //handles movement in 2d mode
    //void Move2D() {
    //    Vector2 input = playerController.GetInput();

    //    Vector3 up = transform.up;
    //    Vector3 left = -transform.right;

    //    Vector3 direction = up * input.y + left * input.x;

    //    playerController.transform.position += moveSpeed2D * Time.deltaTime * direction;
    //}
    void Move2D(Vector3 direction) {
        playerController.transform.position += moveSpeed2D * Time.deltaTime * direction;
    }

    //public void OnTriggerStayCustom(Collider other) {
    //    WallBehaviour wall = other.gameObject.GetComponent<WallBehaviour>();
    //    if (wall != null && !wall.IsWalkThroughEnabled) {
    //        Vector3 pushDirection = playerController.transform.position - other.transform.position;
    //        //move the player out of the wall if they are inside one
    //        //might want to be LERP
    //        playerController.transform.position += pushDirection.normalized * pushForce;
    //    }
    //}




    void TryMove() {
        Vector2 input = playerController.GetInput();
        Vector3 up = transform.up;
        Vector3 left = transform.right;
        Vector3 direction = up * input.y + left * input.x;
        Vector3 destination = playerController.transform.position + moveSpeed2D * Time.deltaTime * direction;

        Collider playerCollider = playerController.GetComponent<Collider>();
        Bounds playerBounds = playerCollider.bounds;
        playerBounds.center = destination;

        bool canMove = true;

        Collider[] hitColliders = Physics.OverlapBox(playerBounds.center, playerBounds.extents, Quaternion.identity, LayerMask.GetMask("Walls"));

        if (hitColliders.Length == 0) {
            Debug.LogWarning("Try Move found no wall to move to");
            return;
        }

        foreach (var hitCollider in hitColliders) {
            WallBehaviour wall = hitCollider.GetComponent<WallBehaviour>();

            if (wall != null && !wall.IsWalkThroughEnabled) {
                canMove = false;
                return;
            }
        }

        if (canMove) {
            Move2D(direction); // Move player if no overlapping wall is found or IsWalkThroughEnabled is true
        }
    }



    public void UpdateCurrentWallBounds(List<float> updatedBounds) {
        if (updatedBounds.Count != 4)
            throw new System.ArgumentException("List must have 4 bounds values");

    }
    public void SetAxis(Vector3 fwd) {
        forward = fwd;

    }
    public void HandleSurfaceExit() {
        //TODO
    }
}
