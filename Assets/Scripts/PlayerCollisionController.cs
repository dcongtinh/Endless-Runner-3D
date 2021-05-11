﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CMF;
using UnityEngine.Networking;

public class PlayerCollisionController : MonoBehaviour
{
    // endpoint to get current action, from Python
    public static string actionUri = "http://localhost:5000/pose";
    public static string imageUri = "http://localhost:5000/image";

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI scoreTextOver;
    public TextMeshProUGUI highScoreTextOver;

    public SpawnManager spawnManager;
    public ParticleSystem coinCollectParticle;
    public ParticleSystem enemyHitParticle;
    public GameObject gameOverMenu;

    private Animator anim;
    private bool isdead = false;
    private bool middleLaneTriggered = true;
    private float scores = 0;
    private float coins = 0;
    private string SavePath = "Assets/HighScores.json";
    private int numPerRanking = 5;

    // using namespace CMF
    private AdvancedWalkerController awc;
    private AudioControl audioControl;
    private string action = "idle";
    private bool httpError = true; // status of server is working or not

    // get pose task handler
    private IEnumerator getPoseRequest;
    private IEnumerator getImageRequest;


    public Image displayImage;

    [Serializable]
    public class PoseResponse
    {
        public string code;
        public string action;
    }


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        awc = GetComponent<AdvancedWalkerController>();
        audioControl = GetComponent<AudioControl>();
        getPoseRequest = GetPose();
        getImageRequest = GetImage();
        StartCoroutine(getPoseRequest);
        StartCoroutine(getImageRequest);
    }

    void FixedUpdate()
    {
        scoreText.text = scores.ToString() + " m";
        coinsText.text = coins.ToString() + " x";
    }
    string currentLane = "m";
    string nextLane = "m";
    bool keyLRPressed = false;

    // Update is called once per frame
    private void SwitchLane()
    {
        if (action == "rRight" || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentLane != "r")
            {
                awc.ChangeAction("rRight");
                nextLane = "r";
                keyLRPressed = true;
            }
        }

        if (action == "rLeft" || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentLane != "l")
            {
                awc.ChangeAction("rLeft");
                nextLane = "l";
                keyLRPressed = true;
            }
        }

        if (keyLRPressed)
        {
            if (nextLane == "r")
            {
                if (currentLane == "l" && middleLaneTriggered)
                {
                    awc.ChangeAction("idle");
                    currentLane = "m";
                    keyLRPressed = false;
                }
                else if (currentLane == "m" && transform.position.x >= 3)
                {
                    awc.ChangeAction("idle");
                    currentLane = "r";
                    keyLRPressed = false;
                    middleLaneTriggered = false;
                }
            }
            else if (nextLane == "l")
            {
                if (currentLane == "r" && middleLaneTriggered)
                {
                    awc.ChangeAction("idle");
                    currentLane = "m";
                    keyLRPressed = false;
                }
                else if (currentLane == "m" && transform.position.x <= -3)
                {
                    awc.ChangeAction("idle");
                    currentLane = "l";
                    keyLRPressed = false;
                    middleLaneTriggered = false;
                }
            }
        }
    }

    void Update()
    {
        awc.Update_STT_Server(httpError);

        if (httpError == false)
        { // server is working !!!
            displayImage.enabled = true;
            if (!isdead)
            {
                SwitchLane();
                if (!keyLRPressed)
                {
                    if (action == "walking")
                    {
                        awc.ChangeAction("walk");
                        scores += 1;
                    }
                    else if (action == "running")
                    {
                        awc.ChangeAction("run");
                        scores += 2;
                    }
                    else if (action == "sliding")
                    {
                        awc.ChangeAction("slide");
                        scores += 2;
                    }
                    else if (action == "idle" || action == "") awc.ChangeAction("idle");
                }
            }
            else awc.ChangeAction("idle");
        }
        else
        { // using keyboard controller
            if (!isdead)
            {
                scores += 1;
                if (Input.GetKeyDown(KeyCode.Space)) awc.ChangeAction("jump");
                else if (Input.GetKeyDown(KeyCode.DownArrow)) awc.ChangeAction("slide");
                else
                {
                    SwitchLane();
                    if (!keyLRPressed)
                    {
                        awc.ChangeAction("run");
                    }
                }
            }
            else awc.ChangeAction("idle");
        }
    }

    IEnumerator GetPose() {
        while (true) {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(actionUri)) {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                if (webRequest.isHttpError) {
                    Debug.Log("HTTP ERROR: " + webRequest.error);
                    this.httpError = true;
                    continue;
                }
                try {
                    // parse to get action from http response
                    var text = webRequest.downloadHandler.text;
                    var response = JsonUtility.FromJson<PoseResponse>(text);
                    this.action = response.action;
                    this.httpError = false;
                    Debug.Log("New action updated: " + action);
                }
                // if error
                catch (Exception e) {
                    this.httpError = true;
                    Debug.LogError("Error happened: " + e.ToString());
                }
                // whatever happens, wait for 0.25 second
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    IEnumerator GetImage()
    {
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                if (webRequest.isHttpError)
                {
                    // Debug.Log("HTTP ERROR: " + webRequest.error);
                    continue;
                }

                try
                {
                    // parse to get action from http response
                    var base64Url = webRequest.downloadHandler.text;
                    byte[] imageBytes = Convert.FromBase64String(base64Url);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageBytes);
                    Rect rect = new Rect(0.0f, 0.0f, tex.width, tex.height);
                    Vector2 pivot = new Vector2(0.5f, 0.5f);
                    float pixelsPerUnit = 128f;
                    Sprite sprite = Sprite.Create(tex, rect, pivot, pixelsPerUnit);
                    displayImage.overrideSprite = sprite;
                }
                // if error
                catch (Exception e)
                {
                    // Debug.LogError("Error happened: " + e.ToString());
                }

                // whatever happens, wait for 0.05 second
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SpawnTrigger")
        {
            spawnManager.SpawnTriggerEntered();
        }

        if (other.tag == "coin")
        {
            Instantiate(coinCollectParticle, other.transform.position + new Vector3(0, 0.49f, 0), other.transform.rotation);
            Destroy(other.gameObject);
            audioControl.PlayCoinSound();
            ++coins;
            scores += 10;
        }

        if (other.tag == "MiddleLane"){
            middleLaneTriggered = true;
        }
    }

    private EntryTable GetSavedHighScores()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("HighScores.json does not exist.");
            return new EntryTable();
        }
        using (StreamReader stream = new StreamReader(SavePath))
        {
            string json = stream.ReadToEnd();
            return JsonUtility.FromJson<EntryTable>(json);
        }
    }

    private void SaveScores(EntryTable entryTable)
    {
        using (StreamWriter stream = new StreamWriter(SavePath))
        {
            string json = JsonUtility.ToJson(entryTable, true);
            stream.Write(json);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "hurdle" && !isdead)
        {
            audioControl.PlayDeathSound();
            isdead = true;
            EntryTable entries = GetSavedHighScores();
            entries.highScores.Add((int)scores);
            entries.highScores.Sort((a, b) => b.CompareTo(a));
            if (entries.highScores.Count > numPerRanking)
            {
                entries.highScores.RemoveRange(numPerRanking, entries.highScores.Count - numPerRanking);
            }

            SaveScores(entries);
            scoreTextOver.text = scores.ToString();
            highScoreTextOver.text = Mathf.Max(scores, entries.highScores[0]).ToString();
            anim.SetTrigger("death");
        }
    }

    public void RestartGame()
    {
        // Application.LoadLevel(Application.loadedLevelName);
        gameOverMenu.SetActive(true);
    }
}
