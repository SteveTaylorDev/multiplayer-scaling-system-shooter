using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
	// Player attributes
	public int playerNumber;
	public Color playerColor;
	public float playerSpeed;
	public float playerTiltAmount;
	public bool controlEnabled;

	// Input wrappers
	private float inputHorizontal;
	private float inputVertical;

	// Input strings
	private string inputHorizontalString;
	private string inputVerticalString;

	private GameManager gameManager;
	private Rigidbody playerRB;

	[HideInInspector]public int collidedObjects;
	[HideInInspector]public float controlCounter;

	// Use this for initialization
	void Start () 
	{
		controlEnabled = true;
		controlCounter = 0.125f;
		collidedObjects = 0;

		// Cheeky check for if player number is 0, destroys the game object if true
		if (playerNumber == 0)
		{
			Debug.Log ("ERROR: Player lower than 1 detected. Removing. If you wanted Player 0 for some reason, sorry. Ain't happening.");
			Destroy (gameObject);
		}
		
		GameObject gameManagerObject = GameObject.FindWithTag ("GameManager");
		gameManager = gameManagerObject.GetComponent<GameManager>();

		playerRB = gameObject.GetComponent<Rigidbody> ();

		inputHorizontalString = "Horizontal" + playerNumber;
		inputVerticalString = "Vertical" + playerNumber;
	}
	
	// Update is called once per frame
	void Update () 
	{
		inputHorizontal = Input.GetAxis (inputHorizontalString);
		inputVertical = Input.GetAxis (inputVerticalString);

		if (collidedObjects >= 1) 		// Resets the collided enemy object count each frame
		{
			collidedObjects = 0;
		}

		ControlEnableCounter();
	}

	void FixedUpdate()
	{
		if (controlEnabled == true) 
		{
			MovePlayer ();
		}
	}

	void MovePlayer ()
	{
		Vector3 moveVector = new Vector3 (inputHorizontal, 0, inputVertical);
		playerRB.velocity = moveVector * playerSpeed;
		playerRB.position = new Vector3 
			(
			Mathf.Clamp (playerRB.position.x, gameManager.boundaryP1.xMin, gameManager.boundaryP1.xMax),
			0,
			Mathf.Clamp (playerRB.position.z, gameManager.boundaryP1.zMin, gameManager.boundaryP1.zMax)
			);

		playerRB.rotation = Quaternion.Euler (0f, 0f, (playerRB.velocity.x /1.5f) * -playerTiltAmount);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == 10)
		{
			collidedObjects++;					// Adds 1 to the collidedObjects int whenever colliding with an object tagged as Enemy (Layer 10) 
		}
	}

	void ControlEnableCounter()
	{
		if (controlEnabled == false) 
		{
			controlCounter -= Time.deltaTime;
		}

		if (controlCounter <= 0.025f) 
		{
			playerRB.velocity = Vector3.zero;
		}

		if (controlCounter <= 0) 
		{
			controlEnabled = true;
			controlCounter = 0.125f;
		}
	}
}
