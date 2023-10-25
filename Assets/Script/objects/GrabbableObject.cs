using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    [SerializeField] protected ObjectInteractDisplayController interactDisplayController;
    [SerializeField] protected MeshRenderer displayObject3D_Mesh;
    public Vector3 HoldOffset3D = new(0, -2.5f, 8);

    public bool IsBeingHeld = false;

    protected GameObject holder;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Pickup3D(GameObject holder) {
        //disable physics for rigid body
        TogglePhysics(disable: true);

        //disable interaction indicator
        interactDisplayController.SetInteractIndicatorActive(false);
        this.holder = holder;

        //set the holder as the parent to carry it around
        transform.SetParent(holder.transform);
        //set offset to be infront of the player at all times
        transform.localPosition = HoldOffset3D;

        //reset the position of the 3d transform to 0
        //this is an alternative to updating the position of the entire transferable object at all times instead of leaving it in one spot and just moving it when required 
        displayObject3D_Mesh.transform.localPosition = Vector3.zero;
        IsBeingHeld = true;


    }
    protected void TogglePhysics(bool disable) {
        displayObject3D_Mesh.GetComponent<Rigidbody>().isKinematic = disable;
        displayObject3D_Mesh.GetComponent<Collider>().isTrigger = disable;
    }
}
