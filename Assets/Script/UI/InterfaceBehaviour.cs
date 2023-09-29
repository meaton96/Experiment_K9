using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterfaceBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dogToggleText;
    private string dogToggleTextPrefix = "D.O.G. Toggle: ";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void SetDogToggleText(bool dogEnabled) {
        dogToggleText.text = dogToggleTextPrefix + (dogEnabled ? "On" : "Off");  
    }
}
