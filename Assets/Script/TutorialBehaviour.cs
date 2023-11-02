using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBehaviour : MonoBehaviour
{
    List<LevelStateTrigger> activatedTriggers = new();
    [SerializeField] InterfaceBehaviour interfaceBehaviour;
    int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdvanceState(LevelStateTrigger trigger) {
        if (activatedTriggers.Contains(trigger)) return;
        activatedTriggers.Add(trigger);
        index++;
    }
    public void BackupAState(LevelStateTrigger trigger) {
        index--;
    }
    public void HandleFirstTrigger() {

    }
}
