using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blobvis : MonoBehaviour
{
    public float shieldDelay;
    public float range;

    GameObject[] enemies;

    void Start()
    {
        StartCoroutine(SpawnShieldCycle());
    }

    IEnumerator SpawnShieldCycle()
    {
        yield return new WaitForSeconds(shieldDelay);
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < range && enemy != gameObject)
            {
                enemy.GetComponent<PathFollowingScript>().shield = true;
            }
        }
        StartCoroutine(SpawnShieldCycle());
    }
}
