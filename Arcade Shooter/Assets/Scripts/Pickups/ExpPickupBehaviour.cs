using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpPickupBehaviour : MonoBehaviour 
{

	public GameObject subLightsObject;

	public float activeExpValue;
	public float passiveExpValue;
	public Material trailMaterial;
	public SpriteRenderer expGlowSprite;
	public SpriteRenderer exp2DSprite;

	[HideInInspector]public float collidedPlayerNumber;
	[HideInInspector]public Color collidedPlayerColor;
	[HideInInspector]public float globalAttractTimer;
	[HideInInspector]public float attractAmount;

	[HideInInspector]public float boundaryDestroyCounter;

	[HideInInspector]public int globalAttractPlayerNumber;

	[HideInInspector]public bool startBoundaryDestroyCounter;
	[HideInInspector]public bool alreadyCollided;
	[HideInInspector]public bool inOverlapSphere;

	private bool spawnColliderTimerBool;
	[HideInInspector]public bool trailLimitRenableBool;

	private GameManager gameManager;
	private BoxCollider localCollider;
	private Rigidbody localRB;
	private TrailRenderer localTrailRend;
	[HideInInspector]public MeshRenderer localRend;

	private float spawnColliderTriggerTime;
	private float spawnColliderEnableTime;
	private float forceAttractTimer;
	[HideInInspector]public float trailColorEnableTimer;
	[HideInInspector]public float trailLimitReEnableTimer;

// Player Transforms
	private Transform player1Transform;
	private Transform player2Transform;
	private Transform player3Transform;
	private Transform player4Transform;


	void Awake()
	{
		GameObject gameManagerObject = GameObject.FindWithTag ("GameManager");
		gameManager = gameManagerObject.GetComponent<GameManager> ();

		localRB = gameObject.GetComponent<Rigidbody> ();

		localTrailRend = gameObject.GetComponent<TrailRenderer> ();
		localRend = gameObject.GetComponent<MeshRenderer> ();

		localCollider = gameObject.GetComponent<BoxCollider> ();
		localCollider.isTrigger = true;

		globalAttractPlayerNumber = 0;
		globalAttractTimer = 0.15f;
		attractAmount = 0.4f;

		spawnColliderTriggerTime = 1f;
		spawnColliderEnableTime = 0.25f;

		boundaryDestroyCounter = 2;

		forceAttractTimer = 0.4f;

		trailColorEnableTimer = 0.16f;
		trailLimitReEnableTimer = 0.15f;

		ExpSetColor ();
	}

	void OnEnable()
	{
		localCollider.enabled = false;
		localCollider.isTrigger = true;

		collidedPlayerNumber = 0;
		globalAttractPlayerNumber = 0;
		
		globalAttractTimer = 0.15f;
		attractAmount = 0.4f;
		
		spawnColliderTriggerTime = 1f;
		spawnColliderEnableTime = 0.25f;
		
		boundaryDestroyCounter = 2;
		
		forceAttractTimer = 0.4f;
		
		alreadyCollided = false;

		spawnColliderTimerBool = true;

		trailColorEnableTimer = 0.16f;
		trailLimitReEnableTimer = 0.15f;

		localTrailRend.time = 0;
		localTrailRend.material.SetColor ("_Color", Color.clear);
		localTrailRend.material.SetColor ("_EmissionColor", Color.clear);
	}

	void OnDisable()
	{
		spawnColliderTimerBool = false;
		trailLimitRenableBool = false;

		subLightsObject.SetActive (false);
		exp2DSprite.enabled = false;
		localRend.enabled = true;

		localTrailRend.time = 0;
		localTrailRend.enabled = false;
		localTrailRend.material.SetColor ("_Color", Color.clear);
		localTrailRend.material.SetColor ("_EmissionColor", Color.clear);
	}

	void Update()
	{
		inOverlapSphere = false;

		BoundaryDestroyCounter ();
		PlayerTransforms ();
	}

	void FixedUpdate()
	{
		if (trailLimitRenableBool == true) 
		{
			trailLimitReEnableTimer -= Time.deltaTime;

			if (trailLimitReEnableTimer <= 0) 
			{
				TrailLimitReEnable ();
			}
		}

		trailColorEnableTimer -= Time.deltaTime;
	
		if (trailColorEnableTimer <= 0) 
		{
			if (gameObject.CompareTag ("MegaExp"))
			{
				localTrailRend.time = 0.2f;

				if (alreadyCollided == false) 
				{
					localTrailRend.material.SetColor ("_Color", Color.magenta * 2);
					localTrailRend.material.SetColor ("_EmissionColor", Color.magenta * 2);
				}
			} 

			if (gameObject.CompareTag ("ActiveExp") || gameObject.CompareTag ("LargeActiveExp"))
			{
				localTrailRend.time = 0.1f;

				if (alreadyCollided == false) 
				{
					localTrailRend.material.SetColor ("_Color", localRend.material.color / 5);
					localTrailRend.material.SetColor ("_EmissionColor", localRend.material.color / 5);
				}
			}


			localTrailRend.time = 0.15f;

			if (alreadyCollided == false) 
			{
				localTrailRend.material.SetColor ("_Color", localRend.material.color / 5);
				localTrailRend.material.SetColor ("_EmissionColor", localRend.material.color / 5);
			}
		}

		if (spawnColliderTimerBool == true)
		{
			spawnColliderTriggerTime -= Time.deltaTime;
			spawnColliderEnableTime -= Time.deltaTime;
		}

		if (spawnColliderTriggerTime <= 0) 
		{
			localCollider.isTrigger = false;
		}

		if (spawnColliderEnableTime <= 0) 
		{
			localCollider.enabled = true;
		}

		PlayerAttractManager ();					// Continuously checks the collidedPlayerNumber for enabling the relevant Attract function
		GlobalPlayerAttractManager ();				// Continuously checks the globalCollidedPlayerNumber for enabling the relevant Attract function
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player")) 
		{
			ExpController playerExpController = other.gameObject.GetComponent<ExpController> ();

			if (collidedPlayerNumber == playerExpController.playerNumber) 							// If the local collidedPlayerNumber is the same as the collider player's number, then allow the exp object to be collected
			{
				playerExpController.activeExpAmount += activeExpValue;
				playerExpController.passiveExpAmount += passiveExpValue;

				gameObject.SetActive (false);
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.CompareTag ("Player")) 
		{
			ExpController playerExpController = other.gameObject.GetComponent<ExpController> ();

			if (collidedPlayerNumber == playerExpController.playerNumber) 							// If the local collidedPlayerNumber is the same as the collider player's number, then allow the exp object to be collected
			{
				playerExpController.activeExpAmount += activeExpValue;
				playerExpController.passiveExpAmount += passiveExpValue;

				gameObject.SetActive (false);
			}
		}
	}


	void ExpSetColor()
	{
		if(gameObject.CompareTag("ActiveExp"))
		{
			Color activeExpColor = new Color (1, 0.764f, 0);
			localRend.material.SetColor ("_Color", activeExpColor);
			expGlowSprite.color = activeExpColor;
			exp2DSprite.color = activeExpColor;
		}

		if (gameObject.CompareTag ("PassiveExp")) 
		{
			Color passiveExpColor = new Color (1, 0.058f, 0.349f);
			localRend.material.SetColor ("_Color", passiveExpColor);
			expGlowSprite.color = passiveExpColor;
			exp2DSprite.color = passiveExpColor;
		}

		if(gameObject.CompareTag("LargeActiveExp"))
		{
			Color largeActiveExpColor = new Color (1, 0.435f, 0);
			localRend.material.SetColor ("_Color", largeActiveExpColor);
			expGlowSprite.color = largeActiveExpColor;
			exp2DSprite.color = largeActiveExpColor;
		}

		if(gameObject.CompareTag("LargePassiveExp"))
		{
			Color largePassiveExpColor = new Color (0.556f, 0.098f, 0.964f);
			localRend.material.SetColor ("_Color", largePassiveExpColor);
			expGlowSprite.color = largePassiveExpColor;
			exp2DSprite.color = largePassiveExpColor;
		}
	}

	void BoundaryDestroyCounter()
	{
		if (startBoundaryDestroyCounter == true) 
		{
			boundaryDestroyCounter -= Time.deltaTime;
			if (boundaryDestroyCounter <= 0) 
			{
				gameObject.SetActive (false);
			}
		}
	}

	void PlayerAttractManager()
	{
		if (collidedPlayerNumber == 1) 
		{
			AttractToPlayer1 ();
		}

		if (collidedPlayerNumber == 2) 
		{
			AttractToPlayer2 ();
		}

		if (collidedPlayerNumber == 3) 
		{
			AttractToPlayer3 ();
		}

		if (collidedPlayerNumber == 4) 
		{
			AttractToPlayer4 ();
		}
	}

	void GlobalPlayerAttractManager()
	{
		if (globalAttractPlayerNumber == 1) 
		{
			globalAttractTimer -= Time.deltaTime;
			if (globalAttractTimer <= 0) 
			{
				localCollider.enabled = true;
				globalAttractTimer = 0.15f;
				collidedPlayerNumber = 1;
				alreadyCollided = true;
			}
		}

		if (globalAttractPlayerNumber == 2) 
		{
			globalAttractTimer -= Time.deltaTime;
			if (globalAttractTimer <= 0) 
			{
				localCollider.enabled = true;
				globalAttractTimer = 0.15f;
				collidedPlayerNumber = 2;
				alreadyCollided = true;
			}
		}

		if (globalAttractPlayerNumber == 3) 
		{
			globalAttractTimer -= Time.deltaTime;
			if (globalAttractTimer <= 0) 
			{
				localCollider.enabled = true;
				globalAttractTimer = 0.15f;
				collidedPlayerNumber = 3;
				alreadyCollided = true;
			}
		}

		if (globalAttractPlayerNumber == 4) 
		{
			globalAttractTimer -= Time.deltaTime;
			if (globalAttractTimer <= 0) 
			{
				localCollider.enabled = true;
				globalAttractTimer = 0.15f;
				collidedPlayerNumber = 4;
				alreadyCollided = true;
			}
		}
	}

	public void AttractToPlayer1()
	{
		if (player1Transform != null) 
		{
			forceAttractTimer -= Time.deltaTime;

			if ((globalAttractPlayerNumber != 0 && forceAttractTimer <= 0) || (globalAttractPlayerNumber == 0))
			{
				if (gameObject.transform.position != player1Transform.position) 
				{
					gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, player1Transform.position, attractAmount);
				}
			}

			if (inOverlapSphere == true && alreadyCollided == false)
			{
				transform.LookAt (player1Transform.position);
				localRB.AddRelativeForce (Vector3.forward * 1.5f);
			}

			if (inOverlapSphere == false && alreadyCollided == false) 
			{
				collidedPlayerNumber = 0;
			}

			if (alreadyCollided == true)
			{
				attractAmount = 0.4f;
				transform.LookAt (player1Transform.position);
				localRB.AddRelativeForce (Vector3.forward * 100);

				localTrailRend.material = trailMaterial;
				localTrailRend.material.SetColor ("_Color", gameManager.players[0].playerColor/2);
				localTrailRend.material.SetColor ("_EmissionColor", gameManager.players[0].playerColor / 2);
			}
		}
	}

	public void AttractToPlayer2()
	{
		if (player2Transform != null) 
		{
			forceAttractTimer -= Time.deltaTime;

			if ((globalAttractPlayerNumber != 0 && forceAttractTimer <= 0) || (globalAttractPlayerNumber == 0)) 
			{
				gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, player2Transform.position, attractAmount);
			}

			if (inOverlapSphere == true && alreadyCollided == false) 
			{
				transform.LookAt (player2Transform.position);
				localRB.AddRelativeForce (Vector3.forward * 1.5f);
			}

			if (inOverlapSphere == false && alreadyCollided == false) 
			{
				collidedPlayerNumber = 0;
			}

			if (alreadyCollided == true) 
			{
				attractAmount = 0.4f;
				transform.LookAt (player2Transform.position);
				localRB.AddRelativeForce (Vector3.forward * 100);

				localTrailRend.material = trailMaterial;
				localTrailRend.material.SetColor ("_Color", gameManager.players[1].playerColor/2);
				localTrailRend.material.SetColor ("_EmissionColor", gameManager.players[1].playerColor/2);
			}
		}
	}

	public void AttractToPlayer3()
	{
		if (player3Transform != null) 
		{
			forceAttractTimer -= Time.deltaTime;
			
			if ((globalAttractPlayerNumber != 0 && forceAttractTimer <= 0) || (globalAttractPlayerNumber == 0)) 
			{
				gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, player3Transform.position, attractAmount);
			}
			
			if (inOverlapSphere == true && alreadyCollided == false) 
			{
				transform.LookAt (player3Transform.position);
				localRB.AddRelativeForce (Vector3.forward * 1.5f);
			}
			
			if (inOverlapSphere == false && alreadyCollided == false) 
			{
				collidedPlayerNumber = 0;
			}
			
			if (alreadyCollided == true) 
			{
				attractAmount = 0.4f;
				transform.LookAt (player3Transform.position);
				localRB.AddRelativeForce (Vector3.forward * 100);
			
				localTrailRend.material = trailMaterial;
				localTrailRend.material.SetColor ("_Color", gameManager.players[2].playerColor/2);
				localTrailRend.material.SetColor ("_EmissionColor", gameManager.players[2].playerColor/2);
			}
		}
	}

	public void AttractToPlayer4()
	{
		if (player4Transform != null) 
		{
			forceAttractTimer -= Time.deltaTime;

			if ((globalAttractPlayerNumber != 0 && forceAttractTimer <= 0) || (globalAttractPlayerNumber == 0)) 
			{
				gameObject.transform.position = Vector3.MoveTowards (gameObject.transform.position, player4Transform.position, attractAmount);
			}

			if (inOverlapSphere == true && alreadyCollided == false) 
			{
				transform.LookAt (player4Transform.position);
				localRB.AddRelativeForce (Vector3.forward * 1.5f);
			}

			if (inOverlapSphere == false && alreadyCollided == false) 
			{
				collidedPlayerNumber = 0;
			}

			if (alreadyCollided == true) 
			{
				attractAmount = 0.4f;
				transform.LookAt (player4Transform.position);
				localRB.AddRelativeForce (Vector3.forward * 100);

				localTrailRend.material = trailMaterial;
				localTrailRend.material.SetColor ("_Color", gameManager.players[3].playerColor/2);
				localTrailRend.material.SetColor ("_EmissionColor", gameManager.players[3].playerColor/2);
			}
		}
	}

	void PlayerTransforms()
	{
		if (gameManager.players [0].instancePlayer != null) 
		{
			player1Transform = gameManager.players [0].instancePlayer.transform;
		}

		if (gameManager.players [1].instancePlayer != null) 
		{
			player2Transform = gameManager.players [1].instancePlayer.transform;
		}

		if (gameManager.players [2].instancePlayer != null)
		{
			player3Transform = gameManager.players [2].instancePlayer.transform;
		}

		if (gameManager.players [3].instancePlayer != null) 
		{
			player4Transform = gameManager.players [3].instancePlayer.transform;
		}
	}

	public void TrailLimitReEnable()
	{
		if(gameObject.CompareTag("ActiveExp") || gameObject.CompareTag("LargeActiveExp"))
		{
			localTrailRend.time = 0.1f;
			return;
		}

		localTrailRend.time = 0.15f;
	}
}
