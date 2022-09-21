using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellGuy : MonoBehaviour
{
    public GameObject closed;
    public GameObject open;

    public float speedMultiplier;
    float startHp;

    bool opened;

    private void Start()
    {
        startHp = GetComponent<PathFollowingScript>().hp;
    }

    private void Update()
    {
        if (!opened && GetComponent<PathFollowingScript>().hp == (startHp * 0.15f))
        {
            closed.SetActive(false);
            open.SetActive(true);
            GetComponent<PathFollowingScript>().speed = GetComponent<PathFollowingScript>().speed * speedMultiplier;
            opened = true;
        }
    }
}
