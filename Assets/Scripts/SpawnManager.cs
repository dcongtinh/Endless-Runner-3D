using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    RoadSpawner roadSpawner;
    PlotSpawner plotSpawner;
    ObstacleSpawner obstacleSpawner;
    EnemySpawner enemySpawner;
    public int enemyTime = 2;
    private int cntTriggered = 0;

    // Start is called before the first frame update
    void Start()
    {
        roadSpawner = GetComponent<RoadSpawner>();
        plotSpawner = GetComponent<PlotSpawner>();
        obstacleSpawner = GetComponent<ObstacleSpawner>();
        enemySpawner = GetComponent<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnTriggerEntered()
    {
        ++cntTriggered;
        roadSpawner.MoveRoad();
        plotSpawner.SpawnPlot();
        plotSpawner.SpawnPlot();
        obstacleSpawner.SpawnObstacle();

        if (cntTriggered % enemyTime == 0)
            enemySpawner.SpawnEnemy();

    }
}
