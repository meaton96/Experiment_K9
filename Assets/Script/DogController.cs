using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField]
    GameObject cameraObject;
    Camera camera;
    bool threeD = true;

    // Start is called before the first frame update
    void Start()
    {
        camera = cameraObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraUpdate();
    }

    private void CameraUpdate()
    {
        //Unfinished
        if (threeD)
        {
            Vector3 followPos = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
            cameraObject.transform.position = followPos;

            cameraObject.transform.rotation = transform.rotation;
        }
    }
}
