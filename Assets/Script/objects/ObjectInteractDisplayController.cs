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
    }

    public void SetInteractIndicatorActive(bool enable) {
        isDisplayingInteractIndicator = enable;
        interactIndicator_3D.SetActive(enable);
    }

    private void OnTriggerEnter(Collider other) {
        if (tObject.IsBeingHeld)
            return;
        if (other.gameObject.CompareTag("InteractRadar")) {
            SetInteractIndicatorActive(true);
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
