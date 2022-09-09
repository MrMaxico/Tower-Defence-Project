using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class PathFollowingScript : MonoBehaviour
{
    public Transform[] path;

    GameObject[] totems;
    GameObject closestTotem;

    public float speed;
    float slowness;

    public int pathProgress;
    public int hp;

    bool onReturn;
    public bool hasTotemEffect;

    private void Start()
    {
        for (int i = 0; i < path.Length - 1; i++)
        {
            Debug.DrawLine(path[i].position, path[i + 1].position, Color.red, 6000f);
        }
    }

    private void Update()
    {
        //check if enemy is at his goal
        if (transform.position == path[path.Length - 1].position)
        {
            StartCoroutine(PickupGem());
        }

        //path of the enemy
        if (gameObject.transform.position != path[pathProgress].position)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[pathProgress].position, speed * Time.deltaTime - slowness * Time.deltaTime);
        }
        else if (!onReturn && pathProgress < path.Length - 1)
        {
            pathProgress++;
        }
        else if (onReturn &&pathProgress > 0)
        {
            pathProgress--;
        }

        if (hp <= 0)
        {
            Destroy(gameObject);
        }

        //slowness totem
        totems = GameObject.FindGameObjectsWithTag("Totem");
        hasTotemEffect = false;
        for (int i = 0; i < totems.Length; i++)
        {
            if (Vector3.Distance(transform.position, totems[i].transform.position) < totems[i].GetComponent<Totem>().range)
            {
                if (hasTotemEffect)
                {
                    if (Vector3.Distance(transform.position, totems[i].transform.position) < Vector3.Distance(transform.position, closestTotem.transform.position))
                    {
                        hasTotemEffect = true;
                        closestTotem = totems[i];
                    }
                }
                else
                {
                    hasTotemEffect = true;
                    closestTotem = totems[i];
                }
            }
        }
        if (hasTotemEffect)
        {
            slowness = speed / closestTotem.GetComponent<Totem>().slownessLevel;
        }
        else
        {
            closestTotem = null;
            slowness = 0;
        }
    }

    private IEnumerator PickupGem()
    {
        yield return new WaitForSeconds(1);
        onReturn = true;
    }
}
