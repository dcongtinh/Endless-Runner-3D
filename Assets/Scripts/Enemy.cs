using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float movementSpeed = 8f;
    private Rigidbody enemyRb;
    private Transform player;
    private float reactDistance = 50f;

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        Vector3 lookDirection;
        Vector3 targetPos = new Vector3(player.position.x, player.position.y, player.position.z);
        if (distance <= reactDistance)
        {
            if (distance > 5f)
            {
                targetPos.z += (distance / 2f);
            }
            lookDirection = (targetPos - transform.position).normalized;
            enemyRb.AddForce(lookDirection * movementSpeed);
        }
        else
        {
            lookDirection = (targetPos - transform.position).normalized;
            enemyRb.AddForce(lookDirection * movementSpeed * 0.2f);
        }

        if ((transform.position.z - player.position.z) < -3f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            transform.GetChild(0).transform.gameObject.SetActive(true);
        }
    }
}
