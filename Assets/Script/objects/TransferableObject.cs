using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferableObject : GrabbableObject {
    
    [SerializeField] private GameObject displayObject_2D;
    [SerializeField] private SpriteRenderer spriteRenderer2D;
    [SerializeField] private float objectDrawOffset = 4f;

    
    

    private void Awake() {
        spriteRenderer2D = displayObject_2D.GetComponent<SpriteRenderer>();
        //turn off the physics if the object starts as 2D
        //interactDisplayController.SetInteractIndicatorActive(true);
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
        Set3DDisplayMode(true);
        displayObject3D_Mesh.enabled = true;
    }
    public void Enable2D() {
        displayObject_2D.SetActive(true);
       
    }
    public void Disable3D() {
        TogglePhysics(disable: true);
        displayObject3D_Mesh.enabled = false;
        Is3D = false;
    }
    public void Disable2D() {
        displayObject_2D.SetActive(false);
        Is3D = true;
    }
    
    public void SetHolderAndOffset(GameObject holder, Vector3 offset) {
        Debug.Log("setting hold: " + holder.name);
        
        this.holder = holder;   
        transform.parent = holder.transform;
        transform.localPosition = offset;
    }
    public void Pickup2D(GameObject holder) {
        

        //set the hold as the parent to carry it around
        transform.SetParent(holder.transform);
        spriteRenderer2D.enabled = false;

        //disable interaction indicator
        interactDisplayController.SetInteractIndicatorActive(false);
        this.holder = holder;

        transform.localPosition = Vector3.zero;

        IsBeingHeld = true;
    }
    public override void DropObject() {
        if (Is3D) 
            Drop3D();
        else
            Drop2D();
    }
    

    
    public void Drop2D() {
        Debug.Log("Dropping 2d object");
        
        transform.forward = holder.transform.forward;

        //calculate an offset based on the players right axis
        bool facingRight = holder.GetComponent<MovementController_2D>().IsFlipped();
        var offset = (facingRight ? objectDrawOffset : -objectDrawOffset) * holder.transform.right;
  
        displayObject_2D.SetActive(true);   
        spriteRenderer2D.enabled = true;

        

        holder = null;
        IsBeingHeld = false;
        transform.parent = null;
        transform.position += offset;

        interactDisplayController.SetInteractIndicatorActive(true);
        interactDisplayController.ResetPosition();
    }
    
}
