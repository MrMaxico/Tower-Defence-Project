using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blobvis : MonoBehaviour
{
    public float shieldDelay;
    public float range;

    public GameObject shieldSpellFX;

    GameObject[] enemies;

    void Start()
    {
        StartCoroutine(SpawnShieldCycle());
        GetComponent<PathFollowingScript>().animations.SetTrigger("Spell");
    }

    IEnumerator SpawnShieldCycle()
    {
        yield return new WaitForSeconds(shieldDelay);
        GetComponent<PathFollowingScript>().animations.ResetTrigger("Spell");
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
