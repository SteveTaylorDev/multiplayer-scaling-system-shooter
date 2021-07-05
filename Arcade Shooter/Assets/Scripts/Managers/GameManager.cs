using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class GameManager : MonoBehaviour {

	public int activePlayerAmount;
	private int currentPlayerAmount;

	public Boundary boundaryP1;
	public Boundary boundaryP2;

	public GameObject firstCamera;
	private CameraSettings firstCameraSettings;

	public GameObject secondCamera;
	private CameraSettings secondCameraSettings;

	public GameObject playerObject;

	public GameObject basicShotObject;

	public GameObject activeExpObject;
	public GameObject passiveExpObject;
	public GameObject largeActiveExpObject;
	public GameObject largePassiveExpObject;
	public GameObject megaExpObject;

	public int pooledBasicShotAmount;		
	public int pooledActiveExpAmount;		
	public int pooledPassiveExpAmount;
	public int pooledLargeActiveExpAmount;
	public int pooledLargePassiveExpAmount;
	public int pooledMegaExpAmount;


// Object Pooling Lists
	[HideInInspector]public List<GameObject> pooledBasicShotList;
	[HideInInspector]public List<GameObject> pooledActiveExpList;
	[HideInInspector]public List<GameObject> pooledPassiveExpList;
	[HideInInspector]public List<GameObject> pooledLargeActiveExpList;
	[HideInInspector]public List<GameObject> pooledLargePassiveExpList;
	[HideInInspector]public List<GameObject> pooledMegaExpList;


	public int maximumBasicShotLights;

	public int maximumActiveExpLights;
	public int maximumActiveExpTrails;
	public int maximumActiveExp3D;

	public int maximumPassiveExpLights;
	public int maximumPassiveExpTrails;
	public int maximumPassiveExp3D;

	public int maximumLargeActiveExpLights;
	public int maximumLargeActiveExpTrails;
	public int maximumLargeActiveExp3D;

	public int maximumLargePassiveExpLights;
	public int maximumLargePassiveExpTrails;
	public int maximumLargePassiveExp3D;

	public int maximumMegaExpLights;
	public int maximumMegaExpTrails;


	public bool easyDifficulty;

	public bool gametypeCoop;
	public bool gametypeMP;

	public bool multiplayerCamera;
	public bool coopCamera;

	private bool zeroPlayersBool;
	private bool pressStart;

	public PlayerManager[] players;

	[HideInInspector]public GameObject player1Instance;
	[HideInInspector]public GameObject player2Instance;
	[HideInInspector]public GameObject player3Instance;
	[HideInInspector]public GameObject player4Instance;

// Player stats (for reset)
	[HideInInspector]public Vector3 player1Position;
	[HideInInspector]public Vector3 player2Position;
	[HideInInspector]public Vector3 player3Position;
	[HideInInspector]public Vector3 player4Position;



	// Use this for initialization
	void Start () 
	{
		if (activePlayerAmount <= 0) 
		{
			Debug.Log ("There are zero active players, setting to 1.");
			activePlayerAmount = 1;
			zeroPlayersBool = true;
		}

		firstCameraSettings = firstCamera.GetComponent<CameraSettings> ();
		secondCameraSettings = secondCamera.GetComponent<CameraSettings> ();

		player1Position = players [0].playerSpawnpoint.transform.position;
		if(players.Length > 0) player2Position = players [1].playerSpawnpoint.transform.position;
		if(players.Length > 1) player3Position = players [2].playerSpawnpoint.transform.position;
		if(players.Length > 2) player4Position = players [3].playerSpawnpoint.transform.position;

	// Pooling Functions
		PoolActiveExp ();
		PoolPassiveExp ();
		PoolLargeActiveExp ();
		PoolLargePassiveExp ();
		PoolMegaExp ();
		PoolBasicShots ();
	//

		GametypeManager ();
		SetupPlayers ();
		SetPlayersHealth ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (pressStart == true) 
		{
			PressStart ();
		}

		if (activePlayerAmount <= 0) 
		{
			Debug.Log ("There are zero active players, setting to 1.");
			activePlayerAmount = 1;
			zeroPlayersBool = true;
		}

		GametypeManager ();
		PlayerJoinHandler ();
		RefreshPlayerStats ();
		CameraModeChanger ();

		LimitExpEffects ();
		LimitShotEffects ();


	// Debug Controls
		if(Input.GetButtonDown("AddPlayer") || Input.GetKeyDown(KeyCode.Equals))
		{
			activePlayerAmount++;
		}

		if (Input.GetButtonDown ("RemovePlayer") || Input.GetKeyDown(KeyCode.Minus)) 
		{
			activePlayerAmount--;
		}

		if (Input.GetButtonDown ("ResetScene") || Input.GetKeyDown(KeyCode.R)) 
		{
			SceneManager.LoadScene ("Game");
		}
	}
		

	void FixedUpdate()
	{
		PlayerInstancesHandler ();
	}


	void PoolBasicShots()
	{
		pooledBasicShotList = new List<GameObject> ();
		for (int i = 0; i < pooledBasicShotAmount; i++) 
		{
			GameObject basicShot = (GameObject)Instantiate (basicShotObject);
			basicShot.SetActive (false);
			pooledBasicShotList.Add (basicShot);
		}
	}


	void PoolActiveExp()
	{
		pooledActiveExpList = new List<GameObject> ();
		for (int i = 0; i < pooledActiveExpAmount; i++) 
		{
			GameObject activeExp = (GameObject)Instantiate (activeExpObject);
			activeExp.SetActive (false);
			pooledActiveExpList.Add (activeExp);
		}
	}

	void PoolPassiveExp()
	{
		pooledPassiveExpList = new List<GameObject> ();
		for (int i = 0; i < pooledPassiveExpAmount; i++) 
		{
			GameObject passiveExp = (GameObject)Instantiate (passiveExpObject);
			passiveExp.SetActive (false);
			pooledPassiveExpList.Add (passiveExp);
		}
	}

	void PoolLargeActiveExp()
	{
		pooledLargeActiveExpList = new List<GameObject> ();
		for (int i = 0; i < pooledLargeActiveExpAmount; i++) 
		{
			GameObject largeActiveExp = (GameObject)Instantiate (largeActiveExpObject);
			largeActiveExp.SetActive (false);
			pooledLargeActiveExpList.Add (largeActiveExp);
		}
	}

	void PoolLargePassiveExp()
	{
		pooledLargePassiveExpList = new List<GameObject> ();
		for (int i = 0; i < pooledLargePassiveExpAmount; i++) 
		{
			GameObject largePassiveExp = (GameObject)Instantiate (largePassiveExpObject);
			largePassiveExp.SetActive (false);
			pooledLargePassiveExpList.Add (largePassiveExp);
		}
	}

	void PoolMegaExp()
	{
		pooledMegaExpList = new List<GameObject> ();
		for (int i = 0; i < pooledMegaExpAmount; i++) 
		{
			GameObject megaExp = (GameObject)Instantiate (megaExpObject);
			megaExp.SetActive (false);
			pooledMegaExpList.Add (megaExp);
		}
	}



	void SetupPlayers ()
	{
		for (int i = 0; i < activePlayerAmount; i++) 
		{
			players [i].playerNumber = i + 1;

			if (players [i].playerSpawnpoint == null) 				// Check for singleplayer and coop spawnpoints
			{
				Debug.Log ("Missing a spawnpoint for player " + (i+1));
			}

		// Instantiates the player in the coop spawnpoint if the gametype is coop
			if (gametypeCoop == true && gametypeMP == false || gametypeCoop == false && gametypeMP == false) 		
			{
				players [i].instancePlayer = Instantiate (playerObject, players [i].playerSpawnpoint.position, Quaternion.Euler (0, 0, 0)) as GameObject;
			}

			if (players [i].playerSpawnpointMP == null) 			// Check for multiplayer spawnpoints
			{
				Debug.Log ("Missing a multiplayer spawnpoint for player " + (i+1));
			}

		// Instantiates the player in the MP spawnpoint if the gametype is MP
			if (gametypeMP == true && gametypeCoop == false) 				
			{
				players [i].instancePlayer = Instantiate (playerObject, players [i].playerSpawnpointMP.position, Quaternion.Euler (0, 0, 0)) as GameObject;
			}

			players [i].SetupPlayer();

			currentPlayerAmount = activePlayerAmount;
		}
	}

	void SetPlayersHealth()
	{
		for (int i = 0; i < activePlayerAmount; i++) 
		{
			players [i].SetPlayerHealth();
		}
	}
		

	void RefreshPlayerStats()
	{
		if (currentPlayerAmount == activePlayerAmount) 
		{
			for (int i = 0; i < activePlayerAmount; i++) 
			{
				players [i].SetupPlayer ();
			}
		}
	}

	void ResetPlayers()
	{
		GameObject[] allPlayers = GameObject.FindGameObjectsWithTag ("Player");

		for (int i = 0; i < allPlayers.Length; i++) 
		{
			Destroy (allPlayers [i]);
		}
	}

	void CameraModeChanger()
	{
		if (activePlayerAmount == 1 && multiplayerCamera == true || activePlayerAmount > 1 && coopCamera == false || coopCamera == true && multiplayerCamera == true) 
		{
			secondCamera.SetActive (true);
			firstCameraSettings.multiplayerCamera = true;
			secondCameraSettings.multiplayerCamera = true;
		}

		if (coopCamera == true && multiplayerCamera == false || activePlayerAmount == 1 && multiplayerCamera == false) 
		{
			firstCameraSettings.multiplayerCamera = false;
			secondCamera.SetActive (false);
		}
	}

	void PlayerJoinHandler()
	{
		if (currentPlayerAmount > players.Length)  
		{
			currentPlayerAmount = players.Length;
		}

		if (activePlayerAmount > players.Length) 
		{
			activePlayerAmount = players.Length;
			Debug.Log ("Active player amount is greater than the Players array. Setting to array length."); 
		}

		if (currentPlayerAmount != activePlayerAmount) 
		{
			if (player1Instance != null) 
			{
				player1Instance.SetActive (true);
			}

			if (player2Instance != null) 
			{
				player2Instance.SetActive (true);
			}

			if (player3Instance != null) 
			{
				player3Instance.SetActive (true);
			}

			if (player4Instance != null) 
			{
				player4Instance.SetActive (true);
			}

			LogPlayerStats ();
			ResetPlayers ();
			SetupPlayers ();
			SetPlayersHealth ();
			ApplyPlayerStats ();
		}
	}

	void GametypeManager()
	{
		if (gametypeCoop == true) 
		{
			coopCamera = true;
			gametypeMP = false;
		}

		if (gametypeMP == true) 
		{
			multiplayerCamera = true;
			gametypeCoop = false;
		}

		if (gametypeMP == true && multiplayerCamera == true) 
		{
			coopCamera = false;
		}

		if (gametypeMP == false && gametypeCoop == false) 
		{
			coopCamera = false;
			multiplayerCamera = false;
		}
	}

	void PlayerInstancesHandler()
	{
		player1Instance = players [0].instancePlayer;

		if (zeroPlayersBool == true) 
		{
			player1Instance.SetActive (false);
			pressStart = true;
			zeroPlayersBool = false;
		}
	
		if (players [1].instancePlayer != null) 
		{
			player2Instance = players [1].instancePlayer;
		}

		if (players [2].instancePlayer != null) 
		{
			player3Instance = players [2].instancePlayer;
		}

		if (players [3].instancePlayer != null) 
		{
			player4Instance = players [3].instancePlayer;
		}
	}

	void PressStart ()
	{
		if (Input.anyKeyDown) 
		{
			if (player1Instance != null) 
			{
				player1Instance.SetActive (true);
			}

			pressStart = false;

			ResetPlayers ();
			SetupPlayers ();
			SetPlayersHealth ();
		}
	}

	void LogPlayerStats()
	{
	// Positions
		player1Position = players [0].instancePlayer.transform.position;

		if (currentPlayerAmount >= 2)
		{
			player2Position = players [1].instancePlayer.transform.position;
		}

		if (currentPlayerAmount >= 3) 
		{
			player3Position = players [2].instancePlayer.transform.position;
		}

		if (currentPlayerAmount >= 4) 
		{
			player4Position = players [3].instancePlayer.transform.position;
		}

	}

	void ApplyPlayerStats()
	{
	// Positions
		players [0].instancePlayer.transform.position = player1Position;

		if (currentPlayerAmount >= 2) 
		{
			players [1].instancePlayer.transform.position = player2Position;
		}

		if (currentPlayerAmount >= 3) 
		{
			players [2].instancePlayer.transform.position = player3Position;
		}

		if (currentPlayerAmount >= 4) 
		{
			players [3].instancePlayer.transform.position = player4Position;
		}
	}


	void LimitShotEffects()
	{
		
	// Basic Shot
		int basicShotEnabledAmount = 0;

		for(int i = 0; i < pooledBasicShotList.Count; i++)
		{
			if (pooledBasicShotList [i].activeInHierarchy) 
			{
				basicShotEnabledAmount += 1;

				if (basicShotEnabledAmount <= maximumBasicShotLights) 
				{
					pooledBasicShotList [i].GetComponent<Light> ().enabled = true;
				}
			}
		}
	}

	void LimitExpEffects()
	{
		
// Active Exp
		int activeExpEnabledAmount = 0;

		for(int i = 0; i < pooledActiveExpList.Count; i++)
		{
			if (pooledActiveExpList [i].activeInHierarchy) 
			{
				activeExpEnabledAmount += 1;

				ExpPickupBehaviour activeExpBehaviour = pooledActiveExpList [i].GetComponent<ExpPickupBehaviour> ();

				if (activeExpEnabledAmount <= maximumActiveExpLights) 
				{
					activeExpBehaviour.subLightsObject.SetActive (true);
				}

				if (activeExpEnabledAmount <= maximumActiveExpTrails) 
				{
					if (activeExpBehaviour.trailLimitRenableBool == false)
					{
						TrailRenderer activeExpTrailRend = pooledActiveExpList [i].GetComponent<TrailRenderer> ();
						activeExpTrailRend.time = 0;
						activeExpTrailRend.enabled = true;

						activeExpBehaviour.trailLimitReEnableTimer = 0.15f;
						activeExpBehaviour.trailLimitRenableBool = true;
					}
				}

				if (activeExpEnabledAmount > maximumActiveExp3D) 
				{
					activeExpBehaviour.localRend.enabled = false;
					activeExpBehaviour.exp2DSprite.enabled = true;				
				}
			}
		}

// Passive Exp
		int passiveExpEnabledAmount = 0;

		for(int i = 0; i < pooledPassiveExpList.Count; i++)
		{
			if (pooledPassiveExpList [i].activeInHierarchy) 
			{
				passiveExpEnabledAmount += 1;

				ExpPickupBehaviour passiveExpBehaviour = pooledPassiveExpList [i].GetComponent<ExpPickupBehaviour> ();

				if (passiveExpEnabledAmount <= maximumPassiveExpLights) 
				{
					passiveExpBehaviour .subLightsObject.SetActive (true);
				}

				if (passiveExpEnabledAmount <= maximumPassiveExpTrails) 
				{
					if (passiveExpBehaviour .trailLimitRenableBool == false)
					{	
						TrailRenderer passiveExpTrailRend = pooledPassiveExpList [i].GetComponent<TrailRenderer> ();
						passiveExpTrailRend.time = 0;
						passiveExpTrailRend.enabled = true;
							  
						passiveExpBehaviour .trailLimitReEnableTimer = 0.15f;
						passiveExpBehaviour .trailLimitRenableBool = true;
					}
				}

				if (passiveExpEnabledAmount > maximumPassiveExp3D) 
				{
					passiveExpBehaviour.localRend.enabled = false;
					passiveExpBehaviour.exp2DSprite.enabled = true;				
				}

			}
		}

// Large Active Exp
		int largeActiveExpEnabledAmount = 0;

		for(int i = 0; i < pooledLargeActiveExpList.Count; i++)
		{
			if (pooledLargeActiveExpList [i].activeInHierarchy) 
			{
				largeActiveExpEnabledAmount += 1;

				ExpPickupBehaviour largeActiveExpBehaviour = pooledLargeActiveExpList [i].GetComponent<ExpPickupBehaviour> ();

				if (largeActiveExpEnabledAmount <= maximumLargeActiveExpLights) 
				{
					pooledLargeActiveExpList [i].GetComponent<ExpPickupBehaviour> ().subLightsObject.SetActive (true);
				}

				if (largeActiveExpEnabledAmount <= maximumLargeActiveExpTrails) 
				{
					if (largeActiveExpBehaviour.trailLimitRenableBool == false)
					{	
						TrailRenderer largeActiveExpTrailRend = pooledLargeActiveExpList [i].GetComponent<TrailRenderer> ();
						largeActiveExpTrailRend.time = 0;
						largeActiveExpTrailRend.enabled = true;
							  
						largeActiveExpBehaviour.trailLimitReEnableTimer = 0.15f;
						largeActiveExpBehaviour.trailLimitRenableBool = true;
					}
				}

				if (largeActiveExpEnabledAmount > maximumLargeActiveExp3D) 
				{
					largeActiveExpBehaviour.localRend.enabled = false;
					largeActiveExpBehaviour.exp2DSprite.enabled = true;				
				}
			}
		}

// Large Passive Exp
		int largePassiveExpEnabledAmount = 0;

		for(int i = 0; i < pooledLargePassiveExpList.Count; i++)
		{
			if (pooledLargePassiveExpList [i].activeInHierarchy) 
			{
				largePassiveExpEnabledAmount += 1;

				ExpPickupBehaviour largePassiveExpBehaviour = pooledLargePassiveExpList [i].GetComponent<ExpPickupBehaviour> ();

				if (largePassiveExpEnabledAmount <= maximumLargePassiveExpLights) 
				{
					largePassiveExpBehaviour.subLightsObject.SetActive (true);
				}

				if (largePassiveExpEnabledAmount <= maximumLargePassiveExpTrails) 
				{
					if (largePassiveExpBehaviour.trailLimitRenableBool == false)
					{		  	 
						TrailRenderer largePassiveExpTrailRend = pooledLargePassiveExpList [i].GetComponent<TrailRenderer> ();
						largePassiveExpTrailRend.time = 0;
						largePassiveExpTrailRend.enabled = true;
								   
						largePassiveExpBehaviour.trailLimitReEnableTimer = 0.15f;
						largePassiveExpBehaviour.trailLimitRenableBool = true;
					}
				}

				if (largePassiveExpEnabledAmount > maximumLargePassiveExp3D) 
				{
					largePassiveExpBehaviour.localRend.enabled = false;
					largePassiveExpBehaviour.exp2DSprite.enabled = true;				
				}
			}
		}

// Mega Exp
		int megaExpEnabledAmount = 0;

		for(int i = 0; i < pooledMegaExpList.Count; i++)
		{
			if (pooledMegaExpList [i].activeInHierarchy) 
			{
				megaExpEnabledAmount += 1;

				if (megaExpEnabledAmount <= maximumMegaExpLights) 
				{
					pooledMegaExpList [i].GetComponent<ExpPickupBehaviour> ().subLightsObject.SetActive (true);
				}

				if (megaExpEnabledAmount <= maximumMegaExpTrails) 
				{
					if (pooledMegaExpList [i].GetComponent<ExpPickupBehaviour> ().trailLimitRenableBool == false)
					{		  
						pooledMegaExpList [i].GetComponent<TrailRenderer> ().time = 0;
						pooledMegaExpList [i].GetComponent<TrailRenderer> ().enabled = true;
							  
						pooledMegaExpList [i].GetComponent<ExpPickupBehaviour> ().trailLimitReEnableTimer = 0.15f;
						pooledMegaExpList [i].GetComponent<ExpPickupBehaviour> ().trailLimitRenableBool = true;
					}
				}
			}
		}
	}
}
