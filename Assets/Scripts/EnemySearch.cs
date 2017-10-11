using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearch : MonoBehaviour {

    public float visionRadius;
    public float speed;

    GameObject player;

    Vector3 initialPosition;


	// Use this for initialization
	void Start () {
        initialPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 target = initialPosition;
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);


        if (distanceToPlayer < visionRadius) {
            target = player.transform.position;
        }

        float fixedSpeed = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, fixedSpeed);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
