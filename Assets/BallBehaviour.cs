using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour {
    public float moveDistance = 3f; // Total distance to move.
    public float moveSpeed = 2f;    // Speed of the movement.

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool movingUp = true;

    void Start() {
        initialPosition = transform.position;
        targetPosition = initialPosition + Vector3.up * moveDistance;
    }

    void Update() {
        float step = moveSpeed * Time.deltaTime;

        if (movingUp) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (transform.position == targetPosition) {
                movingUp = false;
            }
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, step);

            if (transform.position == initialPosition) {
                movingUp = true;
            }
        }
    }
}
