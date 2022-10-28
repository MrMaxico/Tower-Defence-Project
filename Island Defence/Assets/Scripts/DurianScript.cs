using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurianScript : MonoBehaviour
{
    public float bulletSpeed;

    public int damage;

    public Transform goal;

    public GameObject lastHit;

    RaycastHit hit;

    private void Update()
    {
        if (goal != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, goal.position, bulletSpeed * Time.deltaTime);

            if (transform.position == goal.position)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }

        if (Physics.Raycast(transform.position, transform.forward, out hit, .3f))
        {
            if (hit.transform.gameObject.CompareTag("Enemy") && hit.transform.gameObject != lastHit)
            {
                lastHit = hit.transform.gameObject;
                if (hit.transform.gameObject.GetComponent<PathFollowingScript>().shield)
                {
                    hit.transform.gameObject.GetComponent<PathFollowingScript>().shield = false;
                    return;
                }
                else
                {
                    hit.transform.gameObject.GetComponent<PathFollowingScript>().hp -= damage;
                }
            }
        }
    }
}
