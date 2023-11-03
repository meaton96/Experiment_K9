using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterfaceBehaviour : MonoBehaviour {
    //  [SerializeField] private TextMeshProUGUI dogModeText;
    [SerializeField] private TextMeshProUGUI dogAutoEnabledText;
    // private string dogToggleTextPrefix = "D.O.G. Mode: ";
    private string dogEnabledPrefix = "D.O.G. Transfer: ";
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private List<GameObject> tutorialMessages;
    

    public void SetDogToggleText(bool dogIsRangedMode) {
        //  dogModeText.text = dogToggleTextPrefix + (dogIsRangedMode ? "Manual" : "Auto");
        dogAutoEnabledText.gameObject.SetActive(!dogIsRangedMode);
    }
    public void SetDogAutoEnabledText(bool isAutoEnabled) {
        dogAutoEnabledText.text = dogEnabledPrefix + (isAutoEnabled ? "On" : "Off");
    }

    public void Pause() {
        
        pauseMenu.SetActive(true);
    }
    public void UnPause() {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        
    }
    public void DisplayTutorialMessageByIndex(int index) {
        
        if (index < 0 || index >= tutorialMessages.Count) return;

        tutorialMessages[index].SetActive(true);
        for (int x = 0; x < tutorialMessages.Count; x++) {
            if (x != index) {
                tutorialMessages[x].SetActive(false);
            }
        }
    }
    public void DisableActiveTutorials() {
        for (int x = 0; x < tutorialMessages.Count; x++) {
            tutorialMessages[x].SetActive(false);

        }
    }
}
