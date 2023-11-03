using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBehaviour : MonoBehaviour {
    List<LevelStateTrigger> activatedTriggers = new();
    [SerializeField] InterfaceBehaviour interfaceBehaviour;
    [SerializeField] PlayerDimensionController playerDimensionController;
    [SerializeField] PlayerBehaviour playerBehaviour;
    [SerializeField] private bool tutorialEnabled = true;


    int index = -1;
    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this);
    }

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
                    interfaceBehaviour.DisableActiveTutorials();
                    break;
            }
        }
    }

    public void AdvanceState(LevelStateTrigger trigger) {
        if (!tutorialEnabled) return;
        if (activatedTriggers.Contains(trigger)) return;

        activatedTriggers.Add(trigger);
        index++;
        interfaceBehaviour.DisplayTutorialMessageByIndex(index);
    }
    public void BackupAState(LevelStateTrigger trigger) {
        index--;
    }
    private void HandleFirstTrigger() {
        if (playerDimensionController.DOGEnabled) {
            interfaceBehaviour.DisableActiveTutorials();
        }
    }
    private void HandleSecondTrigger() {
        if (playerBehaviour.is3D)
            interfaceBehaviour.DisableActiveTutorials();
    }
    private void HandleThirdTrigger() {
        if (playerBehaviour.IsHoldingObject) {
            interfaceBehaviour.DisableActiveTutorials();
        }
    }

}
