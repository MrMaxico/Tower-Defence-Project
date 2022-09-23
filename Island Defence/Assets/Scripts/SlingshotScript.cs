using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotScript : MonoBehaviour
{
    public GameObject bullet;
    public GameObject target;

    public Transform bulletSpawn;

    public float range;
    public float rotationSpeed;
    public float bulletSpeed;
    public float firerate;
    float closest;
    float hitTimer;

    public int damage;

    public Vector3 offset;
    Vector3 targetDirection;
    Vector3 direction;

    bool shot;
    bool foundGemThief;

    private void Update()
    {
        targetDirection = (target.transform.position + offset) - transform.position;
        direction = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);

        hitTimer += Time.deltaTime;
        if (hitTimer >= firerate - .5f && !shot)
        {
            shot = true;
            GameObject shotBullet = Instantiate(bullet, bulletSpawn.position, Quaternion.LookRotation(direction));
            shotBullet.GetComponent<DurianScript>().goal = target.transform;
            shotBullet.GetComponent<DurianScript>().bulletSpeed = bulletSpeed;
            shotBullet.GetComponent<DurianScript>().damage = damage;
        }

        if (hitTimer >= firerate)
        {
            hitTimer = 0;
            shot = false;
        }
    }
}
