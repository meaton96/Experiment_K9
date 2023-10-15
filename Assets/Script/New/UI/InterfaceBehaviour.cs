using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterfaceBehaviour : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI dogModeText;
    [SerializeField] private TextMeshProUGUI dogAutoEnabledText;
    private string dogToggleTextPrefix = "D.O.G. Mode: ";
    private string dogEnabledPrefix = "D.O.G. Transfer: ";

    public void SetDogToggleText(bool dogIsRangedMode) {
        dogModeText.text = dogToggleTextPrefix + (dogIsRangedMode ? "Manual" : "Auto");
        dogAutoEnabledText.gameObject.SetActive(!dogIsRangedMode);
    }
    public void SetDogAutoEnabledText(bool isAutoEnabled) {
        dogAutoEnabledText.text = dogEnabledPrefix + (isAutoEnabled ? "On" : "Off");
    }
}