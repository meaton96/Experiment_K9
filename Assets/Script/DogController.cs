using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DogController : MonoBehaviour
{
    [SerializeField]
    GameObject cameraObject;
    Camera camera;
    private bool threeD = true;
    private float xRotation;
    private float yRotation;
    private float hInput;
    private float vInput;
    private Vector3 oldPos;


    // Start is called before the first frame update
    void Start()
    {
        camera = cameraObject.GetComponent<Camera>();

        //set camera position behind dog
        cameraObject.transform.position = transform.position;
        cameraObject.transform.position -= transform.forward * 20;
        cameraObject.transform.position += new Vector3(0, 8, 0);
        oldPos = transform.position;

        //hide and lock cursor
        //Press escape to see it so you can exit play mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        CameraUpdate();
        MovementUpdate();
    }

    private void CameraUpdate()
    {
        /// we're going to need to lock the mouse
        
        if (threeD)
        {
            //Follow the dog when it moves
            Vector3 followPos = transform.position - oldPos;
            cameraObject.transform.position += followPos;

            //Get mouse inputs
            float mouseX = Input.GetAxisRaw("Mouse X");// * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y");// * Time.deltaTime * sensY;
            yRotation += mouseX;
            xRotation -= mouseY;

            //don't let player rotate camera infintly
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //only do this if they are holding down ctrl key
            if (Keyboard.current.ctrlKey.isPressed)
            {
                //rotate around the dog based on horizontal mouse movement
                cameraObject.transform.RotateAround(transform.position, Vector3.up, 200 * mouseX * Time.deltaTime);

                //rotate camera up and down based on vertical mouse movement
                cameraObject.transform.rotation = Quaternion.Euler(xRotation, cameraObject.transform.rotation.eulerAngles.y, 0);
            }
            else
            {
                //resets camera rotation
                cameraObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                //zeroes out the x and y rotation
                yRotation = 0;
                xRotation = 0;
                //finally resets position
                cameraObject.transform.position = transform.position;
                cameraObject.transform.position -= transform.forward * 20;
                cameraObject.transform.position += new Vector3(0, 8, 0);
            }
        }
    }
    private void MovementUpdate()
    {
        oldPos = transform.position;

        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        Vector3 newPos = new Vector3(transform.position.x + hInput * 10 * Time.deltaTime, transform.position.y, transform.position.z + vInput * 10 * Time.deltaTime);
        transform.position = newPos;
    }

}
