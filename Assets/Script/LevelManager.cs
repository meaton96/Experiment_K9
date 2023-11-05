using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.IO;
using System.Collections.Generic;
using static Cinemachine.DocumentationSortingAttribute;

public class LevelManager : MonoBehaviour {
    private int lvlNum = 1;
 //   [SerializeField] private GameObject player3D;
    [SerializeField]
 //   PlayerBehaviour player;
    string[] sceneArray;
    [SerializeField]
    List<string> levelNames = new();

    int index = 0;

    public string Next() {
        index++;
        if (index >= levelNames.Count) {
            index = 0;
        }
        return levelNames[index];
    }
    public string Previous() {
        index--;
        if (index < 0) {
            index = levelNames.Count - 1;
        }
        return levelNames[index];
    }
    public string Current() {
        return levelNames[index];
    }

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        //InitializeCatelog();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Update() {
        //LevelNull is just an empty level with the Level Manager and the PlayerWrapper in it.
        if (SceneManager.GetActiveScene().name == "LevelNull")
            IncrementLevel();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode load) {
         if (SceneManager.GetActiveScene().name != "LevelNull")
            SpawnPlayer();

    }
    private void SpawnPlayer() {
        var playerWrap = GameObject.FindWithTag("PlayerWrapper");
        if (playerWrap == null) {
            throw new System.Exception("Player not found in scene");
        }
        //player was found so spawn the player at the spawnpoint
        playerWrap.transform.position = new Vector3(1000, 1000, 1000);
        playerWrap.SetActive(true);

        var playerBehaviour = playerWrap.transform.GetChild(1).gameObject;

        if (playerBehaviour.TryGetComponent(out PlayerBehaviour player)) {
            player.Spawn();
        }
        else {
            throw new System.Exception("Player does not have playerbehaviour");
        }
    }

    //Modifier is a bool that represents whether the int, level, should be added to the current level number
    //or if the level should be set to int level.
    //Default is a modifier of 1 (AKA Next level)
    public void IncrementLevel() {
            SceneManager.LoadScene(Next());
        
    }
    public void DecrementLevel() {
            SceneManager.LoadScene(Previous());
    }
    private void TestChange() {
        if (Keyboard.current.pageUpKey.wasPressedThisFrame)
            IncrementLevel();
        if (Keyboard.current.pageDownKey.wasPressedThisFrame)
            DecrementLevel();
    }
}
