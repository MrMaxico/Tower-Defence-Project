using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blobvis : MonoBehaviour
{
    public float shieldDelay;
    public float animationRecoil;
    public float range;

    public AudioSource spellSound;
    public GameObject shieldSpellFX;
    GameObject[] enemies;

    public Animator anim;

    void Start()
    {
        StartCoroutine(SpawnShieldCycle());
        GetComponent<PathFollowingScript>().animations.SetTrigger("Spell");
    }

    IEnumerator SpawnShieldCycle()
    {
        yield return new WaitForSeconds(shieldDelay - animationRecoil);
        anim.SetTrigger("Spell");
        yield return new WaitForEndOfFrame();
        anim.ResetTrigger("Spell");
        yield return new WaitForSeconds(animationRecoil);
        GetComponent<PathFollowingScript>().animations.ResetTrigger("Spell");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < range && enemy != gameObject)
            {
                enemy.GetComponent<PathFollowingScript>().shield = true;
            }
        }
        yield return new WaitForSeconds(animationRecoil); spellSound.Play();
        GameObject shieldeffect = Instantiate(shieldSpellFX, transform.position, transform.rotation);
        shieldeffect.transform.Rotate(new Vector3(-90, 0, 0));
        StartCoroutine(SpawnShieldCycle());
    }
}
