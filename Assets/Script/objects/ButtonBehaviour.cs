using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : ReceivableParent {
    public bool IsLocked = false;
    [SerializeField] private ActivatablePuzzlePiece puzzlePieceToActivate;

    //// visual path of the button
    //[SerializeField] private GameObject glowPath;
    //private MeshRenderer[] glowPathRenderers;

    [SerializeField] private float springForce = 1f;
    private Rigidbody rb;
    [SerializeField] private MeshRenderer pawMeshRenderer;
    //list of 2 to replace both textures on the paw
    [SerializeField] private List<Material> pressedMaterials;
    [SerializeField] private List<Material> unPressedMaterials;
    //the minimum mass the button needs to be in contact with to activate
    [SerializeField] private float minMassToPress = 5f;
    //control to make the button not unpress itself if it was successfully pressed
    [SerializeField] private bool IsPermanentlyPressedOnPress = false;

    private bool unpressed;
    //keeps track of who or what pressed the button to check its mass against minimum required
    private GameObject presser;

    // Start is called before the first frame update
    void Awake() {

        rb = GetComponent<Rigidbody>();
        //if (glowPath != null)
        //    glowPathRenderers = glowPath.GetComponentsInChildren<MeshRenderer>();
    }

    void Update() {
        if (!IsPermanentlyPressedOnPress)
            UnPress();
    }



    //move the button up if it is not at its starting position
    void UnPress() {
        if (transform.localPosition.y < -0.01f) {
            unpressed = false;
            rb.AddForce(Vector3.up * springForce);
        }
        //clear and reset everything to 0
        else if (!unpressed) {
            unpressed = true;
            var tempVec = transform.localPosition;
            tempVec.y = 0;
            transform.localPosition = tempVec;
            //rb.velocity = Vector3.zero;
            //rb.velocity = Vector3.zero;
        }
    }


    private void OnCollisionEnter(Collision collision) {
        //button hit the trigger cube
        //check if the button can be pressed and open door and swap material if so
        // print(collision);
        if (collision.collider.CompareTag("EventTrigger")) {
            presser = collision.gameObject;
            if (CanPressButton()) {
                
                Activate();
             //   pawMeshRenderer.SetMaterials(pressedMaterials);
                //foreach (MeshRenderer path in glowPathRenderers) {
                //    path.SetMaterials(pressedMaterials);
                //}
            }
        }
        //otherwise the collision was with something trying to press it so store it as the presser
        else {
            presser = collision.gameObject;
        }

    }
    protected override void Activate() {
        base.Activate();
        pawMeshRenderer.SetMaterials(pressedMaterials);
        puzzlePieceToActivate.Activate();
        //foreach (MeshRenderer path in glowPathRenderers) {
        //    path.SetMaterials(pressedMaterials);
        //}
    }
    protected override void Deactivate() {
        pawMeshRenderer.SetMaterials(unPressedMaterials);
        //foreach (MeshRenderer path in glowPathRenderers) {
        //    path.SetMaterials(unPressedMaterials);
        //}
        puzzlePieceToActivate.Deactivate();

        base.Deactivate();
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.collider.CompareTag("EventTrigger")) {
            Deactivate();
        }
        else {
            //null the presser if they leave the collision with the button
            if (collision.gameObject == presser)
                presser = null;
        }

    }
    //checks if the button can be pressed by checking presser null then for player presser
    //then checks for mass on the attached rigid body
    //if no rigid body is found also return false
    private bool CanPressButton() {
        if (presser == null) return false;
        if (presser.layer == LayerInfo.PLAYER) return true;
        if (presser.layer == 13) return true;
        if (presser.TryGetComponent(out Rigidbody rb)) {
            return rb.mass > minMassToPress;
        }
        return false;
    }
}
