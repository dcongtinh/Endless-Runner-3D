using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float movementSpeed = 10f;
    private Rigidbody enemyRb;
    private Transform player;
    private float reactDistance = 64f;

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float speed = 16.0f;
        Vector3 targetDirection = player.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = speed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

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
            // Enable particle on the monster itself
            transform.GetChild(0).transform.gameObject.SetActive(true);
        }
    }
}
