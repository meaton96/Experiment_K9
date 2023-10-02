using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferableObject : MonoBehaviour {
    [SerializeField] private GameObject displayObject_3D;
    [SerializeField] private GameObject displayObject_2D;
    [SerializeField] private ObjectInteractDisplayController interactDisplayController;

    public bool Is3D = true;
    public bool IsBeingHeld = false;

    private GameObject holder;
    private Vector3 holderPreviousPos;
    [SerializeField] private float objectHoldOffsetForward = 2f;
    [SerializeField] private float objectHoldOffsetUpward = 4f;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (IsBeingHeld && holder != null) {
            transform.position += holder.transform.position - holderPreviousPos;
            holderPreviousPos = holder.transform.position;
        }
        
        

    }

    public void Set3DDisplayMode(bool is3D) {
        Is3D = is3D;

    }
    public void Enable3D() {
        displayObject_3D.SetActive(true);
    }
    public void Enable2D() {
        displayObject_2D.SetActive(true);
       
    }
    public void Disable3D() {
        displayObject_3D.SetActive(false);
        Is3D = false;
    }
    public void Disable2D() {
        displayObject_2D.SetActive(false);
        Is3D = true;
    }
    public void Pickup(GameObject holder) {
        //disable physics for rigid body
        displayObject_3D.GetComponent<Rigidbody>().isKinematic = true;

        //disable interaction indicator
        interactDisplayController.SetInteractIndicatorActive(false);
        this.holder = holder;
        holderPreviousPos = holder.transform.position;

        transform.position = holder.transform.position + holder.transform.forward * objectHoldOffsetForward + Vector3.up * objectHoldOffsetUpward;
        displayObject_3D.transform.localPosition = Vector3.zero;
        IsBeingHeld = true;
    }
    public void Drop() {
        displayObject_3D.GetComponent<Rigidbody>().isKinematic = false;
        interactDisplayController.SetInteractIndicatorActive(true);
        holder = null;
        holderPreviousPos = Vector3.zero;
        IsBeingHeld = false;
    }
}
