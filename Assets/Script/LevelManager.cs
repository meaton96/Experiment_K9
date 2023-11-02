using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.IO;

public class LevelManager : MonoBehaviour {
    private int lvlNum = 1;
    [SerializeField]
    PlayerBehaviour player;
    string[] sceneArray;
    [SerializeField]
    LevelList levelList;

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
        //Debug.Log("teleporting in: " + SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name != "LevelNull")
            player.Move3DPlayerToLocation(GameObject.Find("Spawnpoint").transform.position);
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
        if (dog != null) {
            dog.GetComponentInChildren<InteractRadarController>().clearsurfaces();
        }
        //lvlNum = Modifier ? lvlNum += level : level;
        if (level == 1)
            SceneManager.LoadScene(levelList.Next());
        else if (level == -1)
            SceneManager.LoadScene(levelList.Previous());
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
