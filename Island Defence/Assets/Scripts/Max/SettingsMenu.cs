using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public GameObject menuUI;
    public GameObject levelSelectUI;

    public AudioSource buttonClick;

    public AudioMixer audioMixer;

    //BUTTONS//
    public void GoToLevelSelect()
    {
        buttonClick.Play();
        menuUI.SetActive(false);
        levelSelectUI.SetActive(true);
    }

    public void ReturnToMenu()
    {
        buttonClick.Play();
        menuUI.SetActive(true);
        levelSelectUI.SetActive(false);
    }

    public void LoadLevel1()
    {
        buttonClick.Play();
        SceneManager.LoadScene("Max");
    }

    public void LoadLevel2()
    {
        buttonClick.Play();
        SceneManager.LoadScene("Island");
    }

    public void LoadLevel3()
    {
        buttonClick.Play();
        SceneManager.LoadScene("Catacombs");
    }

    //SETTINGS//

    public void SetVolume(float volume)     //master volume
    {
        buttonClick.Play();
        audioMixer.SetFloat("MainVolume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        buttonClick.Play();
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}