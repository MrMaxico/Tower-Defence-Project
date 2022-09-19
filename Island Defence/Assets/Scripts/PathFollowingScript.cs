using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class PathFollowingScript : MonoBehaviour
{
    public Transform[] path;

    public GameObject player;
    public GameObject chest;
    public GameObject gem;
    public GameObject redGem;
    public GameObject coin;
    GameObject[] totems;
    GameObject closestTotem;

    public float speed;
    public float rotationSpeed;
    float slowness;

    public int pathProgress;
    public int hp;
    public int dropAmount;

    bool onReturn;
    public bool hasTotemEffect;

    Vector3 direction;

    public Animator animations;

    private void Start()
    {
        gem.SetActive(false);
        redGem.SetActive(false);
        coin.SetActive(false);
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
            transform.LookAt(path[pathProgress]);
        }
        else if (!onReturn && pathProgress < path.Length - 1)
        {
            pathProgress++;
        }
        else if (onReturn && pathProgress > 0)
        {
            pathProgress--;
        }
        else if (onReturn && gameObject.transform.position == path[0].position)
        {
            Destroy(gameObject);
        }

        if (hp <= 0)
        {
            animations.SetBool("Dead", true);
            speed = 0;
            StartCoroutine(DespawnOnDeath());
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
        if (chest.GetComponent<Chest>().gemsLeft > 1)
        {
            gem.SetActive(true);
            chest.GetComponent<Chest>().gemsLeft--;
        }
        else if (chest.GetComponent<Chest>().gemsLeft == 1)
        {
            redGem.SetActive(true);
            chest.GetComponent<Chest>().gemsLeft--;
        }
        else
        {
            coin.SetActive(true);
            player.GetComponent<PlayerScript>().money--;
        }
        onReturn = true;
    }

    IEnumerator DespawnOnDeath()
    {
        yield return new WaitForSeconds(1.5f);
        player.GetComponent<PlayerScript>().money += dropAmount;
        if (redGem.activeSelf || gem.activeSelf)
        {
            chest.GetComponent<Chest>().gemsLeft++;
        }
        else if (coin.activeSelf)
        {
            player.GetComponent<PlayerScript>().money++;
        }
        Destroy(gameObject);
    }
}
