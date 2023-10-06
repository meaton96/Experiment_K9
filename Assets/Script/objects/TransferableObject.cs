using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferableObject : MonoBehaviour {
    [SerializeField] private MeshRenderer displayObject3D_Mesh;
    [SerializeField] private GameObject displayObject_2D;
    [SerializeField] private ObjectInteractDisplayController interactDisplayController;

    [SerializeField] private float objectDrawOffset = 40f;

    public bool Is3D = true;
    public bool IsBeingHeld = false;

    private GameObject holder;

    public Vector3 HoldOffset3D = new(0, -2.5f, 8);

    private void Awake() {
        //turn off the physics if the object starts as 2D
        if (!Is3D) { 
            
            Disable3D();
        }
    }
    private void Update() {
        if (!Is3D) {
            if (displayObject_2D.transform.localPosition != Vector3.zero) {
                //reset position of sphere and this objec to the location of the 2d sprite while in 2d mode
                //reset position of the 3d object to the 2d object location
                transform.position = displayObject_2D.transform.position;
                displayObject_2D.transform.localPosition = Vector3.zero;
                
            }
        }
    }

    public void Set3DDisplayMode(bool is3D) {
        Is3D = is3D;

    }
    public void Enable3D() {
        displayObject3D_Mesh.enabled = true;
    }
    public void Enable2D() {
        displayObject_2D.SetActive(true);
       
    }
    public void Disable3D() {
        displayObject3D_Mesh.GetComponent<Rigidbody>().isKinematic = true;
        displayObject3D_Mesh.enabled = false;
        Is3D = false;
    }
    public void Disable2D() {
        displayObject_2D.SetActive(false);
        Is3D = true;
    }
    public void Pickup3D(GameObject holder) {
        //disable physics for rigid body
        displayObject3D_Mesh.GetComponent<Rigidbody>().isKinematic = true;

        //disable interaction indicator
        interactDisplayController.SetInteractIndicatorActive(false);
        this.holder = holder;

        //set the hold as the parent to carry it around
        transform.SetParent(holder.transform);
        //set offset to be infront of the player at all times
        transform.localPosition = HoldOffset3D;

        //reset the position of the 3d transform to 0
        //this is an alternative to updating the position of the entire transferable object at all times instead of leaving it in one spot and just moving it when required 
        displayObject3D_Mesh.transform.localPosition = Vector3.zero;
        IsBeingHeld = true;
    }
    public void Pickup2D(GameObject holder) {


        //set the hold as the parent to carry it around
        transform.SetParent(holder.transform);
        //set offset to be infront of the player at all times
        transform.localPosition = -Vector3.forward * objectDrawOffset;

        //disable interaction indicator
        interactDisplayController.SetInteractIndicatorActive(false);
        this.holder = holder;

        //reset the position of the 3d transform to 0
        //this is an alternative to updating the position of the entire transferable object at all times instead of leaving it in one spot and just moving it when required 
        displayObject3D_Mesh.transform.localPosition = Vector3.zero;
        IsBeingHeld = true;
    }
    public void Drop3D() {
        displayObject3D_Mesh.GetComponent<Rigidbody>().isKinematic = false;
        interactDisplayController.SetInteractIndicatorActive(true);
        holder = null;
        IsBeingHeld = false;
        transform.parent = null;    
    }
    public void Drop2D() {
        interactDisplayController.SetInteractIndicatorActive(true);

        //not resetting the position of the indicator
        //shows up underneath the object when dropped
        interactDisplayController.ResetPosition();


        holder = null;
        IsBeingHeld = false;
        transform.parent = null;
    }
    public void ProjectOntoWallAtLocation(GameObject nearestWall, Vector3 projectionDrawCenter, float playerProjectionSize, float wallDrawOffset) {

        var drawLoc = projectionDrawCenter + -Vector3.forward * objectDrawOffset;
        
        if (displayObject_2D.activeInHierarchy == false) {

            //activate and set position and rotation of the projection
            displayObject_2D.transform.position = drawLoc;
            displayObject_2D.transform.forward = nearestWall.transform.up;
            Enable2D();
        }
        //always move the projection while it is enabled 
        displayObject_2D.transform.position = drawLoc + (nearestWall.transform.up * wallDrawOffset);
        displayObject_2D.transform.forward = nearestWall.transform.up;
        
    }
    public void DisableProjection() {
        displayObject_2D.SetActive(false);
    }
}
