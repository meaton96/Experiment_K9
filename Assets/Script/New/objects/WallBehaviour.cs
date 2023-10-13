using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehaviour : MonoBehaviour {
    //to check if the player can enter or leave 2d/3d space using this wall
    public bool AllowsDimensionTransition = false;
    //to check if the player can move into this object or not
    public bool IsWalkThroughEnabled = true;
    //to check if the player can pass through the wall while in 2D
    public bool IsPassthrough = false;
    //remove from all checks for player movement
    public bool RemoveFromWalkChecks = false;

    private Rigidbody player;
    Vector2 WallForce = new Vector2();
    [SerializeField]
    public bool IsMovingWall = false;

    public float pushForce = 1f;
    
    // This method controls conveyor walls, the walls that move the player in a
    // certain direction when they are merged into them.
    private void MovingWall()
    {
        WallForce.x = transform.right.x;
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.layer == LayerInfo.PLAYER) {
            var rb = other.GetComponent<Rigidbody>();
            player = rb;
            //if the wall is walkable just slightly push the player forward out of the wall to prevent weird things
            if (IsWalkThroughEnabled) {
                rb.AddForce(other.transform.forward * pushForce);
            } else {
                //player is inside a non walkable wall push them out aggresively
                var collider = GetComponent<Collider>();
                //get the closest point on the edge of the wall to push the player to
                var closestPointOnBounds = collider.ClosestPointOnBounds(other.transform.position);
                var pushDir = (closestPointOnBounds - other.transform.forward).normalized;


                rb.AddForce(pushDir * pushForce * 5f);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerInfo.PLAYER)
        {
            player = null;
        }
    }
}
