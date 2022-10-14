using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public float timeTillNextEnemySpawn;
    float slowness;
    float fullHealth;

    public int pathProgress;
    public int hp;
    public int dropAmount;

    bool onReturn;
    bool hasDied;
    public bool isMama;
    public bool hasTotemEffect;

    Vector3 direction;
    Vector3 targetDirection;

    public Animator animations;

    public Slider hpBar;

    private void Start()
    {
        gem.SetActive(false);
        gem.GetComponent<PickUps>().parent = gameObject;
        gem.GetComponent<PickUps>().chest = chest;
        redGem.SetActive(false);
        redGem.GetComponent<PickUps>().parent = gameObject;
        redGem.GetComponent<PickUps>().chest = chest;
        coin.SetActive(false);
        coin.GetComponent<PickUps>().parent = gameObject;
        coin.GetComponent<PickUps>().chest = chest;
        fullHealth = hp;
    }

    private void Update()
    {
        //display hp
        float f_hp = hp;
        hpBar.value = f_hp / fullHealth;
        Debug.Log(f_hp / fullHealth);

        //check if enemy is at his goal
        if (transform.position == path[path.Length - 1].position && !onReturn)
        {
            PickupGem();
        }

        //path of the enemy
        if (gameObject.transform.position != path[pathProgress].position)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[pathProgress].position, speed * Time.deltaTime - slowness * Time.deltaTime);
            targetDirection = path[pathProgress].position - transform.position;
            direction = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direction);
            //transform.LookAt(path[pathProgress]);
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

        if (hp <= 0 && !hasDied)
        {
            hasDied = true;
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
        if (gem.activeSelf)
        {
            GameObject droppedGem = Instantiate(gem, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            droppedGem.GetComponent<PickUps>().dropped = true;
            droppedGem.GetComponent<PickUps>().chest = chest;
        }
        else if (redGem.activeSelf)
        {
            GameObject droppedGem = Instantiate(redGem, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            droppedGem.GetComponent<PickUps>().dropped = true;
            droppedGem.GetComponent<PickUps>().chest = chest;
        }
        else if (coin.activeSelf)
        {
            GameObject droppedCoin = Instantiate(coin, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            droppedCoin.GetComponent<PickUps>().dropped = true;
        }
        Debug.Log(gameObject);
        Destroy(gameObject);
    }
}
