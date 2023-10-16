using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Platform : ActivatablePuzzlePiece {
    public override void Activate() {
        unlocked = true;
    }

    public override void Deactivate() {
        unlocked = false;
    }
    public List<Vector3> travelLocations;
    public float platformMovementSpeed = 5f;
    public float firstLastWaitTime = 2.0f;

    [SerializeField] GameObject player;
    Rigidbody playerRb;
    private Rigidbody rb;

    private int currentTargetIndex = 0;
    private bool isMovingForward = true;
    private bool playerOnPlatform = false;
    private float distanceToCheck = .05f;
    [SerializeField] private bool unlocked = false;

    [SerializeField] private bool unlockedByPlayerCollision = true;

    public enum PlatformState {
        First,
        Moving,
        Last
    }
    [SerializeField] private PlatformState state;

    private void Start() {
        state = PlatformState.First;
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (travelLocations == null || travelLocations.Count < 2) {
            Debug.LogWarning("Insufficient travel locations provided.");
            return;
        }
        if (unlocked) {
            MovePlatform();
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
                //check which direction its moving and change the current target 
                if (isMovingForward) {
                    //moving forward so increase the target index
                    currentTargetIndex++;
                    //if the target index is now >= to the count then we reached the end so we need to reverse direction
                    if (currentTargetIndex >= travelLocations.Count) {
                        //set target as the 2nd to last one
                        currentTargetIndex -= 2;
                        //swap to moving backward
                        isMovingForward = false;
                        state = PlatformState.Last;
                        StartCoroutine(WaitThenMove());
                    }
                }
                //platform was moving backward so ssame as above except start the platform moving forward again
                else {
                    currentTargetIndex--;
                    if (currentTargetIndex < 0) {
                        currentTargetIndex = 1;
                        isMovingForward = true;
                        state = PlatformState.First;
                        StartCoroutine(WaitThenMove());
                    }
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

            }
        }
        //platform is not in a movement state
        //this will be the case on level load or potentially when unlocking or something
        else {
            if (state == PlatformState.First) {
                isMovingForward = true;

            }
            else if (state == PlatformState.Last) {
                isMovingForward = false;
            }
            state = PlatformState.Moving;
        }
    }


    private IEnumerator WaitThenMove() {
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
          //  player.transform.SetParent(transform);
        }
        //else if (collision.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
        //    collision.gameObject.transform.SetParent(transform);
        //}
    }
    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.layer == LayerInfo.PLAYER) {
            playerOnPlatform = false;
        //   player.transform.SetParent(null);

        }
        //else if (collision.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
        //    collision.gameObject.transform.SetParent(null);
        //}
    }
}

