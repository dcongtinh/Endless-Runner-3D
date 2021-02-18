using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    RoadSpawner roadSpawner;
    PlotSpawner plotSpawner;
    ObstacleSpawner obstacleSpawner;
    EnemySpawner enemySpawner;
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
        roadSpawner.MoveRoad();
        plotSpawner.SpawnPlot();
        plotSpawner.SpawnPlot();
        obstacleSpawner.SpawnObstacle();
        enemySpawner.SpawnEnemy();
    }
}
