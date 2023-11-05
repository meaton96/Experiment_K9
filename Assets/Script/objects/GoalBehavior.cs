using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBehavior : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerInfo.PLAYER)
            GameObject.Find("LevelManager").GetComponent<LevelManager>().IncrementLevel();
    }
}
