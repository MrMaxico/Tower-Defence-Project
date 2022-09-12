using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerValues : MonoBehaviour
{
    public int level;
    public int maxLevel;
    public int upgradeCost;
    public int inflation;

    public Material maxLevelMat;

    public float range;

    private void Update()
    {
        if (level == maxLevel)
        {
            GetComponent<MeshRenderer>().material = maxLevelMat;
        }
    }
}
