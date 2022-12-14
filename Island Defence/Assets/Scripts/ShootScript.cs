using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootScript : MonoBehaviour
{
    public GameObject bullet;
    GameObject[] enemies;
    GameObject target;

    public Transform bulletSpawn;

    public Animator anim;

    public float range;
    public float rotationSpeed;
    public float bulletSpeed;
    public float firerate;
    public float recoilTime;
    float closest;
    float hitTimer;

    public int damage;

    public Vector3 offset;
    Vector3 targetDirection;
    Vector3 direction;

    public bool hasSplashDamage;
    bool shot;
    bool animationTriggered;
    bool foundGemThief;

    private void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        damage = GetComponent<TowerValues>().damage[GetComponent<TowerValues>().level];
        range = GetComponent<TowerValues>().range[GetComponent<TowerValues>().level];

        closest = range;
        target = null;
        foundGemThief = false;
        for (int i = 0; i < enemies.Length; i++)
        {
            if (!foundGemThief)
            {
                if (enemies[i].GetComponent<PathFollowingScript>().redGem.activeSelf || enemies[i].GetComponent<PathFollowingScript>().gem.activeSelf)
                {
                    if (Vector3.Distance(transform.position, enemies[i].transform.position) < closest && enemies[i].gameObject.GetComponent<PathFollowingScript>().hp > 0)
                    {
                        closest = Vector3.Distance(transform.position, enemies[i].transform.position);
                        target = enemies[i];
                        foundGemThief = true;
                    }
                }
                else if (Vector3.Distance(transform.position, enemies[i].transform.position) < closest && enemies[i].gameObject.GetComponent<PathFollowingScript>().hp > 0)
                {
                    closest = Vector3.Distance(transform.position, enemies[i].transform.position);
                    target = enemies[i];
                }
            }
            else
            {
                if (enemies[i].GetComponent<PathFollowingScript>().redGem.activeSelf || enemies[i].GetComponent<PathFollowingScript>().gem.activeSelf)
                {
                    if (Vector3.Distance(transform.position, enemies[i].transform.position) < closest && enemies[i].gameObject.GetComponent<PathFollowingScript>().hp > 0)
                    {
                        closest = Vector3.Distance(transform.position, enemies[i].transform.position);
                        target = enemies[i];
                    }
                }
            }
        }
        if (target != null)
        {
            targetDirection = (target.transform.position + offset) - transform.position;
            direction = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direction);

            hitTimer += Time.deltaTime;
            if (hitTimer >= firerate && !shot)
            {
                shot = true;
                GameObject shotBullet = Instantiate(bullet, bulletSpawn.position, Quaternion.LookRotation(direction));
                //shotBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
                shotBullet.GetComponent<BulletScript>().goal = target.transform;
                shotBullet.GetComponent<BulletScript>().bulletSpeed = bulletSpeed;
                shotBullet.GetComponent<BulletScript>().damage = damage;
                shotBullet.GetComponent<BulletScript>().offset = offset;
                if (hasSplashDamage)
                {
                    shotBullet.GetComponent<BulletScript>().splashDamage = true;
                    shotBullet.GetComponent<BulletScript>().splashDamageValue = GetComponent<TowerValues>().splashDamage[GetComponent<TowerValues>().level];
                }
                hitTimer = 0;
                shot = false;
                animationTriggered = false;
            }

            if (hitTimer >= firerate - recoilTime && !animationTriggered)
            {
                StartCoroutine(ShootAnimationTrigger());
            }
        }
    }

    IEnumerator ShootAnimationTrigger()
    {
        anim.SetTrigger("Shot");
        animationTriggered = true;
        yield return new WaitForEndOfFrame();
        anim.ResetTrigger("Shot");
    }
}
