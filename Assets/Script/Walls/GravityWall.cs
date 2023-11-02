using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWall : ConveyerWall {
    [SerializeField]
    private float gravityAmount = 9.8f;
    protected override void MovePlayer() {
        if (playerRb != null) {

            if (player2D.Is2DPlayerActive) {

                playerRb.AddForce(Vector3.down * gravityAmount, ForceMode.Acceleration);
            }
        }
    }




}
