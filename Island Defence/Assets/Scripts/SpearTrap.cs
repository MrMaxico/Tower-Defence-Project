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

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= 0.8f)
            {
                if (enemy.GetComponent<PathFollowingScript>().shield)
                {
                    enemy.GetComponent<PathFollowingScript>().shield = false;
                }
                else
                {
                    enemy.GetComponent<PathFollowingScript>().hp -= towerValuesScript.damage[towerValuesScript.level];
                }
            }
        }

        yield return new WaitForSeconds(towerValuesScript.fireRate[towerValuesScript.level] / 6 * 5);

        anim.SetTrigger("Restart");
        yield return new WaitForEndOfFrame();
        StartCoroutine(DamageEnemies());
    }
}
