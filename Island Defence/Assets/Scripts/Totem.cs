using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
    GameObject[] towers;
    public GameObject[] enemies;

    public float range;
    public float slownessLevel;

    private void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            if (Vector3.Distance(transform.position, enemies[i].transform.position) < range)
            {
                enemies[i].GetComponent<PathFollowingScript>().slowness = enemies[i].GetComponent<PathFollowingScript>().speed / slownessLevel;
            }
            else
            {
                enemies[i].GetComponent<PathFollowingScript>().slowness = 0;
            }
        }
    }
}
