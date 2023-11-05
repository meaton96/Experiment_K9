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

    // Update is called once per frame
    void Update() {
    }
    public void Pickup3D(GameObject holder, Transform holdArea) {
    //    Debug.Log(holder.name + " is picking up " + gameObject.name);
        //transform.position = displayObject3D_Mesh.transform.position;
        //displayObject3D_Mesh.transform.localPosition = Vector3.zero;
        var rb3D = displayObject3D_Mesh.GetComponent<Rigidbody>();
        rb3D.useGravity = false;
        rb3D.drag = 10;
        rb3D.constraints = RigidbodyConstraints.FreezeRotation;
        IsBeingHeld = true;
        displayObject3D_Mesh.transform.parent = holdArea;

        //disable physics for rigid body
        // TogglePhysics(disable: true);

        //  transform.SetParent(holder.transform);
        //  transform.localPosition =  HoldOffset3D;
          
        //   ToggleGravity(false);
        //  TogglePhysics(disable: true);


        //disable interaction indicator
        interactDisplayController.SetInteractIndicatorActive(false);
        this.holder = holder;

        //set the holder as the parent to carry it around
        // transform.SetParent(holder.transform);
        // //set offset to be infront of the player at all times
        //   transform.localPosition = HoldOffset3D;

        //reset the position of the 3d transform to 0
        //this is an alternative to updating the position of the entire transferable object at all times instead of leaving it in one spot and just moving it when required 

        // IsBeingHeld = true;


    }
    public virtual void DropObject() {
        Drop3D();
    }
    //protected void TogglePhysics(bool disable) {
    //    displayObject3D_Mesh.GetComponent<Rigidbody>().isKinematic = disable;
    //    //displayObject3D_Mesh.GetComponent<Rigidbody>().useGravity = !disable;
    //    displayObject3D_Mesh.GetComponent<Collider>().isTrigger = disable;
    //}
    protected void ToggleGravity(bool enable) {
        displayObject3D_Mesh.GetComponent<Rigidbody>().useGravity = enable;
    }
    protected void Drop3D() {
       
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
