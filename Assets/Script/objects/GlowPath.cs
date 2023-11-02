using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowPath : MonoBehaviour {
    [SerializeField] private GameObject pathInactive, pathActive;

    public void Activate() {
        pathActive.SetActive(true);
        pathInactive.SetActive(false);
    }
    public void Deactivate() {
        pathInactive.SetActive(true);
        pathActive.SetActive(false);
    }
}
