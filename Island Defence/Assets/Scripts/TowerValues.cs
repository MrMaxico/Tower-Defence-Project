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
    public int[] splashDamage;

    public GameObject[] meshHolder;
    public GameObject[] stars;
    public GameObject maxParticles;

    public Material[] levelMat;

    public float[] range;
    public float[] fireRate;
    public float starVisTime;

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

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
            stars[level].SetActive(true);
        }

        if (level == maxLevel)
        {
            maxParticles.SetActive(true);
        }
    }

    private void Update()
    {
        //display the right amount of stars
        if (starVisTime > 0)
        {
            starVisTime -= Time.deltaTime;
            stars[level].SetActive(true);
        }
        else
        {
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].SetActive(false);
                stars[level].SetActive(false);
            }
        }
    }
}
