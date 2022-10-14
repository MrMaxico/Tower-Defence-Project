using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    public GameObject chest;
    public GameObject player;
    public GameObject mamaHpBarGameObject;
    public GameObject[] waves;
    GameObject[] enemies;

    public Transform[] path;

    public float spawnDelay;
    public float waveDelay;

    public int currentWave;
    public int waveProgress;

    public Text waveIndicator;

    public Slider mamaHpBar;

    private void Start()
    {
        StartCoroutine(SpawnCycle());
        for (int i = 0; i < path.Length - 1; i++)
        {
            Debug.DrawLine(path[i].position, path[i + 1].position, Color.white, 6000f);
        }
        waveIndicator.text = $"Preparing for wave {currentWave + 1}..";
    }

    private void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    private IEnumerator SpawnCycle()
    {
        if (waveProgress == 0)
        {
            waveIndicator.text = $"Preparing for wave {currentWave + 1}..";
            yield return new WaitForSeconds(waveDelay);
        }

        if (waveProgress < waves[currentWave].GetComponent<Wave>().spawn.Length)
        {
            waveIndicator.text = $"Current wave: {currentWave + 1}";
            GameObject spawned = Instantiate(waves[currentWave].GetComponent<Wave>().spawn[waveProgress], transform.position, Quaternion.identity);
            spawned.GetComponent<PathFollowingScript>().chest = chest;
            spawned.GetComponent<PathFollowingScript>().path = path;
            spawned.GetComponent<PathFollowingScript>().player = player;
            if (spawned.GetComponent<PathFollowingScript>().isMama)
            {
                spawned.GetComponent<MamaScript>().babySpawnPos = transform;
                spawned.GetComponent<PathFollowingScript>().hpBar = mamaHpBar;
                spawned.GetComponent<MamaScript>().mamaHpBarGameObject = mamaHpBarGameObject;
            }
            waveProgress++;
            spawnDelay = spawned.GetComponent<PathFollowingScript>().timeTillNextEnemySpawn;
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(SpawnCycle());
        }
        else if (currentWave < waves.Length - 1 && enemies.Length == 0)
        {
            waveProgress = 0;
            currentWave++;
            StartCoroutine(SpawnCycle());
        }
        else if (enemies.Length != 0)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(SpawnCycle());
        }
        else if (enemies.Length == 0)
        {
            Debug.Log("All waves completed");
            waveIndicator.text = $"All waves completed";
            StartCoroutine(GreatSucces());
        }
    }

    IEnumerator GreatSucces()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Main Menu");
    }
}
