using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractDisplayController : MonoBehaviour {
    [SerializeField] private GameObject interactIndicator_3D;

    [SerializeField] private float objectIndicatorOffsetY = 2f;


    private bool isDisplayingInteractIndicator = false;
    [SerializeField] private GrabbableObject tObject;

    // Update is called once per frame
    void Update() {

        if (isDisplayingInteractIndicator && tObject.Is3D) {

            var vecToCamera = Camera.main.transform.position - interactIndicator_3D.transform.position;

            interactIndicator_3D.transform.position = transform.position + Vector3.up * objectIndicatorOffsetY;
            interactIndicator_3D.transform.forward = -vecToCamera;

        }
        else if (isDisplayingInteractIndicator) {
            //might need to set the transform to be forward
            transform.forward = tObject.transform.forward;
        }
    }

    public void SetInteractIndicatorActive(bool enable) {
        isDisplayingInteractIndicator = enable;
        interactIndicator_3D.SetActive(enable);

    }
    public void ResetPosition() {
        interactIndicator_3D.transform.localPosition = Vector3.up * objectIndicatorOffsetY;
        interactIndicator_3D.transform.forward = -tObject.transform.forward;


    }
    private void OnCollisionEnter(Collision collision) {
        tObject.isColliding = true;
    }
    private void OnCollisionExit(Collision collision) {
        tObject.isColliding = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (tObject.IsBeingHeld) {
            
            return;
        }
        if (other.gameObject.CompareTag("InteractRadar")) {

            var player = GameObject.FindWithTag("PlayerBehavior");
            // print(player);
            if (player.TryGetComponent(out PlayerBehaviour playerBehaviourScript)) {
                // print("worked");
                //make sure both objects are in the same dimension before displaying the indicator
                if ((tObject.Is3D && playerBehaviourScript.IsIn3D()) ||
                    (!tObject.Is3D && !playerBehaviourScript.IsIn3D())) {
                    SetInteractIndicatorActive(true);
                }
            }

        }
    }
    private void OnTriggerExit(Collider other) {
        if (tObject.IsBeingHeld) {
            return;
        }
        if (other.gameObject.CompareTag("InteractRadar")) {

            SetInteractIndicatorActive(false);
        }
    }



}
