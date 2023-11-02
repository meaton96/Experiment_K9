using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReceiver_2D : ReceivableParent {
    [SerializeField] GameObject outsideOff;
    [SerializeField] GameObject outsideOn;
    [SerializeField] ActivatablePuzzlePiece puzzlePieceToActivate;
    [SerializeField] bool Allow3DActivation = false;

    protected override void Activate() {
        base.Activate();
        puzzlePieceToActivate.Activate();
        outsideOff.SetActive(false);
        outsideOn.SetActive(true);
    }
    protected override void Deactivate() {
        base.Deactivate();
        puzzlePieceToActivate.Deactivate();
        outsideOff.SetActive(true);
        outsideOn.SetActive(false);
    }
    private void OnTriggerEnter(Collider other) {
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
