using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MamaScript : MonoBehaviour
{
    public Transform babySpawnPos;

    public GameObject babyPrefab;
    public GameObject mamaHpBarGameObject;

    public int babyAmount;
    int babiesSpawned;

    public float babyDelay;
    float timer;

    private void Start()
    {
        mamaHpBarGameObject.SetActive(true);
        FindObjectOfType<AudioManagerScript>().StopPlaying("DefenceSetupMusic");
        FindObjectOfType<AudioManagerScript>().StopPlaying("WavemMusic1");
        FindObjectOfType<AudioManagerScript>().StopPlaying("WavemMusic2");
        FindObjectOfType<AudioManagerScript>().play("MamaSquidMusic");
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= babyDelay)
        {
            babiesSpawned = 0;
            StartCoroutine(MakeBabies());
            FindObjectOfType<AudioManagerScript>().play("MamaSquidRoar");
            timer = 0;
        }
    }


    IEnumerator MakeBabies()
    {
        GameObject baby = Instantiate(babyPrefab, babySpawnPos.position, Quaternion.identity);
        baby.GetComponent<PathFollowingScript>().chest = GetComponent<PathFollowingScript>().chest;
        baby.GetComponent<PathFollowingScript>().path = GetComponent<PathFollowingScript>().path;
        baby.GetComponent<PathFollowingScript>().player = GetComponent<PathFollowingScript>().player;
        babiesSpawned++;
        if (babiesSpawned < babyAmount)
        {
            yield return new WaitForSeconds(baby.GetComponent<PathFollowingScript>().timeTillNextEnemySpawn);
            StartCoroutine(MakeBabies());
        }
    }
}
