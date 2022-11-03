using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellGuy : MonoBehaviour
{
    public GameObject closed;
    public GameObject open;
    public AudioSource phase2Sound;
    public GameObject phase2Transform;

    public float speedMultiplier;
    float startHp;

    bool opened;

    private void Start()
    {
        startHp = GetComponent<PathFollowingScript>().hp;
    }

    private void Update()
    {
        if (!opened && GetComponent<PathFollowingScript>().hp <= (startHp * 0.3f))
        {
            closed.SetActive(false);
            open.SetActive(true);
            GetComponent<PathFollowingScript>().speed = GetComponent<PathFollowingScript>().speed * speedMultiplier;
            opened = true;
            GameObject deathPoof = Instantiate(phase2Transform, transform.position, transform.rotation);
            deathPoof.transform.Rotate(new Vector3(-90, 0, 0));
            //phase2Sound.Play();
            //phase2Sound.pitch = Random.Range(0.9f, 1.1f);
        }
    }
}
