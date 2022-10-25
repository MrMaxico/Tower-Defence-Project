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
    public float recoilTime;
    float hitTimer;

    public int damage;

    public Vector3 offset;
    Vector3 targetDirection;
    Vector3 direction;

    bool shot;
    bool animationTriggered;

    public Animator anim;

    private void Update()
    {
        damage = GetComponent<TowerValues>().damage[GetComponent<TowerValues>().level];
        //range = GetComponent<TowerValues>().range[GetComponent<TowerValues>().level];

        targetDirection = (target.transform.position + offset) - transform.position;
        direction = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);

        hitTimer += Time.deltaTime;
        if (hitTimer >= firerate && !shot)
        {
            shot = true;
            GameObject shotBullet = Instantiate(bullet, bulletSpawn.position, Quaternion.LookRotation(direction));
            shotBullet.GetComponent<DurianScript>().goal = target.transform;
            shotBullet.GetComponent<DurianScript>().bulletSpeed = bulletSpeed;
            shotBullet.GetComponent<DurianScript>().damage = damage;
            hitTimer = 0;
            shot = false;
            animationTriggered = false;
        }

        if (hitTimer >= firerate - recoilTime && !animationTriggered)
        {
            StartCoroutine(ShootAnimationTrigger());
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
