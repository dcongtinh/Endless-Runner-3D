using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class EntryTable
{
    public List<Int32> highScores = new List<Int32>();
}

public class RankingTable : MonoBehaviour
{
    // Start is called before the first frame update
    private string SavePath = "Assets/HighScores.json";
    private Transform entryContainer;
    private Transform entryTemplate;

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

    void Start()
    {
        EntryTable entries = GetSavedHighScores();
        List<Int32> highScores = entries.highScores;
        highScores.Sort((a, b) => b.CompareTo(a));

        entryContainer = transform.Find("Table");
        entryTemplate = entryContainer.Find("Entry");

        entryTemplate.gameObject.SetActive(false);
        float templateHeight = 50f;
        for (int i = 0; i < 5; ++i)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector3(160, -75f - templateHeight * i, 0);
            entryTransform.gameObject.SetActive(true);

            int rank = i + 1;
            string rankStr, medalStr;
            switch (rank)
            {
                case 1: rankStr = "1ST"; medalStr = "TrophyGold"; break;
                case 2: rankStr = "2ND"; medalStr = "TrophySilver"; break;
                case 3: rankStr = "3RD"; medalStr = "TrophyBronze"; break;
                default:
                    rankStr = rank + "TH";
                    medalStr = "None";
                    break;
            }
            entryTransform.Find("Pos").GetComponent<TextMeshProUGUI>().text = rankStr;
            entryTransform.Find("Score").GetComponent<TextMeshProUGUI>().text = highScores[i].ToString();
            entryRectTransform.GetComponent<Image>().enabled = (rank%2 == 1);

            if (rank == 1){
                entryTransform.Find("Pos").GetComponent<TextMeshProUGUI>().color = Color.green;
                entryTransform.Find("Score").GetComponent<TextMeshProUGUI>().color = Color.green;
            }

            if (rank <= 3){
                entryRectTransform.Find(medalStr).gameObject.SetActive(true);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
