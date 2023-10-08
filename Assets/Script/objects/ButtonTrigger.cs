using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{

    [SerializeField] private ButtonBehaviour parentButtonScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision) {
        Debug.Log("collision");
        if (parentButtonScript != null && collision.collider.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            parentButtonScript.TryToOpenDoor(collision.collider);
        }
    }
    private void OnCollisionExit(Collision collision) {
        if (parentButtonScript != null && collision.collider.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            parentButtonScript.TryToCloseDoor(collision.collider);
        }
    }



}
