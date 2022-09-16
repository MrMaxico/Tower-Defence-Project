using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    public int gemsLeft;

    public Text gemCount;

    private void Update()
    {
        gemCount.text = $"Gems left: {gemsLeft}";
    }
}
