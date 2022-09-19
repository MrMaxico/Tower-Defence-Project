using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Transform goal;

    public Vector3 offset;

    public float bulletSpeed;

    public int damage;

    void Update()
    {
        if (goal != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, goal.position + offset, bulletSpeed * Time.deltaTime);

            if (transform.position == goal.position + offset)
            {
                Debug.Log("BananaSplit");
                goal.gameObject.GetComponent<PathFollowingScript>().hp -= damage;
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
