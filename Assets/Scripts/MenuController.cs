﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    AudioSource clickSound;

    // Start is called before the first frame update
    void Start()
    {
        clickSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        // SceneManager.LoadScene("Level01");
        clickSound.Play();
        Application.LoadLevel("Level01");
    }

    public void Ranking()
    {
        // SceneManager.LoadScene("Ranking");
        clickSound.Play();
        Application.LoadLevel("Ranking");
    }

    public void MainMenu()
    {
        // SceneManager.LoadScene("Level01");
        clickSound.Play();
        Application.LoadLevel("MainMenu");
    }
}
