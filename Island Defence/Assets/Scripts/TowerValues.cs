using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerValues : MonoBehaviour
{
    public int level;
    public int maxLevel;
    public int upgradeCost;
    public int inflation;

    public GameObject meshHolder;

    public Material maxLevelMat;

    public float range;

    private void Update()
    {
        if (level == maxLevel)
        {
            if (meshHolder.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
            {
                meshRenderer.material = maxLevelMat;
            }
            else if (meshHolder.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer skinnedMeshRenderer))
            {
                skinnedMeshRenderer.material = maxLevelMat;
            }
        }
    }
}
