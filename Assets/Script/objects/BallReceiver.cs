using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReceiver : MonoBehaviour {
    [SerializeField] GameObject onLeds;
    [SerializeField] GameObject offLeds;

    [SerializeField] GameObject glowPath;
    [SerializeField] private List<Material> pressedMaterials;
    [SerializeField] private List<Material> unPressedMaterials;
    private MeshRenderer[] glowPathRenderers;

    bool isOn = false;

    [SerializeField] ActivatablePuzzlePiece puzzlePieceToActivate;


    // Start is called before the first frame update
    void Awake()
    {
        if (glowPath != null)
            glowPathRenderers = glowPath.GetComponentsInChildren<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerInfo.INTERACTABLE_OBJECT) {
            if (!isOn) {
                onLeds.SetActive(true);
                offLeds.SetActive(false);
                puzzlePieceToActivate.Activate();
                isOn = true;
                if (glowPathRenderers != null)
                    foreach (MeshRenderer path in glowPathRenderers)
                    {
                        path.SetMaterials(pressedMaterials);
                    }
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
                foreach (MeshRenderer path in glowPathRenderers)
                {
                    path.SetMaterials(unPressedMaterials);
                }
            }
        }
    }
}
