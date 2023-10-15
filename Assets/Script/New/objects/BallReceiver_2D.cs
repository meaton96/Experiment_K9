using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReceiver_2D : MonoBehaviour
{
    [SerializeField] GameObject outsideOff;
    [SerializeField] GameObject outsideOn;
    [SerializeField] DoorBehaviour doorToOpen;

    void Activate() {
        doorToOpen.OpenDoor();
        outsideOff.SetActive(false);
        outsideOn.SetActive(true);
    }
    void Deactivate() {
        doorToOpen.CloseDoor();
        outsideOff.SetActive(true);
        outsideOn.SetActive(false);
    }
    private void OnTriggerEnter(Collider other) {
        Debug.Log("hit something");
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            Activate();
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            Deactivate();
        }
    }
}
