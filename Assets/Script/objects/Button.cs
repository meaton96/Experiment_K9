using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    Vector3 defaultPosition, pressedPosition;
    [SerializeField] private GameObject button;
    [SerializeField] private float pressDistance = .6f;
    public bool IsLocked  = false;
    //only supports doors for now
    [SerializeField] private DoorBehaviour doorToOpen;

    //how long it will take to open
    [SerializeField]
    private float doorSpeed = 5f;

    enum ButtonState {
        Pressed,
        Unpressed,
        Moving
    }

    private ButtonState currentState;
    private ButtonState goalState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = goalState = ButtonState.Unpressed;
        defaultPosition = button.transform.localPosition;
        pressedPosition = button.transform.localPosition + Vector3.down * pressDistance ;

    }

    void Update() {
        if (!IsLocked && currentState != goalState) {
            currentState = ButtonState.Moving;

            // Determine the target position based on the goal state
            Vector3 targetPosition = goalState == ButtonState.Pressed ? pressedPosition : defaultPosition;

            // Move the door towards the target position
            button.transform.localPosition = Vector3.MoveTowards(button.transform.localPosition, targetPosition, doorSpeed * Time.deltaTime);

            // Check if the door has reached the open or closed position
            if (button.transform.localPosition == pressedPosition) {
                currentState = ButtonState.Pressed;
                
            }
            else if (button.transform.localPosition == defaultPosition) {
                currentState = ButtonState.Unpressed;
                
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            goalState = ButtonState.Pressed;
            //open door when button is pressed
            if (doorToOpen != null) {
                doorToOpen.OpenDoor();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            goalState = ButtonState.Unpressed;
            if (doorToOpen != null) {
                doorToOpen.CloseDoor();
            }
        }
    }
}
