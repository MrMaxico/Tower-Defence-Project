using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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

    public TextMeshProUGUI waveIndicator;

    public Slider mamaHpBar;

    public bool firstWave;
    public bool firstScene;

    private void Start()
    {
        if (!firstScene)
        {
            StartCoroutine(SpawnCycle());
        }
        firstWave = true;
        for (int i = 0; i < path.Length - 1; i++)
        {
            Debug.DrawLine(path[i].position, path[i + 1].position, Color.white, 6000f, false);
        }
        waveIndicator.text = $"Preparing for wave {currentWave + 1}..";
    }

    private void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public IEnumerator SpawnCycle()
    {
        if (waveProgress == 0)
        {
            Debug.Log("SpawnCycle triggered");
            waveIndicator.text = $"Preparing for wave {currentWave + 1}..";
            FindObjectOfType<AudioManagerScript>().Play("NextWaveHorn");
            yield return new WaitForSeconds(waveDelay);
        }

        if (waveProgress < waves[currentWave].GetComponent<Wave>().spawn.Length)
        {
            waveIndicator.text = $"WAVE: {currentWave + 1}";
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
