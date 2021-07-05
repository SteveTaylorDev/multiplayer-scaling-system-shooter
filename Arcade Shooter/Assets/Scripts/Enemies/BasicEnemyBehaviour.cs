using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyBehaviour : MonoBehaviour 
{
	public GameManager gameManager;
	public float expSpawnForce;							// 4 Seems to work well
	[HideInInspector]public bool spawnExp;
	[HideInInspector]public bool finishedExpSpawn;

	private int passiveExpDropAmount;
	private int activeExpDropAmount;

	private int playerHitByNumber;

	// Multple variables for each of the 4 players
	private float damageTaken1;
	private float damageTaken2;
	private float damageTaken3;
	private float damageTaken4;
	private float currentPlayerDamage;
	private float fiddyDamageTaken1;
	private float fiddyDamageTaken2;
	private float fiddyDamageTaken3;
	private float fiddyDamageTaken4;
	private float currentPlayerFiddyDamage;

	private ExpPickupBehaviour expInstanceBehaviour;
	private HealthController localHealthController;


	void Awake()
	{
		GameObject gameManagerObject = GameObject.FindWithTag ("GameManager");
		gameManager = gameManagerObject.GetComponent<GameManager> ();

		localHealthController = gameObject.GetComponent<HealthController> ();
		finishedExpSpawn = false;
		playerHitByNumber = 0;
		currentPlayerDamage = 0;
		currentPlayerFiddyDamage = 0;
	
	}

	void Update()
	{
		if(spawnExp == true)
		{
			SpawnExp ();
		}
	}

	void OnTriggerEnter (Collider other)
	{
		HealthController colliderHealthController = other.gameObject.GetComponent<HealthController>();
		PlayerMovement colliderMovementScript = other.gameObject.GetComponent<PlayerMovement>();
		Rigidbody colliderRigidbody = other.gameObject.GetComponent<Rigidbody> ();

		if(other.gameObject.CompareTag("Player"))														// Checks if collided object is tagged with Player
		{
			PlayerContact (colliderHealthController, colliderMovementScript, colliderRigidbody);		// Executes the PlayerContact function and sends the collided player's Health Controller and Player Movement data with it
		}
	}
		

	void PlayerContact(HealthController colliderHealthController, PlayerMovement colliderMovementScript, Rigidbody colliderRigidbody)
	{
		if(colliderMovementScript.collidedObjects < 2)					// Checks that the amount of enemy objects the player has collided with in the last frame is below 2 (Stops multiple enemy hit registers)
		{
			float contactDamageAmount = colliderHealthController.startingAmount * 0.25f;										// Calculates 25% of the contacted player's health
			colliderHealthController.healthAmount = colliderHealthController.healthAmount - contactDamageAmount;				// Subtracts that from the current contacted player's health

			float selfDamageMinCheck = 100;								// The default value for calculating self damage from player contact

			if (localHealthController.startingAmount > 100) 			// If this enemy has more than 100 health, it calculates local damage based on its own max health
			{
				selfDamageMinCheck = localHealthController.startingAmount;
			}

			float contactSelfDamage = selfDamageMinCheck * 0.20f;		// Calculates the amount of self damage to inflict (20% of max health if max is above 100HP)
			localHealthController.healthAmount = localHealthController.healthAmount - contactSelfDamage;


			colliderHealthController.flashActive = true;				// Sets flash active in the player's health controller, which starts the hit animation function
			colliderMovementScript.controlEnabled = false;				// Disables the controlEnabled bool in the collided player movement script, stopping the MovePlayer function from running
			colliderRigidbody.velocity = Vector3.zero;					// Sets the collided player's rigidbody velocity to 0

			colliderRigidbody.AddForce (0, 0, -15,ForceMode.Impulse);	// Adds force to the collided player's rigidbody in the -z direction

			localHealthController.enemyFlashActive = true;
		}					

		if (colliderMovementScript.collidedObjects >= 2) 				// Sets the collided enemy object count back to 0 after it hits 2 or higher
		{
			colliderMovementScript.collidedObjects = 0;
			return;
		}
	}


	void SpawnExp()
	{
		if (localHealthController.startingAmount <= (20 * gameManager.activePlayerAmount)) 
		{
			if (gameManager.easyDifficulty == false) 
			{
				passiveExpDropAmount = 1 * gameManager.activePlayerAmount;
				activeExpDropAmount = 2 * gameManager.activePlayerAmount;
			}

			if (gameManager.easyDifficulty == true) 
			{
				passiveExpDropAmount = 1 * gameManager.activePlayerAmount;
				activeExpDropAmount = 2 * gameManager.activePlayerAmount /2;
			}

			DeathSpawnExpAttract (passiveExpDropAmount, activeExpDropAmount);
		} 

		if (localHealthController.startingAmount >= 1000 && localHealthController.startingAmount < 2000) 
		{
			passiveExpDropAmount = 0;
			activeExpDropAmount = 0;

			HighHealthExpSpawn ();
			return;
		} 

		if (localHealthController.startingAmount >= 2000 && localHealthController.startingAmount < 15000) 
		{
			passiveExpDropAmount = 0;
			activeExpDropAmount = 0;

			HighestHealthExpSpawn ();
			return;
		} 

		if (localHealthController.startingAmount >= 15000) 
		{
			passiveExpDropAmount = 0;
			activeExpDropAmount = 0;

			MegaExpSpawn ();
			return;
		} 

		if (localHealthController.startingAmount > (20 * gameManager.activePlayerAmount) && localHealthController.startingAmount < 1000) 
		{
			float activeDropFloat = localHealthController.startingAmount / 20;
			int activeDropInt = Mathf.CeilToInt (activeDropFloat);

			float passiveDropFloat = activeDropFloat * 0.5f;
			int passiveDropInt = Mathf.CeilToInt (passiveDropFloat);

			passiveExpDropAmount = passiveDropInt;
			activeExpDropAmount = activeDropInt;

			DeathSpawnExpAttract (passiveExpDropAmount, activeExpDropAmount);
		}
			
 		for (int i = 0; i < passiveExpDropAmount; i++) 
 		{
			int playerNumber = 0;
			bool attractSpawn = false;
			PassiveExpPoolSpawn (playerNumber, attractSpawn);
 		}
 		
 		for (int i = 0; i < activeExpDropAmount; i++) 
 		{
			int playerNumber = 0;
			bool attractSpawn = false;
			ActiveExpPoolSpawn (playerNumber, attractSpawn);
 		}
 		
		finishedExpSpawn = true;
	}
		

	public void SpawnActiveExpHit(float damageAmount, int playerNumber)
	{
		int expInstAmount;

		if (playerNumber == 1) 
		{
			damageTaken1 += damageAmount;
			currentPlayerDamage = damageTaken1;
		}

		if (playerNumber == 2) 
		{
			damageTaken2 += damageAmount;
			currentPlayerDamage = damageTaken2;
		}

		if (playerNumber == 3) 
		{
			damageTaken3 += damageAmount;
			currentPlayerDamage = damageTaken3;
		}

		if (playerNumber == 4) 
		{
			damageTaken4 += damageAmount;
			currentPlayerDamage = damageTaken4;
		}

		if (currentPlayerDamage >= 10) 
		{
			if (playerNumber == 1) 
			{
				damageTaken1 = 0;
			}

			if (playerNumber == 2) 
			{
				damageTaken2 = 0;
			}

			if (playerNumber == 3) 
			{
				damageTaken3 = 0;
			}

			if (playerNumber == 4) 
			{
				damageTaken4 = 0;
			}

			expInstAmount = Mathf.CeilToInt (currentPlayerDamage) / 10;

			for (int i = 0; i < expInstAmount; i++) 
			{
				bool attractSpawn = true;
				ActiveExpPoolSpawn (playerNumber, attractSpawn);
			}
		}
	}

	public void SpawnPassiveExpHit(float damageAmount, int playerNumber)
	{
		int expInstAmount;

		if (playerNumber == 1) 
		{
			fiddyDamageTaken1 += damageAmount;
			currentPlayerFiddyDamage = fiddyDamageTaken1;
		}

		if (playerNumber == 2) 
		{
			fiddyDamageTaken2 += damageAmount;
			currentPlayerFiddyDamage = fiddyDamageTaken2;
		}

		if (playerNumber == 3) 
		{
			fiddyDamageTaken3 += damageAmount;
			currentPlayerFiddyDamage = fiddyDamageTaken3;
		}

		if (playerNumber == 4) 
		{
			fiddyDamageTaken4 += damageAmount;
			currentPlayerFiddyDamage = fiddyDamageTaken4;
		}

		if (currentPlayerFiddyDamage >= 50) 
		{
			if (playerNumber == 1) 
			{
				fiddyDamageTaken1 = 0;
			}

			if (playerNumber == 2) 
			{
				fiddyDamageTaken2 = 0;
			}

			if (playerNumber == 3) 
			{
				fiddyDamageTaken3 = 0;
			}

			if (playerNumber == 4) 
			{
				fiddyDamageTaken4 = 0;
			}

			expInstAmount = Mathf.CeilToInt (currentPlayerFiddyDamage) /50;

			for (int i = 0; i < expInstAmount; i++) 
			{	
				bool attractSpawn = true;
				PassiveExpPoolSpawn (playerNumber, attractSpawn);
			}
		}
	}

	void DeathSpawnExpAttract(int localPassiveDropAmount, int localActiveDropAmount)
	{
		localActiveDropAmount = localActiveDropAmount / gameManager.activePlayerAmount;
		localPassiveDropAmount = localPassiveDropAmount / gameManager.activePlayerAmount;

		for (int i = 0; i < gameManager.activePlayerAmount; i++) 
		{
			damageTaken1 = 0;
			damageTaken2 = 0;
			damageTaken3 = 0;
			damageTaken4 = 0;

			bool attractSpawn = true;
			for (int ia = 0; ia < localActiveDropAmount; ia++) 
			{
				ActiveExpPoolSpawn (gameManager.players [i].playerNumber, attractSpawn);
			}

			fiddyDamageTaken1 = 0;
			fiddyDamageTaken2 = 0;
			fiddyDamageTaken3 = 0;
			fiddyDamageTaken4 = 0;

			for(int ip = 0; ip < localPassiveDropAmount; ip++)
			{
				PassiveExpPoolSpawn (gameManager.players [i].playerNumber, attractSpawn);
			}
		}
	}

	void HighHealthExpSpawn()
	{
		float activeDropFloat = localHealthController.startingAmount / 400;
		int activeDropInt = Mathf.CeilToInt (activeDropFloat);

		float passiveDropFloat = activeDropFloat * 0.5f;
		int passiveDropInt = Mathf.CeilToInt (passiveDropFloat);

		passiveExpDropAmount = passiveDropInt;
		activeExpDropAmount = activeDropInt;

		for (int i = 0; i < activeExpDropAmount; i++) 
		{
			int playerNumber = 0;
			bool attractSpawn = false;
			LargeActiveExpPoolSpawn (playerNumber, attractSpawn);
		}

		for (int i = 0; i < passiveExpDropAmount; i++) 
		{
			int playerNumber = 0;
			bool attractSpawn = false;
			LargePassiveExpPoolSpawn (playerNumber, attractSpawn);
		}
			
		float floatActiveExpDropAmount = activeDropFloat / gameManager.activePlayerAmount;
		float floatPassiveExpDropAmount = passiveDropFloat / gameManager.activePlayerAmount;

		for (int i = 0; i < gameManager.activePlayerAmount; i++) 
		{
			activeExpDropAmount = Mathf.CeilToInt (floatActiveExpDropAmount);
			passiveExpDropAmount = Mathf.CeilToInt (floatPassiveExpDropAmount);
			LargeActiveExpAttract (activeExpDropAmount, gameManager.players [i].playerNumber);
			LargePassiveExpAttract (passiveExpDropAmount, gameManager.players [i].playerNumber);
		}

		activeDropFloat = localHealthController.startingAmount / 40;
		activeDropInt = Mathf.CeilToInt (activeDropFloat);
		
		passiveDropFloat = activeDropFloat * 0.5f;
		passiveDropInt = Mathf.CeilToInt (passiveDropFloat);
		
		passiveExpDropAmount = passiveDropInt;
		activeExpDropAmount = activeDropInt;
		
		for (int i = 0; i < activeExpDropAmount; i++) 
		{
			int playerNumber = 0;
			bool attractSpawn = false;
			ActiveExpPoolSpawn (playerNumber, attractSpawn);
		}
		
		for (int i = 0; i < passiveExpDropAmount; i++) 
		{
			int playerNumber = 0;
			bool attractSpawn = false;
			PassiveExpPoolSpawn (playerNumber, attractSpawn);
		}
			
		floatActiveExpDropAmount = activeDropFloat / gameManager.activePlayerAmount;
		floatPassiveExpDropAmount = passiveDropFloat / gameManager.activePlayerAmount;
		
		for (int i = 0; i < gameManager.activePlayerAmount; i++) 
		{
			activeExpDropAmount = Mathf.CeilToInt (floatActiveExpDropAmount);
			passiveExpDropAmount = Mathf.CeilToInt (floatPassiveExpDropAmount);

			bool attractSpawn = true;

			for (int ia = 0; ia < activeExpDropAmount; ia++) 
			{
				ActiveExpPoolSpawn (gameManager.players [i].playerNumber, attractSpawn);
			}

			for (int ip = 0; ip < passiveExpDropAmount; ip++) 
			{
				PassiveExpPoolSpawn (gameManager.players [i].playerNumber, attractSpawn);
			}
		}
			
		finishedExpSpawn = true;
	}

	void HighestHealthExpSpawn()
	{
		float activeDropFloat = localHealthController.startingAmount / 200;
		int activeDropInt = Mathf.CeilToInt (activeDropFloat);

		float passiveDropFloat = activeDropFloat * 0.5f;
		int passiveDropInt = Mathf.CeilToInt (passiveDropFloat);

		float floatActiveExpDropAmount = activeDropFloat / gameManager.activePlayerAmount;
		float floatPassiveExpDropAmount = passiveDropFloat / gameManager.activePlayerAmount;

		for (int i = 0; i < gameManager.activePlayerAmount; i++) 
		{
			activeExpDropAmount = Mathf.CeilToInt (floatActiveExpDropAmount);
			passiveExpDropAmount = Mathf.CeilToInt (floatPassiveExpDropAmount);
			LargeActiveExpAttract (activeExpDropAmount, gameManager.players [i].playerNumber);
			LargePassiveExpAttract (passiveExpDropAmount, gameManager.players [i].playerNumber);
		}

		passiveExpDropAmount = passiveDropInt;
		activeExpDropAmount = activeDropInt;

		for (int i = 0; i < activeExpDropAmount; i++) 
		{
			int playerNumber = 0;
			bool attractSpawn = false;
			LargeActiveExpPoolSpawn (playerNumber, attractSpawn);
		}

		for (int i = 0; i < passiveExpDropAmount; i++) 
		{
			int playerNumber = 0;
			bool attractSpawn = false;
			LargePassiveExpPoolSpawn (playerNumber, attractSpawn);
		}

		finishedExpSpawn = true;
	}

	void MegaExpSpawn()
	{
		float activeDropFloat = localHealthController.startingAmount / 2000;
		int activeDropInt = Mathf.CeilToInt (activeDropFloat);

		float floatActiveExpDropAmount = activeDropFloat / gameManager.activePlayerAmount;

		for (int i = 0; i < gameManager.activePlayerAmount; i++) 
		{
			activeExpDropAmount = Mathf.CeilToInt (floatActiveExpDropAmount);
			MegaExpAttract (activeExpDropAmount, gameManager.players [i].playerNumber);
		}

		activeExpDropAmount = activeDropInt;

		for (int i = 0; i < activeExpDropAmount; i++) 
		{
			int playerNumber = 0;
			bool attractSpawn = false;
			MegaExpPoolSpawn (playerNumber, attractSpawn);
		}
			
		finishedExpSpawn = true;
	}

	void LargeActiveExpAttract(int instAmount, int playerNumber)
	{
		for(int i = 0; i < instAmount; i++)
		{
			bool attractSpawn = true;
			LargeActiveExpPoolSpawn (playerNumber, attractSpawn);
		}
	}

	void LargePassiveExpAttract(int instAmount, int playerNumber)
	{
		for(int i = 0; i < instAmount; i++)
		{
			bool attractSpawn = true;
			LargePassiveExpPoolSpawn (playerNumber, attractSpawn);
		}
	}

	void MegaExpAttract(int instAmount, int playerNumber)
	{
		for(int i = 0; i < instAmount; i++)
		{
			bool attractSpawn = true;
			MegaExpPoolSpawn (playerNumber, attractSpawn);
		}
	}

	void ActiveExpPoolSpawn(int playerNumber, bool attractSpawn)
	{
		for (int i = 0; i < gameManager.pooledActiveExpList.Count; i++) 
		{
			if (!gameManager.pooledActiveExpList [i].activeInHierarchy) 
			{
				Vector3 spawnForceVector = Vector3.zero;

				if (attractSpawn == false) 
				{
					spawnForceVector = new Vector3 ((Random.Range (0.35f, -0.35f)), 0, (Random.Range (-0.175f, -0.55f))) * expSpawnForce * 3;
				} 

				if (attractSpawn == true) 
				{
					spawnForceVector = new Vector3 ((Random.Range (-0.5f, 0.5f)), 0.3f, (Random.Range (0.4f, 0.9f))) * expSpawnForce * 4;
				}

				playerHitByNumber = playerNumber;

				gameManager.pooledActiveExpList [i].transform.position = transform.position;
				gameManager.pooledActiveExpList [i].transform.rotation = transform.rotation;
				gameManager.pooledActiveExpList [i].SetActive (true);
				gameManager.pooledActiveExpList [i].GetComponent<Rigidbody> ().velocity = Vector3.zero;
				gameManager.pooledActiveExpList [i].GetComponent<Rigidbody> ().AddForce (spawnForceVector, ForceMode.Impulse);

				if (attractSpawn == false) 
				{
					return;
				}

				if (attractSpawn == true)
				{
					if (playerHitByNumber == 1) 
					{
						gameManager.pooledActiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 1;
						damageTaken1 = 0;
					}

					if (playerHitByNumber == 2) 
					{
						gameManager.pooledActiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 2;
						damageTaken2 = 0;
					}

					if (playerHitByNumber == 3) 
					{
						gameManager.pooledActiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 3;
						damageTaken3 = 0;
					}

					if (playerHitByNumber == 4) 
					{
						gameManager.pooledActiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 4;
						damageTaken4 = 0;
					}

					return;
				}
			}
		}

		ActiveExpPoolExpand (playerNumber, attractSpawn);
	}

	void ActiveExpPoolExpand(int playerNumber, bool attractSpawn)
	{
		Debug.Log ("Added Active Exp to pool");
		GameObject activeExp = (GameObject)Instantiate (gameManager.activeExpObject);
		activeExp.SetActive (false);
		gameManager.pooledActiveExpList.Add (activeExp);
		ActiveExpPoolSpawn (playerNumber, attractSpawn);
	}

	void PassiveExpPoolSpawn(int playerNumber, bool attractSpawn)
	{
		for (int i = 0; i < gameManager.pooledPassiveExpList.Count; i++) 
		{
			if (!gameManager.pooledPassiveExpList [i].activeInHierarchy) 
			{
				Vector3 spawnForceVector = Vector3.zero;

				if (attractSpawn == false) 
				{
					spawnForceVector = new Vector3 ((Random.Range (0.3f, -0.3f)), 0, (Random.Range (-0.175f, -0.55f))) * expSpawnForce * 2;
				}

				if (attractSpawn == true) 
				{
					spawnForceVector = new Vector3 ((Random.Range (-0.5f, 0.5f)), 0.3f, (Random.Range (0.4f, 0.9f))) * expSpawnForce * 4;
				}
					
				playerHitByNumber = playerNumber;

				gameManager.pooledPassiveExpList [i].transform.position = transform.position;
				gameManager.pooledPassiveExpList [i].transform.rotation = transform.rotation;
				gameManager.pooledPassiveExpList [i].SetActive (true);
				gameManager.pooledPassiveExpList [i].GetComponent<Rigidbody> ().velocity = Vector3.zero;
				gameManager.pooledPassiveExpList [i].GetComponent<Rigidbody> ().AddForce (spawnForceVector, ForceMode.Impulse);


				if (attractSpawn == false) 
				{
					return;
				}

				if (attractSpawn == true)
				{

					if (playerHitByNumber == 1) 
					{
						gameManager.pooledPassiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 1;
						fiddyDamageTaken1 = 0;
					}

					if (playerHitByNumber == 2)
					{
						gameManager.pooledPassiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 2;
						fiddyDamageTaken2 = 0;
					}

					if (playerHitByNumber == 3) 
					{
						gameManager.pooledPassiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 3;
						fiddyDamageTaken3 = 0;
					}

					if (playerHitByNumber == 4) 
					{
						gameManager.pooledPassiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 4;
						fiddyDamageTaken4 = 0;
					}

					return;
				}
			}
		}

		PassiveExpPoolExpand (playerNumber, attractSpawn);
	}

	void PassiveExpPoolExpand(int playerNumber, bool attractSpawn)
	{
		Debug.Log ("Added Passive Exp to pool");
		GameObject passiveExp = (GameObject)Instantiate (gameManager.passiveExpObject);
		passiveExp.SetActive (false);
		gameManager.pooledPassiveExpList.Add (passiveExp);
		PassiveExpPoolSpawn (playerNumber, attractSpawn);
	}

	void LargeActiveExpPoolSpawn(int playerNumber, bool attractSpawn)
	{
		for (int i = 0; i < gameManager.pooledLargeActiveExpList.Count; i++) 
		{
			if (!gameManager.pooledLargeActiveExpList [i].activeInHierarchy) 
			{
				Vector3 spawnForceVector = Vector3.zero;

				if (attractSpawn == false) 
				{
					spawnForceVector = new Vector3 ((Random.Range (0.35f, -0.35f)), 0, (Random.Range (-0.175f, -0.55f))) * expSpawnForce * 3;
				} 

				if (attractSpawn == true) 
				{
					spawnForceVector = new Vector3 ((Random.Range (-0.5f, 0.5f)), 0.3f, (Random.Range (0.4f, 0.9f))) * expSpawnForce * 4;
				}

				playerHitByNumber = playerNumber;

				gameManager.pooledLargeActiveExpList [i].transform.position = transform.position;
				gameManager.pooledLargeActiveExpList [i].transform.rotation = transform.rotation;
				gameManager.pooledLargeActiveExpList [i].SetActive (true);
				gameManager.pooledLargeActiveExpList [i].GetComponent<Rigidbody> ().velocity = Vector3.zero;
				gameManager.pooledLargeActiveExpList [i].GetComponent<Rigidbody> ().AddForce (spawnForceVector, ForceMode.Impulse);

				if (attractSpawn == false)
				{
					return;
				}

				if (attractSpawn == true)
				{
					if (playerHitByNumber == 1) 
					{
						gameManager.pooledLargeActiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 1;
						damageTaken1 = 0;
					}

					if (playerHitByNumber == 2)
					{
						gameManager.pooledLargeActiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 2;
						damageTaken2 = 0;
					}

					if (playerHitByNumber == 3) 
					{
						gameManager.pooledLargeActiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 3;
						damageTaken3 = 0;
					}

					if (playerHitByNumber == 4) 
					{
						gameManager.pooledLargeActiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 4;
						damageTaken4 = 0;
					}

					return;
				}
			}
		}

		LargeActiveExpPoolExpand (playerNumber, attractSpawn);
	}

	void LargeActiveExpPoolExpand(int playerNumber, bool attractSpawn)
	{
		Debug.Log ("Added Large Active Exp to pool");
		GameObject largeActiveExp = (GameObject)Instantiate (gameManager.largeActiveExpObject);
		largeActiveExp.SetActive (false);
		gameManager.pooledLargeActiveExpList.Add (largeActiveExp);
		LargeActiveExpPoolSpawn (playerNumber, attractSpawn);
	}

	void LargePassiveExpPoolSpawn(int playerNumber, bool attractSpawn)
	{
		for (int i = 0; i < gameManager.pooledLargePassiveExpList.Count; i++) 
		{
			if (!gameManager.pooledLargePassiveExpList [i].activeInHierarchy) 
			{
				Vector3 spawnForceVector = Vector3.zero;

				if (attractSpawn == false) 
				{
					spawnForceVector = new Vector3 ((Random.Range (0.3f, -0.3f)), 0, (Random.Range (-0.175f, -0.55f))) * expSpawnForce * 2;
				}

				if (attractSpawn == true) 
				{
					spawnForceVector = new Vector3 ((Random.Range (-0.5f, 0.5f)), 0.3f, (Random.Range (0.4f, 0.9f))) * expSpawnForce * 4;
				}

				playerHitByNumber = playerNumber;

				gameManager.pooledLargePassiveExpList [i].transform.position = transform.position;
				gameManager.pooledLargePassiveExpList [i].transform.rotation = transform.rotation;
				gameManager.pooledLargePassiveExpList [i].SetActive (true);
				gameManager.pooledLargePassiveExpList [i].GetComponent<Rigidbody> ().velocity = Vector3.zero;
				gameManager.pooledLargePassiveExpList [i].GetComponent<Rigidbody> ().AddForce (spawnForceVector, ForceMode.Impulse);


				if (attractSpawn == false) 
				{
					return;
				}

				if (attractSpawn == true) 
				{

					if (playerHitByNumber == 1) 
					{
						gameManager.pooledLargePassiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 1;
						fiddyDamageTaken1 = 0;
					}

					if (playerHitByNumber == 2) 
					{
						gameManager.pooledLargePassiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 2;
						fiddyDamageTaken2 = 0;
					}

					if (playerHitByNumber == 3) 
					{
						gameManager.pooledLargePassiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 3;
						fiddyDamageTaken3 = 0;
					}

					if (playerHitByNumber == 4) 
					{
						gameManager.pooledLargePassiveExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 4;
						fiddyDamageTaken4 = 0;
					}

					return;
				}
			}
		}

		LargePassiveExpPoolExpand (playerNumber, attractSpawn);
	}

	void LargePassiveExpPoolExpand(int playerNumber, bool attractSpawn)
	{
		Debug.Log ("Added Large Passive Exp to pool");
		GameObject largePassiveExp = (GameObject)Instantiate (gameManager.largePassiveExpObject);
		largePassiveExp.SetActive (false);
		gameManager.pooledLargePassiveExpList.Add (largePassiveExp);
		LargePassiveExpPoolSpawn (playerNumber, attractSpawn);
	}

	void MegaExpPoolSpawn(int playerNumber, bool attractSpawn)
	{
		for (int i = 0; i < gameManager.pooledMegaExpList.Count; i++) 
		{
			if (!gameManager.pooledMegaExpList [i].activeInHierarchy) 
			{
				Vector3 spawnForceVector = Vector3.zero;

				if (attractSpawn == false) 
				{
					spawnForceVector = new Vector3 ((Random.Range (0.35f, -0.35f)), 0, (Random.Range (-0.175f, -0.55f))) * expSpawnForce * 3;
				} 

				if (attractSpawn == true) 
				{
					spawnForceVector = new Vector3 ((Random.Range (-0.5f, 0.5f)), 0.3f, (Random.Range (0.4f, 0.9f))) * expSpawnForce * 4;
				}

				playerHitByNumber = playerNumber;

				gameManager.pooledMegaExpList [i].transform.position = transform.position;
				gameManager.pooledMegaExpList [i].transform.rotation = transform.rotation;
				gameManager.pooledMegaExpList [i].SetActive (true);
				gameManager.pooledMegaExpList [i].GetComponent<Rigidbody> ().velocity = Vector3.zero;
				gameManager.pooledMegaExpList [i].GetComponent<Rigidbody> ().AddForce (spawnForceVector, ForceMode.Impulse);

				if (attractSpawn == false)
				{
					return;
				}

				if (attractSpawn == true)
				{
					if (playerHitByNumber == 1) 
					{
						gameManager.pooledMegaExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 1;
						damageTaken1 = 0;
					}

					if (playerHitByNumber == 2)
					{
						gameManager.pooledMegaExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 2;
						damageTaken2 = 0;
					}

					if (playerHitByNumber == 3) 
					{
						gameManager.pooledMegaExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 3;
						damageTaken3 = 0;
					}

					if (playerHitByNumber == 4) 
					{
						gameManager.pooledMegaExpList [i].GetComponent<ExpPickupBehaviour> ().globalAttractPlayerNumber = 4;
						damageTaken4 = 0;
					}

					return;
				}
			}
		}

		MegaExpPoolExpand (playerNumber, attractSpawn);
	}

	void MegaExpPoolExpand(int playerNumber, bool attractSpawn)
	{
		Debug.Log ("Added Mega Exp to pool");
		GameObject megaExp = (GameObject)Instantiate (gameManager.megaExpObject);
		megaExp.SetActive (false);
		gameManager.pooledMegaExpList.Add (megaExp);
		MegaExpPoolSpawn (playerNumber, attractSpawn);
	}
}
