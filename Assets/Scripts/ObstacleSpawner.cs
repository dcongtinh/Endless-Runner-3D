using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    private int initAmount = 5;
    private float plotSize = 60f;
    private float lastZPos = 15f;
    public List<GameObject> obstacles;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < initAmount; ++i)
        {
            SpawnObstacle();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnObstacle()
    {
        GameObject plotLeft = obstacles[Random.Range(0, obstacles.Count)];
        GameObject plotRight = obstacles[Random.Range(0, obstacles.Count)];

        float zPos = lastZPos + plotSize;
        Instantiate(plotLeft, new Vector3(0, -0.25f, zPos), plotLeft.transform.rotation);

        lastZPos += plotSize;
    }
}
