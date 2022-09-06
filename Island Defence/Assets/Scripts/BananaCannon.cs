using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaCannon : MonoBehaviour
{
    public GameObject bullet;
    GameObject[] enemies;
    GameObject target;

    public float range;
    public float rotationSpeed;
    public float bulletSpeed;
    float closest;
    float hitTimer;

    Vector3 targetDirection;
    Vector3 direction;

    bool shot;

    private void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        closest = range;
        target = null;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (Vector3.Distance(transform.position, enemies[i].transform.position) < closest)
            {
                closest = Vector3.Distance(transform.position, enemies[i].transform.position);
                target = enemies[i];
            }
        }
        if (target != null)
        {
            targetDirection = target.transform.position - transform.position;
            direction = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direction);

            hitTimer += Time.deltaTime;
            if (hitTimer >= 1.008333333333333 && !shot)
            {
                shot = true;
                GameObject shotBullet = Instantiate(bullet, transform.position + new Vector3(0, 0.4f, 0), Quaternion.LookRotation(direction));
                shotBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
            }

            if (hitTimer >= 1.2)
            {
                hitTimer = 0;
                shot = false;
            }
        }
    }
}
