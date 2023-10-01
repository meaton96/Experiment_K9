using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterfaceBehaviour : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI dogToggleText;
    private string dogToggleTextPrefix = "D.O.G. Mode: ";
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void SetDogToggleText(bool dogIsRangedMode) {
        dogToggleText.text = dogToggleTextPrefix + (dogIsRangedMode ? "Manual" : "Auto");
    }
}
