using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStateTrigger : MonoBehaviour
{

    [SerializeField] TutorialBehaviour tutorialBehaviour;

    private void OnTriggerEnter(Collider other) {
        tutorialBehaviour.AdvanceState(this);
    }
    
}
