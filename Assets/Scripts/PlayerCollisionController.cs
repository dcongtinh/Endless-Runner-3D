using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCollisionController : MonoBehaviour
{
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
    private float scores = 0;
    private float coins = 0;
    private string SavePath = "Assets/HighScores.json";
    private int numPerRanking = 5;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        scoreText.text = scores.ToString() + " m";
        coinsText.text = coins.ToString() + " x";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("jump");
        }
        if (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f)
        {
            if (!isdead) {
                scores += 1;
                anim.SetTrigger("run");
            }
        }
        else anim.SetTrigger("idle");
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
            ++coins;
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
            isdead = true;
            EntryTable entries = GetSavedHighScores();
            entries.highScores.Add((int)scores);
            entries.highScores.Sort((a, b) => b.CompareTo(a));
            if (entries.highScores.Count > numPerRanking){
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
