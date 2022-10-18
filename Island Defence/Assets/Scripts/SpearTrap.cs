using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearTrap : MonoBehaviour
{
    GameObject[] enemies;

    public Animator anim;
    public AudioSource attackSound;

    private void Start()
    {
        StartCoroutine(DamageEnemies());
        Debug.DrawLine(transform.position, transform.position - transform.forward * .8f, Color.cyan, 1000f, false);
    }

    private IEnumerator DamageEnemies()
    {
        anim.ResetTrigger("Restart");
        attackSound.Play();
        attackSound.pitch = Random.Range(0.8f, 1.2f);
        TowerValues towerValuesScript = GetComponent<TowerValues>();
        yield return new WaitForSeconds(towerValuesScript.fireRate[towerValuesScript.level] / 6);
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemie in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemie.transform.position);
            if (distance <= 0.8f)
            {
                enemie.GetComponent<PathFollowingScript>().hp -= towerValuesScript.damage[towerValuesScript.level];
                Debug.Log(enemie.GetComponent<PathFollowingScript>().hp);
            }
        }

        yield return new WaitForSeconds(towerValuesScript.fireRate[towerValuesScript.level] / 6 * 5);

        anim.SetTrigger("Restart");
        yield return new WaitForEndOfFrame();
        StartCoroutine(DamageEnemies());
    }
}
