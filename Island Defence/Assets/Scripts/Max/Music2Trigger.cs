using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music1Trigger : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<AudioManagerScript>().Play("WaveMusic1");
        Destroy(gameObject);
    }

}
