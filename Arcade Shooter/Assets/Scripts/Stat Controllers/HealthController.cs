using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour 
{
	public GameManager gameManager;
	public UIManager uiManager;

	public GameObject healthColorObject;

	public float healthAmount;
	public float hitFlashLength;
	public int playerNumber;
	[HideInInspector]public float startingAmount;

	private float lastPlayerAmount;
	private float originalStartingAmount;
	private float difficultyScaling;
	private float flashCounter;
	private float enemyFlashCounter;

	private bool startingDifficulty;
	private bool healthHasBeenDiffScaled;
	[HideInInspector]public bool flashActive;
	[HideInInspector]public bool enemyFlashActive;

	// Use this for initialization
	void Start () 
	{
		GameObject gameManagerObject = GameObject.FindGameObjectWithTag ("GameManager");
		gameManager = gameManagerObject.GetComponent<GameManager> ();

		GameObject uiManagerObject = GameObject.FindWithTag ("UIManager");							// Note: Check these objects exist and are tagged if health things are breaking
		uiManager = uiManagerObject.GetComponent<UIManager> ();

		startingAmount = healthAmount;
		originalStartingAmount = startingAmount;

		startingDifficulty = gameManager.easyDifficulty;

		difficultyScaling = 0.5f;

		if (gameManager.activePlayerAmount != 0) 
		{
			MultiplayerHealthScaling ();
			DifficultyScaling ();
		}

		startingAmount = healthAmount;

		flashCounter = hitFlashLength;
		enemyFlashCounter = 0.05f;

		UpdatePlayerHealthUI ();
	}
	
	// Update is called once per frame
	void Update () 
	{ 
		UpdatePlayerHealthUI ();

		if (gameManager.activePlayerAmount != 0) 
		{
			if (lastPlayerAmount != gameManager.activePlayerAmount) 
			{
				MultiplayerHealthScaling ();
				DifficultyScaling ();
			}

			if (startingDifficulty != gameManager.easyDifficulty) 
			{
				DifficultyScaling ();
			}
			
			HealthColor ();

			CheckForDeath ();
		}

		if (flashActive == true) 
		{
			PlayerDamageAnim ();
		}

		if (enemyFlashActive == true) 
		{
			EnemyDamageAnim ();
		}
			
	}

	void CheckForDeath()
	{
		if (healthAmount <= 0) 
		{
			if (gameObject.CompareTag ("Player")) 
			{
				gameObject.SetActive (false);
			} 

			else 
			{
				BasicEnemyBehaviour localEnemyBehaviour = gameObject.GetComponent<BasicEnemyBehaviour> ();
				if(localEnemyBehaviour != null)
				{
					localEnemyBehaviour.spawnExp = true;
				}

				if (localEnemyBehaviour.finishedExpSpawn == true) 
				{
					Destroy (gameObject);
				}
			}
		}
	}

	void UpdatePlayerHealthUI()
	{
		if(playerNumber == 1)
		{
			uiManager.player1Health = healthAmount;
			uiManager.player1MaxHealth = startingAmount;
		}

		if(playerNumber == 2)
		{
			uiManager.player2Health = healthAmount;
			uiManager.player2MaxHealth = startingAmount;
		}

		if(playerNumber == 3)
		{
			uiManager.player3Health = healthAmount;
			uiManager.player3MaxHealth = startingAmount;
		}

		if(playerNumber == 4)
		{
			uiManager.player4Health = healthAmount;
			uiManager.player4MaxHealth = startingAmount;
		}
	}

	void HealthColor ()
	{
		float healthProportion = healthAmount / startingAmount;

		if (healthColorObject != null) 
		{
			MeshRenderer healthColorRend = healthColorObject.GetComponent<MeshRenderer> ();
			Color healthColor = Color.green;

			if (healthProportion <= 0.6f && healthProportion > 0.3f)
			{
				healthColor = Color.yellow;

				healthColorRend.material.SetColor ("_Color", healthColor);
				healthColorRend.material.SetColor ("_EmissionColor", healthColor /1.5f);
			} 

			else if (healthProportion <= 0.3f) 
			{
				healthColor = Color.red;

				healthColorRend.material.SetColor ("_Color", healthColor);
				healthColorRend.material.SetColor ("_EmissionColor", healthColor /1.5f);
			}

			else 
			{
				healthColorRend.material.SetColor ("_Color", healthColor);
				healthColorRend.material.SetColor ("_EmissionColor", healthColor /2);
			}

		}


		if (!gameObject.CompareTag ("Player")) 
		{
			Color currentColor = Color.Lerp (Color.red, Color.green, healthProportion);

			MeshRenderer rend = gameObject.GetComponent<MeshRenderer> ();
			rend.material.color = currentColor;
			rend.material.SetColor ("_EmissionColor", currentColor / 4);

			if (startingAmount <= 5) 
			{
				rend.material.color = Color.red;
				rend.material.SetColor ("_EmissionColor", Color.red / 4);
				return;
			}
	
			if (startingAmount <= (5 * gameManager.activePlayerAmount)) 
			{
				currentColor = Color.Lerp (Color.red, Color.green, healthProportion / 5);
				rend.material.color = currentColor;
				rend.material.SetColor ("_EmissionColor", currentColor / 4);
				return;
			}

			if (startingAmount <= (10 * gameManager.activePlayerAmount) && startingAmount > (5 * gameManager.activePlayerAmount)) 
			{
				currentColor = Color.Lerp (Color.red, Color.green, healthProportion / 3);
				rend.material.color = currentColor;
				rend.material.SetColor ("_EmissionColor", currentColor / 4);
				return;
			}

			if (startingAmount <= (25 * gameManager.activePlayerAmount) && startingAmount > (10 * gameManager.activePlayerAmount)) 
			{
				currentColor = Color.Lerp (Color.red, Color.green, healthProportion / 1.5f);
				rend.material.color = currentColor;
				rend.material.SetColor ("_EmissionColor", currentColor / 4);
				return;
			}
			
			if (startingAmount >= 1000 && startingAmount < 15000) 
			{
				Color purple = new Color (0.350f, 0.035f, 0.865f);
				currentColor = Color.Lerp (Color.red, purple, healthProportion);
				rend.material.color = currentColor;
				rend.material.SetColor ("_EmissionColor", currentColor / 4);
				return;
			}

			if (startingAmount >= 15000) 
			{
				currentColor = Color.Lerp (Color.red, Color.yellow, healthProportion);
				rend.material.color = currentColor;
				rend.material.SetColor ("_EmissionColor", currentColor / 4);
				return;
			}
				
		}
	}

	void MultiplayerHealthScaling()
	{
		if(!gameObject.CompareTag("Player"))
		{
			float healthPercentage = healthAmount / startingAmount;

			if (gameManager.activePlayerAmount > lastPlayerAmount) 
			{
				startingAmount = (originalStartingAmount * gameManager.activePlayerAmount);
				healthAmount = (healthPercentage * startingAmount);
				lastPlayerAmount = gameManager.activePlayerAmount;
			} 

			if (gameManager.activePlayerAmount < lastPlayerAmount)
			{
				if (healthHasBeenDiffScaled == true) 
				{
					// Temporarily negate the difficulty scaling if it's in effect
					startingAmount = startingAmount * 2;
					healthAmount = healthAmount * 2;

					// Work out the new health amounts
					startingAmount = (startingAmount / lastPlayerAmount) * (gameManager.activePlayerAmount);
					healthAmount = (healthPercentage * startingAmount);
					lastPlayerAmount = gameManager.activePlayerAmount;
				} 

				else 
				{
					// Work out the new health amounts
					startingAmount = (startingAmount / lastPlayerAmount) * (gameManager.activePlayerAmount);
					healthAmount = (healthPercentage * startingAmount);
					lastPlayerAmount = gameManager.activePlayerAmount;
				}
			}
		}
	}
		
	void DifficultyScaling()
	{
		if(!gameObject.CompareTag("Player"))
		{
			float healthPercentage = healthAmount / startingAmount;
			
			if (gameManager.easyDifficulty == true)
			{
				startingAmount = startingAmount * difficultyScaling;
				healthAmount = (healthPercentage * startingAmount);
				healthHasBeenDiffScaled = true;
			}

			if (gameManager.easyDifficulty == false) 
			{
				if (healthHasBeenDiffScaled == true) 
				{
					startingAmount = startingAmount * 2;
					healthAmount = healthPercentage * startingAmount;
					healthHasBeenDiffScaled = false;
				} 
			}

			startingDifficulty = gameManager.easyDifficulty;
		}
	}

	public void PlayerDamageAnim()
	{
		gameObject.layer = 13;

		flashCounter -= Time.deltaTime;

		MeshRenderer playerMeshRend = gameObject.GetComponent<MeshRenderer> ();
		MeshRenderer playerHealthRend = healthColorObject.GetComponent<MeshRenderer> ();


		if (flashCounter > hitFlashLength * 0.66f) 
		{
			Color currentColor = playerMeshRend.material.color;
			Color currentHealthColor = playerHealthRend.material.color;

			playerMeshRend.material.SetColor ("_Color", currentColor / 10);
			playerMeshRend.material.SetColor ("_EmissionColor", currentColor / 10);

			playerHealthRend.material.SetColor ("_Color", currentHealthColor / 10);
			playerHealthRend.material.SetColor ("_EmissionColor", currentHealthColor /2);
		}

		if (flashCounter > hitFlashLength * 0.33f && flashCounter < hitFlashLength * 0.66f) 
		{
			Color currentColor = playerMeshRend.material.color;
			Color currentHealthColor = playerHealthRend.material.color;

			playerMeshRend.material.SetColor ("_Color", currentColor / 5);
			playerMeshRend.material.SetColor ("_EmissionColor", currentColor / 5);

			playerHealthRend.material.SetColor ("_Color", currentHealthColor / 5);
			playerHealthRend.material.SetColor ("_EmissionColor", currentHealthColor /2);
		}

		if (flashCounter > 0 && flashCounter < hitFlashLength * 0.33f) 
		{
			Color currentColor = playerMeshRend.material.color;
			Color currentHealthColor = playerHealthRend.material.color;

			playerMeshRend.material.SetColor ("_Color", currentColor / 10);
			playerMeshRend.material.SetColor ("_EmissionColor", currentColor / 10);

			playerHealthRend.material.SetColor ("_Color", currentHealthColor / 10);
			playerHealthRend.material.SetColor ("_EmissionColor", currentHealthColor /2);
		}

		if (flashCounter <= 0 && flashCounter > -0.5f) 
		{
			Color currentColor = playerMeshRend.material.color;
			Color currentHealthColor = playerHealthRend.material.color;

			playerMeshRend.material.SetColor ("_Color", currentColor / 5);
			playerMeshRend.material.SetColor ("_EmissionColor", currentColor / 5);

			playerHealthRend.material.SetColor ("_Color", currentHealthColor / 5);
			playerHealthRend.material.SetColor ("_EmissionColor", currentHealthColor /2);
		}

		if (flashCounter <= -0.5f) 
		{
			Color currentColor = playerMeshRend.material.color;
			Color currentHealthColor = playerHealthRend.material.color;

			playerMeshRend.material.SetColor ("_Color", currentColor);
			playerMeshRend.material.SetColor ("_EmissionColor", currentColor / 4);

			playerHealthRend.material.SetColor ("_Color", currentHealthColor);
			playerHealthRend.material.SetColor ("_EmissionColor", currentHealthColor /2);

			ResetFlashCounter ();
			gameObject.layer = 12;
			flashActive = false;
		}
	}

	void ResetFlashCounter()
	{
		flashCounter = hitFlashLength;
	}

	public void EnemyDamageAnim()
	{
		enemyFlashCounter -= Time.deltaTime;

		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer> ();

		if (enemyFlashCounter > 0) 
		{
			if (startingAmount >= 15000) 
			{
				Color brighterRed = new Color (1f, 0.350f, 0.420f);
				meshRenderer.material.color = brighterRed;
				meshRenderer.material.SetColor ("_EmissionColor", brighterRed);
			} 

			else 
			{
				Color brightRed = new Color (1f, 0.2f, 0.25f);
				meshRenderer.material.color = brightRed;
				meshRenderer.material.SetColor ("_EmissionColor", brightRed);
			}
		}
	
		else 
		{
			meshRenderer.material.SetColor ("_EmissionColor", Color.black);
			enemyFlashActive = false;
			ResetEnemyFlashCounter ();
		}

	}

	public void ResetEnemyFlashCounter()
	{
		enemyFlashCounter = 0.05f;
	}
}
