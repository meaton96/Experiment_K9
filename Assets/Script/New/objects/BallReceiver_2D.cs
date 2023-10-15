using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReceiver_2D : MonoBehaviour {
    [SerializeField] GameObject outsideOff;
    [SerializeField] GameObject outsideOn;
    [SerializeField] DoorBehaviour doorToOpen;
    [SerializeField] bool Allow3DActivation = false;

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
        //check object layer
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {

            //get the object trying to activate the switch
            if (TryGetTransferableObjectScript(other.gameObject, out TransferableObject tObject)) {
                //check if its 3d 
                if (tObject != null) {
                    //only activate if it is 3d and 3d activation is enabled or the object is 2d
                    if ((tObject.Is3D && Allow3DActivation) || !tObject.Is3D) {
                        Activate();
                    }
                }
            }


        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            Deactivate();
        }
    }

    private bool TryGetTransferableObjectScript(GameObject interactableObject, out TransferableObject tObject) {
        if (interactableObject.layer != LayerInfo.INTERACTABLE_OBJECT) {
            tObject = null;
            return false;
        }
        if (interactableObject.TryGetComponent(out TransferableObject component)) {
            tObject = component;
            return true;
        }
        tObject = null;
        return false;

    }
}
