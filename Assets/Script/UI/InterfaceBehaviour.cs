using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterfaceBehaviour : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI dogModeText;
    [SerializeField] private TextMeshProUGUI dogAutoEnabledText;
    private string dogToggleTextPrefix = "D.O.G. Mode: ";
    private string dogEnabledPrefix = "D.O.G. Transfer: ";
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void SetDogToggleText(bool dogIsRangedMode) {
        dogModeText.text = dogToggleTextPrefix + (dogIsRangedMode ? "Manual" : "Auto");
        dogAutoEnabledText.gameObject.SetActive(!dogIsRangedMode);
    }
    public void SetDogAutoEnabledText(bool isAutoEnabled) {
        dogAutoEnabledText.text = dogEnabledPrefix + (isAutoEnabled ? "On" : "Off");
    }
}
