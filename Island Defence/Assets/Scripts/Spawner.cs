using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Spawner : MonoBehaviour
{
    [Header("Input values")]
    public GameObject[] waves;
    public Transform[] path;
    public float spawnDelay;
    public float waveDelay;
    public int currentWave;
    public int waveProgress;
    public bool firstWave;
    public bool firstScene;
    [Space(20)]
    [Header("Game info")]
    public GameObject chest;
    public GameObject player;
    GameObject[] enemies;
    [Space(20)]
    [Header("UX")]
    public GameObject mamaHpBarGameObject;
    public GameObject[] winConfetti;
    public TextMeshProUGUI waveIndicator;
    public Slider mamaHpBar;
    public Animator winScreen;

    private void Start()
    {
        //auto-start wave in second level
        if (!firstScene)
        {
            firstWave = true;
            StartCoroutine(SpawnCycle());
        }
        //draw a debug line to display the path
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
        if (firstWave && !firstScene)
        {
            yield return new WaitForSeconds(45 - waveDelay);
            firstWave = false;
        }
        if (waveProgress == 0)
        {
            Debug.Log("SpawnCycle triggered");
            waveIndicator.text = $"Preparing for wave {currentWave + 1}..";
            FindObjectOfType<AudioManagerScript>().Play("NextWaveHorn");
            FindObjectOfType<AudioManagerScript>().Play("WaveMusic1");
            FindObjectOfType<AudioManagerScript>().StopPlaying("DefenceSetupMusic");
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
            foreach (GameObject _winConfetti in winConfetti)
            {
                _winConfetti.SetActive(true);
            }
            winScreen.SetBool("IsGameOver", true);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
