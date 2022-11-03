using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Richard : MonoBehaviour
{
    public GameObject[] slides;
    public GameObject next;
    public GameObject prev;
    public GameObject quit;
    public GameObject player;

    public bool talking;

    int currentSlide;

    private void Update()
    {
        //display only the current slide
        if (talking)
        {
            for (int i = 0; i < slides.Length; i++)
            {
                if (i == currentSlide)
                {
                    slides[i].SetActive(true);
                }
                else
                {
                    slides[i].SetActive(false);
                }
            }
        }

        if (!talking)
        {
            for (int i = 0; i < slides.Length; i++)
            {
                slides[i].SetActive(false);
            }
            next.SetActive(false);
            prev.SetActive(false);
            quit.SetActive(false);
        }
        else if (currentSlide >= slides.Length - 1)
        {
            next.SetActive(false);
            prev.SetActive(true);
            quit.SetActive(true);
        }
        else if (currentSlide < 1)
        {
            next.SetActive(true);
            prev.SetActive(false);
            quit.SetActive(false);
        }
        else
        {
            next.SetActive(true);
            prev.SetActive(true);
            quit.SetActive(false);
        }
    }

    public void NextSlide(int skip)
    {
        currentSlide += skip;
    }

    public void EndInteraction()
    {
        currentSlide = 0;
        Time.timeScale = 1;
        for (int i = 0; i < slides.Length; i++)
        {
            slides[i].SetActive(false);
        }
        talking = false;
        player.GetComponent<PlayerScript>().talking = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
