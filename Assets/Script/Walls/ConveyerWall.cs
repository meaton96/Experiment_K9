using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerWall : WallBehaviour {
    protected Rigidbody playerRb;
    protected MovementController_2D player2D;
    public float playerMoveForceAmount = 20f;

    private void Update() {
        MovePlayer();
    }
    protected virtual void MovePlayer() {
        if (playerRb != null) {

            if (player2D.Is2DPlayerActive) {
                playerRb.AddForce(transform.forward * playerMoveForceAmount);
            }
        }
    }
    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.layer == LayerInfo.PLAYER) {
            if (collision.gameObject.TryGetComponent(out MovementController_2D player2D)) {
                playerRb = collision.gameObject.GetComponent<Rigidbody>();
                this.player2D = player2D;

            }

        }
    }
    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.layer == LayerInfo.PLAYER) {
            playerRb = null;
            player2D = null;
        }
    }
}
