using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReceiver : MonoBehaviour {
    [SerializeField] GameObject onLeds;
    [SerializeField] GameObject offLeds;

    bool isOn = false;

    [SerializeField] ActivatablePuzzlePiece puzzlePieceToActivate;
    

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            if (!isOn) {
                onLeds.SetActive(true);
                offLeds.SetActive(false);
                puzzlePieceToActivate.Activate();
                isOn = true;
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            if (isOn) {
                onLeds.SetActive(false);
                offLeds.SetActive(true);
                puzzlePieceToActivate.Deactivate();
                isOn = false;
            }
        }
    }
}
