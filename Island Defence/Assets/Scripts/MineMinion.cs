using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineMinion : MonoBehaviour
{
    public GameObject player;
    public GameObject coin;

    public Transform[] mineToChestRoute;

    public float speed;
    public float timeToMine;

    public bool onReturn;

    public int pathProgress;

    private void Start()
    {
        onReturn = true;
    }

    private void Update()
    {
        //check if enemy is at his goal
        if (transform.position == mineToChestRoute[0].position && onReturn)
        {
            StartCoroutine(Mining());
        }

        //path of the enemy
        if (gameObject.transform.position != mineToChestRoute[pathProgress].position)
        {
            transform.position = Vector3.MoveTowards(transform.position, mineToChestRoute[pathProgress].position, speed * Time.deltaTime);
        }
        else if (!onReturn && pathProgress < mineToChestRoute.Length - 1)
        {
            pathProgress++;
        }
        else if (onReturn && pathProgress > 0)
        {
            pathProgress--;
        }
        else if (!onReturn && gameObject.transform.position == mineToChestRoute[mineToChestRoute.Length - 1].position)
        {
            player.GetComponent<PlayerScript>().money++;
            onReturn = true;
        }

        if (onReturn)
        {
            coin.SetActive(false);
        }
        else
        {
            coin.SetActive(true);
        }
    }

    IEnumerator Mining()
    {
        yield return new WaitForSeconds(timeToMine);
        onReturn = false;
        coin.SetActive(true);
        pathProgress = 1;
    }
}
