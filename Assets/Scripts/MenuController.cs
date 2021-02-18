using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        // SceneManager.LoadScene("Level01");
        Application.LoadLevel("Level01");
    }

    public void Ranking()
    {
        // SceneManager.LoadScene("Ranking");
        Application.LoadLevel("Ranking");
    }

    public void MainMenu()
    {
        // SceneManager.LoadScene("Level01");
        Application.LoadLevel("MainMenu");
    }
}
