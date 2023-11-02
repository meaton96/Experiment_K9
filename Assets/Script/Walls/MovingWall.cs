using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovingWall : WallBehaviour
{
    public bool isMoving = false;
    //waiting for a bit before moving
    private bool wait = false;
    [SerializeField] private float waitTime = 10f;
    //how fast the wall is moving
    public float MoveSpeed = 5.0f;
    //the staring and ending points of the wall movement(has to stop at a wall before going back)
    public Transform initialposition;
    public Transform targetposition;
    //tracks time
    private float time;
    public float stopThreshold;
    private Vector3 stopLocalPosition;
    Vector2 WallForce = new Vector2();

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void setWallX() {
        WallForce.x = transform.right.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving == true) {
            //basically speed wall
            float step = MoveSpeed * Time.deltaTime;
            //this will move the wall at the constant speed towards the target
            transform.position = Vector3.MoveTowards(transform.position, targetposition.position, step);
            Vector3 localPosition = transform.InverseTransformPoint(targetposition.position);

            if (Vector3.Distance(localPosition, stopLocalPosition) <= stopThreshold) {
                //makes the wall wait a few seconds
                wait = true;
                //basically makes the wall go back and fourth between the two wall positions by swapping target and initial
                (targetposition, initialposition) = (initialposition, targetposition);
                //allows the wait and resets time.
                isMoving = false;
                time = 0;
            }
        }
        else if (wait == true) {
            //waitting and then allow wall to move again
            time += Time.deltaTime;
            if (time >= waitTime) {
                isMoving = true;
            }
        }
    }
}
