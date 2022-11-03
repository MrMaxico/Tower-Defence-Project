using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Transform goal;

    public Vector3 offset;

    public float bulletSpeed;
    public float splashRange;

    public int damage;
    public int splashDamageValue;

    public bool splashDamage;

    public GameObject hitEffect;

    GameObject[] enemies;

    void Update()
    {
        if (goal != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, goal.position + offset, bulletSpeed * Time.deltaTime);

            if (transform.position == goal.position + offset)
            {
                if (goal.GetComponent<PathFollowingScript>().shield)
                {
                    goal.GetComponent<PathFollowingScript>().shield = false;
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    goal.GetComponent<PathFollowingScript>().hp -= damage;
                    enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    GameObject deathPoof = Instantiate(hitEffect, transform.position, transform.rotation);
                    deathPoof.transform.Rotate(new Vector3(-90, 0, 0));
                    foreach (GameObject enemy in enemies)
                    {
                        if (Vector3.Distance(enemy.transform.position, transform.position) <= splashRange && splashDamage)
                        {
                            enemy.GetComponent<PathFollowingScript>().hp -= splashDamageValue;
                        }
                    }
                }
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
