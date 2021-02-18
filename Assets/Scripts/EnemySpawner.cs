using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] hurdles;
    // public float minHurdleTime = 2;
    // public float maxHurdleTime = 5;
    // private float hurdleTime = 5;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // StartCoroutine(spawnHurdle());
    }

    // IEnumerator spawnHurdle()
    // {
    //     hurdleTime = Random.Range(minHurdleTime, maxHurdleTime);
    //     yield return new WaitForSeconds(hurdleTime);
    //     spawn();
    // }

    public void SpawnEnemy()
    {
        int randHurdle = Random.Range(0, hurdles.Length);

        float[] xpos = new float[3];
        xpos[0] = 0f;
        xpos[1] = -5.75f;
        xpos[2] = 5.75f;
        int randXPos = Random.Range(0, xpos.Length);

        Vector3 hpos = new Vector3(xpos[randXPos], 3, player.position.z + 36);
        Instantiate(hurdles[randHurdle], hpos, hurdles[randHurdle].transform.rotation);
        // StartCoroutine(spawnHurdle());
    }
}
