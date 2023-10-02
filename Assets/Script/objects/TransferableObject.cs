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

    [SerializeField] private Vector3 holdOffset = new(0, -2.5f, 8);

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

        //set the hold as the parent to carry it around
        transform.SetParent(holder.transform);
        //set offset to be infront of the player at all times
        transform.localPosition = holdOffset;

        //reset the position of the 3d transform to 0
        //this is an alternative to updating the position of the entire transferable object at all times instead of leaving it in one spot and just moving it when required 
        displayObject_3D.transform.localPosition = Vector3.zero;
        IsBeingHeld = true;
    }
    public void Drop() {
        displayObject_3D.GetComponent<Rigidbody>().isKinematic = false;
        interactDisplayController.SetInteractIndicatorActive(true);
        holder = null;
        IsBeingHeld = false;
        transform.parent = null;    
    }
}
