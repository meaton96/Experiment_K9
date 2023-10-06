using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractDisplayController : MonoBehaviour
{
    [SerializeField] private GameObject interactIndicator_3D;

    [SerializeField] private float objectIndicatorOffsetY = 2f;


    private bool isDisplayingInteractIndicator = false;
    [SerializeField] private TransferableObject tObject;
    
    // Update is called once per frame
    void Update()
    {
        if (isDisplayingInteractIndicator && tObject.Is3D) {

            var vecToCamera = Camera.main.transform.position - interactIndicator_3D.transform.position;

            interactIndicator_3D.transform.position = transform.position + Vector3.up * objectIndicatorOffsetY;
            interactIndicator_3D.transform.forward = -vecToCamera;

        }
        else {
            //might need to set the transform to be forward
        }
    }

    public void SetInteractIndicatorActive(bool enable) {
        isDisplayingInteractIndicator = enable;
        interactIndicator_3D.SetActive(enable);
    }
    public void ResetPosition() {
        transform.localPosition = Vector3.zero + Vector3.up * objectIndicatorOffsetY;

    }

    private void OnTriggerEnter(Collider other) {
        if (tObject.IsBeingHeld)
            return;
        if (other.gameObject.CompareTag("InteractRadar")) {
            //get the player script to check what dimension its in
            if (other.transform.parent.TryGetComponent(out PlayerControllerBeta playerController)) {
                //make sure both objects are in the same dimension before displaying the indicator
                if ((tObject.Is3D && playerController.IsIn3D()) ||
                    (!tObject.Is3D && !playerController.IsIn3D())) {
                    SetInteractIndicatorActive(true);
                }
            }
            
        }
    }
    private void OnTriggerExit(Collider other) {
        if (tObject.IsBeingHeld)
            return;
        if (other.gameObject.CompareTag("InteractRadar")) {

            SetInteractIndicatorActive(false);
        }
    }

    

}
