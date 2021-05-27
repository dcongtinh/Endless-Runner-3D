using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Enemy
    {
        public GameObject prefab;
        public bool active;
    }

    public Enemy[] enemies;
    public KeyValuePair<GameObject, bool> test;
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
        List<Enemy> activeEnemies = new List<Enemy>();
        foreach (Enemy e in enemies){
            if (e.active == true){
                activeEnemies.Add(e);
            }
        }

        if (activeEnemies.Count > 0) {
            int randHurdle = Random.Range(0, activeEnemies.Count);
            float[] xpos = new float[3];
            xpos[0] = 0f;
            xpos[1] = -2.5f;
            xpos[2] = 2.5f;
            int randXPos = Random.Range(0, xpos.Length);
            float height = activeEnemies[randHurdle].prefab.name.Contains("Dragon") ? 2.5f : 0f;
            Vector3 hpos = new Vector3(xpos[randXPos], height, player.position.z + 72f);
            Instantiate(activeEnemies[randHurdle].prefab, hpos, activeEnemies[randHurdle].prefab.transform.rotation);
            // StartCoroutine(spawnHurdle());
        }
    }
}
