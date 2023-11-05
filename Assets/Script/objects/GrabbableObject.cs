using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrabbableObject : MonoBehaviour {
    [SerializeField] protected ObjectInteractDisplayController interactDisplayController;
    public MeshRenderer displayObject3D_Mesh;
    public Vector3 HoldOffset3D = new(0, -2.5f, 8);

    public bool IsBeingHeld = false;
    public bool Is3D = true;
    protected GameObject holder;
    public bool isColliding;
    // Start is called before the first frame update
    void Start() {
        Is3D = true;

    }
    //handles picking up the object when in 3d
    public void Pickup3D(GameObject holder, Transform holdArea) {
        //get the rigid body, disable gravity, set drag to 10, freeze rotation, set parent to the hold area
        var rb3D = displayObject3D_Mesh.GetComponent<Rigidbody>();
        rb3D.useGravity = false;
        rb3D.drag = 10;
        rb3D.constraints = RigidbodyConstraints.FreezeRotation;
        IsBeingHeld = true;
        displayObject3D_Mesh.transform.parent = holdArea;

        //disable interaction indicator
        interactDisplayController.SetInteractIndicatorActive(false);
        this.holder = holder;

    }
    public virtual void DropObject() {
        Drop3D();
    }
    protected void Drop3D() {
       //reset the rigid body, enable gravity, set drag to 1, unfreeze rotation, set parent to null
        var rb3D = displayObject3D_Mesh.GetComponent<Rigidbody>();
        displayObject3D_Mesh.transform.parent = transform;
        transform.position = displayObject3D_Mesh.transform.position;
        displayObject3D_Mesh.transform.localPosition = Vector3.zero;
        rb3D.useGravity = true;
        rb3D.drag = 1;
        rb3D.constraints = RigidbodyConstraints.None;
        interactDisplayController.SetInteractIndicatorActive(true);
        holder = null;
        IsBeingHeld = false;
        transform.parent = null;
    }
}
