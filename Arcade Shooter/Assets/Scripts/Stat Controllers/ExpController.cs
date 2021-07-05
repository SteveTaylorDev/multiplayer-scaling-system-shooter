using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpController : MonoBehaviour 
{
	public UIManager uiManager;

	public float activeExpAmount;
	public float passiveExpAmount;
	public int playerNumber;
	public Color playerColor;

	private PlayerMovement playerMoveScript;			// Used mainly for the player number reference
	private HealthController healthController;

	void Start()
	{
		GameObject uiManagerObject = GameObject.FindWithTag ("UIManager");
		uiManager = uiManagerObject.GetComponent<UIManager> ();

		playerMoveScript = gameObject.GetComponent<PlayerMovement> ();
		healthController = gameObject.GetComponent<HealthController> ();

		playerNumber = playerMoveScript.playerNumber;
		healthController.playerNumber = playerNumber;
	}

	void Update()
	{
		UpdatePlayerExpUI ();
	}

	void FixedUpdate()
	{
		CloseOverlapSphere ();
		FarOverlapSphere ();
	}

	void UpdatePlayerExpUI()
	{
		if(playerNumber == 1)
		{
			uiManager.player1ActiveExp = activeExpAmount;
		}

		if(playerNumber == 2)
		{
			uiManager.player2ActiveExp = activeExpAmount;
		}

		if(playerNumber == 3)
		{
			uiManager.player3ActiveExp = activeExpAmount;
		}

		if(playerNumber == 4)
		{
			uiManager.player4ActiveExp = activeExpAmount;
		}
	}

	void CloseOverlapSphere()
	{
		Collider[] overlappedColliders = Physics.OverlapSphere (gameObject.transform.position + Vector3.up, 2f);
		for (int i = 0; i < overlappedColliders.Length; i++) 
		{
			if (overlappedColliders [i].gameObject.layer == 11) 
			{
				ExpPickupBehaviour expBehaviour = overlappedColliders [i].GetComponent<ExpPickupBehaviour> ();

				if (expBehaviour.alreadyCollided == false) 
				{
					expBehaviour.collidedPlayerNumber = playerNumber;
					expBehaviour.collidedPlayerColor = playerColor;
					expBehaviour.alreadyCollided = true;
				}
			}
		}
	}

	void FarOverlapSphere()
	{
		Collider[] overlappedColliders = Physics.OverlapSphere (gameObject.transform.position + Vector3.up, 3f);
		for (int i = 0; i < overlappedColliders.Length; i++)
		{
			if (overlappedColliders [i].gameObject.layer == 11)
			{
				ExpPickupBehaviour expBehaviour = overlappedColliders [i].GetComponent<ExpPickupBehaviour> ();

				if (expBehaviour.alreadyCollided == false) 
				{
					expBehaviour.collidedPlayerNumber = playerNumber;
					expBehaviour.attractAmount = 0.01f;
					expBehaviour.inOverlapSphere = true;
				}
			}
		}
	}
}