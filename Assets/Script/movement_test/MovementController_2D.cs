using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    public const float AXIS_CHANGE_MIN = 1000f;


    private bool cameraTransitioning = false;
    private Vector3 newSpritePos;
    private Vector3 newCameraTarget;
    private float cameraTransitionSpeed = 4f;

    

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
                TryMove();
            }
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

        //dont like having GetComponent in updat emethod but this fully breaks without this
        //assignign any other hitbox just doesnt work for some reason
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
            var temp = hitCollider.GetComponent<WallBehaviour>();

            //flag a change in axes
            if (temp != null && temp.transform.up != transform.forward && !temp.IsPassthrough && !temp.RemoveFromWalkChecks) {

            }
            //handles overlapping walls uses the one closer to the camera to handle movement logic
            if (distance < closest) {
                closest = distance;
                wall = temp;
            }



        }
        if (wall.transform.up != transform.forward) {
            //transition to new axis

        }

        if (wall != null && wall.IsWalkThroughEnabled) {
            Move2D(direction); // Move player if IsWalkThroughEnabled is true
        }


    }

    void TransitionToNewAxis(Vector3 pos, WallBehaviour wall) {
        transform.forward = wall.transform.up;
        transform.position = pos;
    }

    private void TransitionCamera() {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newCameraTarget, cameraTransitionSpeed * Time.deltaTime);

        Vector3 lookDirection = newSpritePos - Camera.main.transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, rotation, cameraTransitionSpeed * Time.deltaTime);

        if (Vector3.Distance(Camera.main.transform.position, newCameraTarget) < 0.15f) {
            cameraTransitioning = false;

            //  playerControllerScript.ChangeDimension();
            //  playerControllerScript.ToggleMovement();
            //  dog2DHitbox.SetActive(true);    //enable 2d movement hitbox as last step to avoid double collision

        }


    }
    public void CallOnTrigger(Collider other) {
        Debug.Log(other.gameObject.name);
    }
}
