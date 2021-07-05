using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerManager 
{
	// Player Attributes
	public int playerNumber;
	public Color playerColor;
	public float playerHealth;					// Starts at 100
	public float playerSpeed;					// Starts at 7
	public float playerTiltAmount;				// Usually 3
	public float shotSpeed;						// Starts at 1
	public float shotDamage;					// Starts at 1 (5 damage per Basic Shot)
	public float fireRate;						// Starts at 0.5

	// Player Setup Variables
	[HideInInspector]public GameObject instancePlayer;
	[HideInInspector]public PlayerMovement instancePlayerMovement;
	[HideInInspector]public PlayerWeapon instancePlayerWeapon;

	public Transform playerSpawnpoint;
	public Transform playerSpawnpointMP;
		

	public void SetupPlayer()
	{
		if (instancePlayer == null) 
		{
			return;
		}

		// Get the movement script and send the instance stats to it
		instancePlayerMovement = instancePlayer.GetComponent<PlayerMovement> ();
		instancePlayerMovement.playerNumber = playerNumber;
		instancePlayerMovement.playerColor = playerColor;
		instancePlayerMovement.playerSpeed = playerSpeed;
		instancePlayerMovement.playerTiltAmount = playerTiltAmount;

		// Get the weapon script and send the instance stats to it
		instancePlayerWeapon = instancePlayer.GetComponent<PlayerWeapon>();
		instancePlayerWeapon.playerNumber = playerNumber;
		instancePlayerWeapon.playerColor = playerColor;
		instancePlayerWeapon.shotSpeed = shotSpeed;
		instancePlayerWeapon.shotDamage = shotDamage;
		instancePlayerWeapon.fireRate = fireRate;

		// Gets the mesh renderer from the instance and changes its material color to the player color
		MeshRenderer instancePlayerRend = instancePlayer.GetComponent<MeshRenderer> ();
		instancePlayerRend.material.color = playerColor;
		instancePlayerRend.material.SetColor ("_EmissionColor", playerColor / 4);
	}

	public void SetPlayerHealth()
	{
		// Get the health script and send the player health stats to it
		HealthController instanceHealthController = instancePlayer.GetComponent<HealthController> ();
		instanceHealthController.healthAmount = playerHealth;
	}

	public void DisableAllMovement()
	{
		instancePlayerMovement = instancePlayer.GetComponent<PlayerMovement> ();
		instancePlayerMovement.enabled = false;
	}
}
