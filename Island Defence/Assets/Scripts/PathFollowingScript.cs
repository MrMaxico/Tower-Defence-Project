using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class PathFollowingScript : MonoBehaviour
{
    public Transform[] path;

    public GameObject chest;
    public GameObject gem;
    public GameObject redGem;
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
        gem.SetActive(false);
        redGem.SetActive(false);
    }

    private void Update()
    {
        //check if enemy is at his goal
        if (transform.position == path[path.Length - 1].position && !onReturn)
        {
            PickupGem();
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

    private void PickupGem()
    {
        if (chest.GetComponent<Chest>().GemsLeft > 1)
        {
            gem.SetActive(true);
            chest.GetComponent<Chest>().GemsLeft--;
        }
        else if (chest.GetComponent<Chest>().GemsLeft == 1)
        {
            redGem.SetActive(true);
            chest.GetComponent<Chest>().GemsLeft--;
        }
        onReturn = true;
    }
}
