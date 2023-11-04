using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.IO;
using System.Collections.Generic;

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
            ChangeLevel();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode load) {
        // player3D.SetActive(false);
        //Debug.Log("teleporting in: " + SceneManager.GetActiveScene().name);
         if (SceneManager.GetActiveScene().name != "LevelNull")
            SpawnPlayer();
        //    player.Move3DPlayerToLocation(GameObject.Find("Spawnpoint").transform.position);
        //player3D.SetActive(true);   

    }
    private void SpawnPlayer() {
        var playerWrap = GameObject.FindWithTag("PlayerWrapper");
        if (playerWrap == null) {
            throw new System.Exception("Player not found in scene");
        }
        //player was found so spawn the player at the spawnpoint

        playerWrap.transform.position = new Vector3(1000, 1000, 1000);
        playerWrap.SetActive(true);

        //var player3D = playerWrap.transform.GetChild(0).transform.GetChild(2).gameObject;

        //if (player3D == null) {
        //    throw new System.Exception("Player3D not found in scene");
        //}
        

        var playerBehaviour = playerWrap.transform.GetChild(1).gameObject;

        if (playerBehaviour.TryGetComponent(out PlayerBehaviour player)) {
            player.Spawn();
        }
        else {
            throw new System.Exception("Player does not have playerbehaviour");
        }


        


       // if (SceneManager.GetActiveScene().name != "LevelNull")
         //   player.Move3DPlayerToLocation(GameObject.Find("Spawnpoint").transform.position);

    }


    //private void InitializeCatelog() {
    //    //Grabs all levels in the 'levels' asset bundle and loads them to an array of strings to be accessed by ChangeLevel
    //    AssetBundle myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "levels"));
    //    if (myLoadedAssetBundle == null) {
    //        Debug.Log("Failed to load AssetBundle!");
    //        return;
    //    }
    //    sceneArray = myLoadedAssetBundle.GetAllScenePaths();
    //}

    //Modifier is a bool that represents whether the int, level, should be added to the current level number
    //or if the level should be set to int level.
    //Default is a modifier of 1 (AKA Next level)
    public void ChangeLevel(int level = 1, GameObject dog = null) {
        //if (dog != null) {
        //    if (TryGetComponent(out InteractRadarController dogRadar)) {
        //        dogRadar.clearsurfaces();
        //    }
        //    else {
        //        throw new System.Exception("Dog does not have radarcontroller");
        //    }
        //}
        //lvlNum = Modifier ? lvlNum += level : level;
        if (level == 1)
            SceneManager.LoadScene(Next());
        else if (level == -1)
            SceneManager.LoadScene(Previous());
    }


    private void TestChange() {
        if (Keyboard.current.pageUpKey.wasPressedThisFrame)
            ChangeLevel();
        if (Keyboard.current.pageDownKey.wasPressedThisFrame)
            ChangeLevel(-1);
        //if (Keyboard.current.homeKey.wasPressedThisFrame)
        //    ChangeLevel(0, false);
    }
}
