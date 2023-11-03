using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Platform : ActivatablePuzzlePiece {

    public List<Vector3> travelLocations;
    public float platformMovementSpeed = 5f;
    public float firstLastWaitTime = 2.0f;

    [SerializeField] GameObject player;
    Rigidbody playerRb;
    private Rigidbody rb;

    private int currentTargetIndex = 0;
    private bool isMovingForward = true;
    [SerializeField] private bool playerOnPlatform = false;
    private float distanceToCheck = .05f;
    [SerializeField] private bool unlocked = false;

    [SerializeField] private bool unlockedByPlayerCollision = true;
    [SerializeField] private bool dontMoveWithoutPlayer = true;

    private Vector3 lastPos, firstPos;
    public enum PlatformState {
        Waiting,
        Moving,
    }
    [SerializeField] private PlatformState state;

    private void Start() {
        if (travelLocations == null || travelLocations.Count < 2) {
            Debug.LogWarning("Insufficient travel locations provided.");
            return;
        }
        else {
            lastPos = travelLocations[^1];
            firstPos = travelLocations[0];
        }
        state = PlatformState.Waiting;
        rb = GetComponent<Rigidbody>();
    }
    public override void Activate() {
        unlocked = true;
    }

    public override void Deactivate() {
        unlocked = false;
    }

    private void Update() {
        if (travelLocations == null || travelLocations.Count < 2) {
            Debug.LogWarning("Insufficient travel locations provided.");
            return;
        }

        MovePlatform();


    }

    public void StartMoving() {
        if (unlocked) {
            state = PlatformState.Moving;
        }

    }

    private void MovePlatform() {

        //if the platform is moving
        if (state == PlatformState.Moving) {
            var targetPosition = travelLocations[currentTargetIndex];
            //check how close it is to the target
            var distSquaredToTarget = (targetPosition - transform.position).sqrMagnitude;
            //reached a destination
            if (distSquaredToTarget <= distanceToCheck) {
                //end pos
                if (Vector3.Distance(targetPosition, lastPos) < .01f) {
                    StartCoroutine(WaitThenMove());
                    isMovingForward = false;
                    currentTargetIndex--;
                }
                //start pos
                else if (Vector3.Distance(targetPosition, firstPos) < .01f) {
                    StartCoroutine(WaitThenMove());
                    isMovingForward = true;
                    currentTargetIndex++;
                }
                //middle point
                else {
                    if (isMovingForward)
                        currentTargetIndex++;
                    else
                        currentTargetIndex--;
                }
            }
            //move the platform if not at a destination
            else {
                var moveDirection = (targetPosition - transform.position).normalized;
                var velocity = platformMovementSpeed * moveDirection;
                rb.velocity = velocity; // set the Rigidbody's velocity to move the platform
                if (playerOnPlatform) {
                    playerRb.velocity = velocity;
                }
                //transform.position = Vector3.MoveTowards(transform.position, targetPosition, platformMovementSpeed * Time.deltaTime);
                //var velocity = (targetPosition - transform.position) * (platformMovementSpeed * Time.deltaTime);


            }
        }
    }



    private IEnumerator WaitThenMove() {
        state = PlatformState.Waiting;
        rb.velocity = Vector3.zero;
        for (int x = 0; x < 2; x++) {
            yield return new WaitForSeconds(firstLastWaitTime / 2f);
        }
        state = PlatformState.Moving;
        yield return null;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == LayerInfo.PLAYER) {
            if (!unlocked && unlockedByPlayerCollision) {
                unlocked = true;
            }
            playerOnPlatform = true;
            player = collision.gameObject;
            playerRb = player.GetComponent<Rigidbody>();
            StartMoving();
        }
    }
    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.layer == LayerInfo.PLAYER) {
            playerOnPlatform = false;

        }
    }
}

