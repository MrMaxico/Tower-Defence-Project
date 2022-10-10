using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PickUps : MonoBehaviour
{
    public bool dropped;
    public bool redGem;

    public GameObject parent;
    public GameObject chest;
    public GameObject gem;
    public GameObject gemDespawnTimer;

    public Image gemDespawnFill;

    public float despawnRate;
    float despawnTimer;

    private void Update()
    {
        if (!dropped && chest.GetComponent<Chest>().gemsLeft > 0 && redGem)
        {
            gameObject.SetActive(false);
            parent.GetComponent<PathFollowingScript>().gem.SetActive(true);
        }

        if (dropped && chest.GetComponent<Chest>().gemsLeft > 0 && redGem)
        {
            GameObject spawnedGem = Instantiate(gem, transform.position, Quaternion.identity);
            spawnedGem.GetComponent<PickUps>().chest = chest;
            spawnedGem.GetComponent<PickUps>().dropped = dropped;
            Destroy(gameObject);
        }

        if (dropped)
        {
            //despawning
            despawnTimer += Time.deltaTime;

            gemDespawnTimer.SetActive(true);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            gemDespawnTimer.transform.LookAt(player.transform.position);
            gemDespawnFill.fillAmount -= Time.deltaTime / 30;

            if (despawnTimer >= despawnRate)
            {
                if (redGem)
                {
                    Destroy(gameObject);
                    SceneManager.LoadScene("Main Menu");
                    return;
                }
                Destroy(gameObject);
            }
        }
        else
        {
            gemDespawnTimer.SetActive(false);
        }
    }
}
