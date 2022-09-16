using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public GameObject chest;
    public GameObject player;
    public GameObject[] waves;

    public Transform[] path;

    public float spawnDelay;
    public float waveDelay;

    public int currentWave;
    public int waveProgress;

    public Text waveIndicator;

    private void Start()
    {
        StartCoroutine(SpawnCycle());
        for (int i = 0; i < path.Length - 1; i++)
        {
            Debug.DrawLine(path[i].position, path[i + 1].position, Color.red, 6000f);
        }
        waveIndicator.text = $"Preparing for wave {currentWave + 1}..";
    }

    private IEnumerator SpawnCycle()
    {
        if (waveProgress == 0)
        {
            yield return new WaitForSeconds(waveDelay / 2);
            waveIndicator.text = $"Preparing for wave {currentWave + 1}..";
            yield return new WaitForSeconds(waveDelay /2);
        }

        if (waveProgress < waves[currentWave].GetComponent<Wave>().spawn.Length)
        {
            waveIndicator.text = $"Current wave: {currentWave + 1}";
            GameObject spawned = Instantiate(waves[currentWave].GetComponent<Wave>().spawn[waveProgress], transform.position, Quaternion.identity);
            spawned.GetComponent<PathFollowingScript>().chest = chest;
            spawned.GetComponent<PathFollowingScript>().path = path;
            spawned.GetComponent<PathFollowingScript>().player = player;
            waveProgress++;
            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(SpawnCycle());
        }
        else if (currentWave < waves.Length - 1)
        {
            waveProgress = 0;
            currentWave++;
            StartCoroutine(SpawnCycle());
        }
        else
        {
            Debug.Log("All waves completed");
            waveIndicator.text = $"All waves completed";
        }
    }
}
