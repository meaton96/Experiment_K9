using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractRadarController : MonoBehaviour
{
  //  [SerializeField] private PlayerControllerBeta playerScript;
    [SerializeField] private PlayerBehaviour newPlayerScript;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            var tGObject = other.transform.parent;
            if (tGObject.TryGetComponent(out TransferableObject tObject)) {
                newPlayerScript.AddObjectToInRangeList(tObject);
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            var tGObject = other.transform.parent;
            if (tGObject.TryGetComponent(out TransferableObject tObject)) {
                newPlayerScript.RemoveObjectFromRangeList(tObject);
            }
        }
    }
}
