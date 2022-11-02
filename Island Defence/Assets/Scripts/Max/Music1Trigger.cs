using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music2Trigger : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<AudioManagerScript>().Play("WaveMusic2");
        Destroy(gameObject);
    }

}
