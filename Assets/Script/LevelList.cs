using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelList", menuName = "ScriptableObjects/LevelList", order = 1)]
public class LevelList : ScriptableObject
{
    [SerializeField]
    List<string> levelNames = new();

    int index = 0;

    private string Next() {
        index++;
        if (index >= levelNames.Count) {
            index = 0;
        }
        return levelNames[index];
    }
    private string Previous() {
        index--;
        if (index < 0) {
            index = levelNames.Count - 1;
        }
        return levelNames[index];
    }
    private string Current() {
        return levelNames[index];
    }   
}
