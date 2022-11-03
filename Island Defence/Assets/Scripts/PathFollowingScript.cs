using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PathFollowingScript : MonoBehaviour
{
    [Header("Input values")]
    public float speed;
    public float rotationSpeed;
    public float timeTillNextEnemySpawn;
    public float hpBarVisTime;
    float slowness;
    float fullHealth;
    public int hp;
    public int dropAmount;
    public int pathProgress;
    bool onReturn;
    bool hasDied;
    public bool isMama;
    public bool shield;
    public bool hasTotemEffect;
    Vector3 direction;
    Vector3 targetDirection;
    public Transform[] path;
    [Space(20)]
    [Header("Game info")]
    public GameObject player;
    public GameObject chest;
    public GameObject gem;
    public GameObject redGem;
    public GameObject coin;
    public GameObject shieldBlub;
    GameObject[] totems;
    GameObject closestTotem;
    [Space(20)]
    [Header("UX")]
    public AudioSource deathSound;
    public GameObject onDeathFX;
    public GameObject slownessFX;
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
        if (hpBarVisTime > 0)
        {
            hpBarVisTime -= Time.deltaTime;
            hpBar.gameObject.SetActive(true);
            float f_hp = hp;
            hpBar.value = f_hp / fullHealth;
            Debug.Log(f_hp / fullHealth);
        }
        else
        {
            hpBar.gameObject.SetActive(false);
        }

        //shield
        shieldBlub.SetActive(shield);

        //check if enemy is at his goal
        if (transform.position == path[path.Length - 1].position && !onReturn)
        {
            if (!isMama)
            {
                PickupGem();
            }
            else
            {
                StartCoroutine(MamaDoom());
            }
        }

        //path of the enemy
        if (gameObject.transform.position != path[pathProgress].position)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[pathProgress].position, speed * Time.deltaTime - slowness * Time.deltaTime);
            targetDirection = path[pathProgress].position - transform.position;
            direction = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(direction);
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

        //die
        if (hp <= 0 && !hasDied)
        {
            hasDied = true;
            animations.SetBool("Dead", true);
            deathSound.Play();
            deathSound.pitch = Random.Range(0.8f, 1.2f);
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
            slownessFX.SetActive(true);
        }
        else
        {
            closestTotem = null;
            slowness = 0;
            slownessFX.SetActive(false);
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

    IEnumerator MamaDoom()
    {
        onReturn = true;
        speed = 0;
        rotationSpeed = 0;
        chest.GetComponent<Chest>().gemsLeft--;
        yield return new WaitForSeconds(1);
        StartCoroutine(MamaDoom());
    }

    IEnumerator DespawnOnDeath()
    {
        if (isMama)
        {
            yield return new WaitForSeconds(5f);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }

        player.GetComponent<PlayerScript>().money += dropAmount;
        if (gem.activeSelf)
        {
            Drop(gem);
        }
        else if (redGem.activeSelf)
        {
            Drop(redGem);
        }
        else if (coin.activeSelf)
        {
            Drop(coin);
        }
        Debug.Log(gameObject);
        Destroy(gameObject);
        GameObject deathPoof = Instantiate(onDeathFX, transform.position, Quaternion.identity);
        deathPoof.transform.Rotate(new Vector3(-90, 0, 0));
    }

    void Drop(GameObject drop)
    {
        GameObject droppedGem = Instantiate(drop, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        droppedGem.GetComponent<PickUps>().dropped = true;
        droppedGem.GetComponent<PickUps>().chest = chest;
    }
}
