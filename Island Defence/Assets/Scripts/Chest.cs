using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Chest : MonoBehaviour
{
    public int gemsLeft;

    public TextMeshProUGUI gemCount;
    public void Start()
    {

    }

    private void Update()
    {
        gemCount.text = $":{gemsLeft}";
    }
}
