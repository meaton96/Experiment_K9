using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBehaviour : MonoBehaviour {
    List<LevelStateTrigger> activatedTriggers = new();
 //   InterfaceBehaviour interfaceBehaviour;
 //   PlayerDimensionController playerDimensionController;
   // [SerializeField] PlayerBehaviour playerBehaviour;
  //  PickupController pickupController;
    [SerializeField] private bool tutorialEnabled = true;


    int index = -1;


    // Update is called once per frame
    void Update() {
        if (tutorialEnabled) {
            switch (index) {
                case 0:
                    HandleFirstTrigger();
                    break;
                case 1:
                    HandleSecondTrigger();
                    break;
                case 2:
                    HandleThirdTrigger();
                    break;
                case 3:
                    HandleThirdTrigger();
                    break;
                default:
                    PlayerBehaviour.Instance.interfaceScript.DisableActiveTutorials();
                    break;
            }
        }
    }

    public void AdvanceState(LevelStateTrigger trigger) {
        if (!tutorialEnabled) return;
        if (activatedTriggers.Contains(trigger)) return;

        activatedTriggers.Add(trigger);
        index++;
        PlayerBehaviour.Instance.interfaceScript.DisplayTutorialMessageByIndex(index);
    }
    public void BackupAState(LevelStateTrigger trigger) {
        index--;
    }
    private void HandleFirstTrigger() {
        if (PlayerBehaviour.Instance.playerDimensionController.DOGEnabled) {
            PlayerBehaviour.Instance.interfaceScript.DisableActiveTutorials();
        }
    }
    private void HandleSecondTrigger() {
        if (PlayerBehaviour.Instance.is3D)
            PlayerBehaviour.Instance.interfaceScript.DisableActiveTutorials();
    }
    private void HandleThirdTrigger() {
        if (PlayerBehaviour.Instance.pickupController.IsHoldingObject()) {
            PlayerBehaviour.Instance.interfaceScript.DisableActiveTutorials();
        }
    }

}
