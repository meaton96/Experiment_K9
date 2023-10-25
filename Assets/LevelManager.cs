using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    private int lvlNum = 0;
    GameObject dog;
    bool teleported;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        TestChange();
        if (SceneManager.GetActiveScene().name == "Level1" && !teleported)
        {
            teleported = true;
            Debug.Log("teleporting ");
            dog.transform.parent.transform.parent.GetComponentInChildren<PlayerBehaviour>().Move3DPlayerToLocation(new Vector3(-180, 5, -302));
            
        }
    }

    //Modifier is a bool that represents whether the int, level, should be added to the current level number
    //or if the level should be set to int level.
    //Default is a modifier of 1 (AKA Next level)
    public void ChangeLevel(int level = 1, bool Modifier = true, GameObject dog = null)
    {
        if (dog != null)
        {
            dog.GetComponentInChildren<InteractRadarController>().clearsurfaces();
        }
        lvlNum = Modifier ? lvlNum += level : level;
        string sceneName = "Level" + lvlNum;
        this.dog = dog;
        SceneManager.LoadScene(sceneName);            
    }

    private void TestChange()
    {
        if (Keyboard.current.pageUpKey.wasPressedThisFrame)
            ChangeLevel();
        if (Keyboard.current.pageDownKey.wasPressedThisFrame)
            ChangeLevel(-1);
        if (Keyboard.current.homeKey.wasPressedThisFrame)
            ChangeLevel(0, false);
        if (SceneManager.GetActiveScene().name == "LevelNull")
            ChangeLevel(0, false);
    }
}
