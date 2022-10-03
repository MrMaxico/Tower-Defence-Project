using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerValues : MonoBehaviour
{
    public int level;
    public int maxLevel;
    public int[] upgradeCost;
    public int sellFor;
    public int[] damage;

    public GameObject[] meshHolder;

    public Material[] levelMat;

    public float[] range;

    public void UpdateMaterial()
    {
        for (int i = 0; i < meshHolder.Length; i++)
        {
            if (meshHolder[i].TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
            {
                meshRenderer.material = levelMat[level];
            }
            else if (meshHolder[i].TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer skinnedMeshRenderer))
            {
                skinnedMeshRenderer.material = levelMat[level];
            }
        }
    }
}
