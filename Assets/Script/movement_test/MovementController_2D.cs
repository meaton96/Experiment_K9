using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
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
    void Move2D(Vector3 direction) {
        playerController.transform.position += moveSpeed2D * Time.deltaTime * direction;
    }

    void TryMove() {
        Vector2 input = playerController.GetInput();
        Vector3 up = transform.up;
        Vector3 left = -transform.right;
        Vector3 direction = up * input.y + left * input.x;
        Vector3 destination = playerController.transform.position + moveSpeed2D * Time.deltaTime * direction;

        Collider playerCollider = playerController.GetComponent<Collider>();
        Bounds playerBounds = playerCollider.bounds;
        playerBounds.center = destination;


        Collider[] hitColliders = Physics.OverlapBox(playerBounds.center, playerBounds.extents, Quaternion.identity, LayerMask.GetMask("Walls"));

        if (hitColliders.Length == 0) {
            Debug.LogWarning("Try Move found no wall to move to");
            return;
        }

        //determine the wall closest to the camera
        WallBehaviour wall = null;
        float closest = float.MaxValue;
        foreach (var hitCollider in hitColliders) {

            float distance = 0;
            if (left == new Vector3(0, 0, 1) || left == new Vector3(0, 0, -1)) {
                distance = Mathf.Abs(Mathf.Pow(Camera.main.transform.position.x, 2) - Mathf.Pow(hitCollider.transform.position.x, 2));

            }
            else if ((left == new Vector3(1, 0, 0) || left == new Vector3(-1, 0, 0))) {
                distance = Mathf.Abs(Mathf.Pow(Camera.main.transform.position.z, 2) - Mathf.Pow(hitCollider.transform.position.z, 2));

            }
            else {
                Debug.LogError("Doggo on ceiling");
            }
            Debug.Log(distance);
            if (distance < closest) {
                closest = distance;
                wall = hitCollider.GetComponent<WallBehaviour>();
            }



        }

        if (wall != null && wall.IsWalkThroughEnabled) {
            Move2D(direction); // Move player if no overlapping wall is found or IsWalkThroughEnabled is true
        }
        

    }


}
