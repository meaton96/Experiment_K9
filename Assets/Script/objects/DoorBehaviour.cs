using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    public bool IsAuto;
    public bool IsLocked;
    //potential future hook up to a button or something to open
  //  [SerializeField] private GameObject activator;
    //door to move
    [SerializeField] private GameObject door;
    //amount to move
    [SerializeField]
    private float doorHeight;
    //how long it will take to open
    [SerializeField]
    private float doorSpeed = 6f;

    enum DoorState {
        Closed,
        Open,
        Moving
    }

    private DoorState currentState;
    private DoorState goalState;

   // private bool currentState = false;
    private bool isMoving = false;

    Vector3 openLocalPosition, closedLocalPosition;


    // Start is called before the first frame update
    void Start()
    {
        closedLocalPosition = door.transform.localPosition;
        openLocalPosition = door.transform.localPosition + Vector3.up * doorHeight;

        Debug.Log(closedLocalPosition);
        Debug.Log(openLocalPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocked && currentState != goalState) {
            currentState = DoorState.Moving;

            // Determine the target position based on the goal state
            Vector3 targetPosition = goalState == DoorState.Open ? openLocalPosition : closedLocalPosition;

            // Move the door towards the target position
            door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, targetPosition, doorSpeed * Time.deltaTime);

            // Check if the door has reached the open or closed position
            if (door.transform.localPosition == openLocalPosition) {
                currentState = DoorState.Open;
            }
            else if (door.transform.localPosition == closedLocalPosition) {
                currentState = DoorState.Closed;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (IsAuto && other.CompareTag("InteractRadar")) {
            goalState = DoorState.Open;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (IsAuto && other.CompareTag("InteractRadar")) {
            goalState = DoorState.Closed;
        }
    }
    public void OpenDoor() {
        goalState = DoorState.Open;
    }
    public void CloseDoor() {
        goalState = DoorState.Closed;
    }

    //public void OpenOrCloseDoor(bool open) {
    //    //TODO
    //}
    //IEnumerator MoveDoor(bool open) {

    //    float elapsedTime = 0; // Time elapsed since the start of door movement
    //    Vector3 startingPosition = door.transform.position; // Starting position of the door
    //    Vector3 targetPosition;

    //    // Determine the target position based on whether the door should open or close
    //    if (open) {
    //        targetPosition = startingPosition + Vector3.up * doorHeight;
    //    }
    //    else {
    //        targetPosition = startingPosition - Vector3.up * doorHeight;
    //    }

    //    // Move the door to the target position over the specified amount of time
    //    while (elapsedTime < timeToOpenDoor) {
    //        door.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / timeToOpenDoor);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    // Ensure the door reaches the target position
    //    door.transform.position = targetPosition;
    //    isMoving = false;
    //}



}
