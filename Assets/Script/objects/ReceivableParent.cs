using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReceivableParent : MonoBehaviour {

    [SerializeField] GlowPath glowPath;
    protected virtual void Activate() {
        if (glowPath == null) return;
        glowPath.Activate();
    }
    protected virtual void Deactivate() {
        if (glowPath == null) return;
        glowPath.Deactivate();
    }

}
