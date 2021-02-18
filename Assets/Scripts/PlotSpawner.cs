using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotSpawner : MonoBehaviour
{
    private int initAmount = 10;
    private float plotSize = 30f;
    private float xPosLeft = -38.25f;
    private float xPosRight = 38.25f;
    private float lastZPos = -30f;
    public List<GameObject> plots;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < initAmount; ++i)
        {
            SpawnPlot();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnPlot()
    {
        GameObject plotLeft = plots[Random.Range(0, plots.Count)];
        GameObject plotRight = plots[Random.Range(0, plots.Count)];

        float zPos = lastZPos + plotSize;
        Instantiate(plotLeft, new Vector3(xPosLeft, 0.0001f, zPos), plotLeft.transform.rotation);
        Instantiate(plotRight, new Vector3(xPosRight, 0.0001f, zPos), new Quaternion(0, 180, 0, 0));

        lastZPos += plotSize;
    }
}
