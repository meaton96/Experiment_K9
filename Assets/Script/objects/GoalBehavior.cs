using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBehavior : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Its working");
        if (collision.gameObject.layer == LayerInfo.PLAYER)
            GameObject.Find("LevelManager").GetComponent<LevelManager>().ChangeLevel(1, collision.gameObject);
    }
}
