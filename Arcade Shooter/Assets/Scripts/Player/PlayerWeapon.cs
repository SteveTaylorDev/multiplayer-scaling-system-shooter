using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {

	private GameManager gameManager;

	// Player Attributes
	public int playerNumber;
	public Color playerColor;
	public float shotSpeed;
	public float shotDamage;
	public float fireRate;				

	public Rigidbody playerRigidbody;
	public ExpController playerExpController;

	// Input Wrappers
	private bool mainFire;

	// Input Strings
	private string mainFireString;

	// Weapon Spawn Transforms
	public Transform weaponSpawnCenter;
	public Transform weaponSpawnLeft;
	public Transform weaponSpawnRight;

	// Shot Objects
	public GameObject basicShotObject;


	private float nextShot;


	// Use this for initialization
	void Start () 
	{
		GameObject gameManagerObject = GameObject.FindWithTag ("GameManager");
		gameManager = gameManagerObject.GetComponent<GameManager> ();

		mainFireString = "MainFire" + playerNumber;

		playerRigidbody = gameObject.GetComponent<Rigidbody> ();
		playerExpController = gameObject.GetComponent<ExpController> ();
	}
	
	// Update is called once per frame
	void Update () 
	{		
		playerExpController.playerColor = playerColor;

		// Setting up the mainFire bool to represent GetButtonDown and GetButtonUp
		if(Input.GetButtonDown (mainFireString))
		{
			mainFire = true;
		}

		if (Input.GetButtonUp (mainFireString)) 
		{
			mainFire = false;
		}

		// If the fire button is pressed, run the fire weapon function
		if(mainFire == true && Time.time > nextShot)
		{
			nextShot = Time.time + fireRate;
			FireMainWeapon ();
		}
	}


	void FireMainWeapon ()
	{
		for (int i = 0; i < gameManager.pooledBasicShotList.Count; i++) 
		{
			if (!gameManager.pooledBasicShotList [i].activeInHierarchy) 
			{
				gameManager.pooledBasicShotList [i].transform.position = weaponSpawnCenter.position;
				gameManager.pooledBasicShotList [i].transform.rotation = weaponSpawnCenter.rotation;
				gameManager.pooledBasicShotList [i].SetActive (true);


				gameManager.pooledBasicShotList [i].GetComponent<MeshRenderer> ().material.color = playerColor;
				gameManager.pooledBasicShotList [i].GetComponent<ShotBehaviour> ().globalShotSpeed = shotSpeed;
				gameManager.pooledBasicShotList [i].GetComponent<ShotBehaviour> ().globalShotDamage = shotDamage;
				gameManager.pooledBasicShotList [i].GetComponent<ShotBehaviour> ().playerNumber = playerNumber;
				gameManager.pooledBasicShotList [i].GetComponent<ShotBehaviour> ().playerCurrentSpeed = playerRigidbody.velocity;
				gameManager.pooledBasicShotList [i].GetComponent<Light> ().color = playerColor;
				return;
			}
		}

		BasicShotPoolExpand ();
	}

	void BasicShotPoolExpand()
	{
		Debug.Log ("Added Basic Shot to pool");
		GameObject basicShot = (GameObject)Instantiate (gameManager.basicShotObject);
		basicShot.SetActive (false);
		gameManager.pooledBasicShotList.Add (basicShot);
		FireMainWeapon ();
	}
}
