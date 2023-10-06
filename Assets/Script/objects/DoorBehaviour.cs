using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    public bool IsAuto;
    public bool IsLocked;
    //potential future hook up to a button or something to open
    [SerializeField] private GameObject activator;
    //door to move
    [SerializeField] private GameObject door;
    //amount to move
    [SerializeField]
    private float doorHeight;
    //how long it will take to open
    [SerializeField]
    private float timeToOpenDoor;

    private bool isOpen;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerInfo.PLAYER) {
            if (IsAuto &&  !IsLocked) {
                OpenOrCloseDoor(open: true);
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerInfo.PLAYER) {
            OpenOrCloseDoor(open: false);
        }
    }

    public void OpenOrCloseDoor(bool open) {
        if (isOpen == open)
            return;
        isOpen = open;
        StartCoroutine(MoveDoor(open));
    }
    IEnumerator MoveDoor(bool open) {
        float elapsedTime = 0; // Time elapsed since the start of door movement
        Vector3 startingPosition = door.transform.position; // Starting position of the door
        Vector3 targetPosition;

        // Determine the target position based on whether the door should open or close
        if (open) {
            targetPosition = startingPosition + Vector3.up * doorHeight;
        }
        else {
            targetPosition = startingPosition - Vector3.up * doorHeight;
        }

        // Move the door to the target position over the specified amount of time
        while (elapsedTime < timeToOpenDoor) {
            door.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / timeToOpenDoor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the door reaches the target position
        door.transform.position = targetPosition;
    }



}
