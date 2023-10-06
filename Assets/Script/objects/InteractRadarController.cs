using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractRadarController : MonoBehaviour
{
  //  private const int OBJECT_LAYER = 7;
    [SerializeField] private PlayerControllerBeta playerScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            var tGObject = other.transform.parent;
            if (tGObject.TryGetComponent(out TransferableObject tObject)) {
                playerScript.AddObjectToInRangeList(tObject);
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            var tGObject = other.transform.parent;
            if (tGObject.TryGetComponent(out TransferableObject tObject)) {
                playerScript.RemoveObjectFromRangeList(tObject);
            }
        }
    }
}
