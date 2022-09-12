using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject chest;
    public GameObject[] waves;

    public Transform[] path;

    public float spawnDelay;
    public float waveDelay;

    public int currentWave;
    public int waveProgress;

    private void Start()
    {
        StartCoroutine(SpawnCycle());
        for (int i = 0; i < path.Length - 1; i++)
        {
            Debug.DrawLine(path[i].position, path[i + 1].position, Color.red, 6000f);
        }
    }

    private IEnumerator SpawnCycle()
    {
        if (waveProgress == 0)
        {
            yield return new WaitForSeconds(waveDelay);
        }

        if (waveProgress < waves[currentWave].GetComponent<Wave>().spawn.Length)
        {
            GameObject spawned = Instantiate(waves[currentWave].GetComponent<Wave>().spawn[waveProgress], transform.position, Quaternion.identity);
            spawned.GetComponent<PathFollowingScript>().chest = chest;
            spawned.GetComponent<PathFollowingScript>().path = path;
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
        }
    }
}
