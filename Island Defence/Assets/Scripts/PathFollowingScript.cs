using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class PathFollowingScript : MonoBehaviour
{
    public Transform[] path;

    public float speed;

    public int pathProgress;
    public int hp;

    bool onReturn;

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
            transform.position = Vector3.MoveTowards(transform.position, path[pathProgress].position, speed * Time.deltaTime);
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
    }

    private IEnumerator PickupGem()
    {
        yield return new WaitForSeconds(1);
        onReturn = true;
    }
}
