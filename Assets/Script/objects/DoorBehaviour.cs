using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : ActivatablePuzzlePiece {
    public bool IsAuto;
    public bool IsLocked;
    ////potential future hook up to a button or something to open
    [SerializeField] private List<GameObject> activators;
    //door to move
    [SerializeField] private GameObject door;
    //amount to move
    [SerializeField]
    private float doorHeight;
    //how long it will take to open
    [SerializeField]
    private float doorSpeed = 6f;

    [SerializeField] bool is2D;
    //set this to true if you want the door to move into the wall instead of up for some reason
    [SerializeField] bool openNegativeRight = false;
    enum DoorState {
        Closed,
        Open,
        Moving
    }

    private DoorState currentState;
    private DoorState goalState;


    Vector3 openLocalPosition, closedLocalPosition;


    // Start is called before the first frame update
    void Start() {
        closedLocalPosition = door.transform.localPosition;


        if (openNegativeRight) {
            openLocalPosition = door.transform.localPosition + -transform.right * doorHeight;
            return;
        }
        else {
            openLocalPosition = door.transform.localPosition + transform.up * doorHeight;

        }

        // The door will be locked at the start if there are any activators
        // tied to it.
        if (activators.Count > 0)
        {
            IsLocked = true;
        }
        else
        {
            IsLocked = false;
        }
    }

    // Update is called once per frame
    void Update() {
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

        if (CheckActivators() == true)
        {
            IsLocked = true;
        }
        else
        {
            IsLocked = false;
        }
    }

    // Returns a bool based on whether all the associated activators are
    // activated.
    private bool CheckActivators()
    {
        bool allOn = true;

        for (int i = 0; i < activators.Count; i++)
        {
            if (activators[i] is BallReceiver)
            {
                if (activators[i].active == false)
                {
                    allOn = false;
                    break;
                }
            }
        }

        return allOn;
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

    private void OpenDoor() {
        goalState = DoorState.Open;
    }
    private void CloseDoor() {
        goalState = DoorState.Closed;
    }
    public bool IsOpen() {
        return goalState == DoorState.Open;
    }

    public override void Activate() {
        OpenDoor();
    }

    public override void Deactivate() {
        CloseDoor();
    }
}