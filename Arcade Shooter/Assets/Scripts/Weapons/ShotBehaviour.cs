using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBehaviour : MonoBehaviour {

	//Shot Properties
	public float globalShotSpeed;
	public float globalShotDamage;
	public float shotDamage;
	public int playerNumber;

	// Shot Properties - Hidden Statics
	[HideInInspector]public float basicShotSpeed;
	[HideInInspector]public Color playerColor;
	public Vector3 playerCurrentSpeed;

	private Rigidbody shotRB;



	// Use this for initialization
	void Start () 
	{
		shotRB = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Check against all shot tags and start relevant function
		if(gameObject.CompareTag("BasicShot"))
		{
			BasicShot ();
		}
	}

	void OnDisable()
	{
		GetComponent<Light> ().enabled = false;
	}

	void BasicShot()
	{
		shotDamage = 5f * globalShotDamage;
		basicShotSpeed = 20f;
		shotRB.velocity = (Vector3.forward * basicShotSpeed * globalShotSpeed);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Boundary") || other.gameObject.CompareTag("Player") || other.gameObject.layer == 9 || other.gameObject.layer == 11) 
		{
			return;
		}

		HealthController collisionHealthController = other.gameObject.GetComponent<HealthController> ();
		BasicEnemyBehaviour collisionEnemyBehaviour = other.gameObject.GetComponent<BasicEnemyBehaviour> ();

		if (collisionHealthController == null) 
		{
			Debug.Log ("Shot collided with an object without a Health Controller.");
			return;
		}

		collisionHealthController.healthAmount = collisionHealthController.healthAmount - shotDamage;
		collisionHealthController.ResetEnemyFlashCounter ();
		collisionHealthController.enemyFlashActive = true;

		if (collisionHealthController.healthAmount <= 0) 
		{
			gameObject.SetActive (false);
			return;
		}

		else
		{
			collisionEnemyBehaviour.SpawnActiveExpHit (shotDamage, playerNumber);
			collisionEnemyBehaviour.SpawnPassiveExpHit (shotDamage, playerNumber);
			gameObject.SetActive (false);
		}
	}
}
