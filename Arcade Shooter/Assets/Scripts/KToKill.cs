using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KToKill : MonoBehaviour {

	private HealthController healthController;
	private BasicEnemyBehaviour enemyBehaviour;

	// Use this for initialization
	void Awake () 
	{
		healthController = gameObject.GetComponent<HealthController> ();
		enemyBehaviour = gameObject.GetComponent<BasicEnemyBehaviour> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.K)) 
		{
			healthController.healthAmount -= 500;

			if (healthController.healthAmount > 0) 
			{
				enemyBehaviour.SpawnActiveExpHit(500, 1);
				enemyBehaviour.SpawnPassiveExpHit(500, 1);
			}
		}
	}
}
