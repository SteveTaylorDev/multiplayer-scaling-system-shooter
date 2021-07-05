using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour 
{
	private GameManager gameManager;

	public GameObject player1Hud;
	public GameObject player2Hud;
	public GameObject player3Hud;
	public GameObject player4Hud;

	public Image player1HealthBar;
	public Image player1HealthBarBG;
	public UnityEngine.UI.Text player1ActiveExpText;

	public Image player2HealthBar;
	public Image player2HealthBarBG;
	public UnityEngine.UI.Text player2ActiveExpText;

	public Image player3HealthBar;
	public Image player3HealthBarBG;
	public UnityEngine.UI.Text player3ActiveExpText;

	public Image player4HealthBar;
	public Image player4HealthBarBG;
	public UnityEngine.UI.Text player4ActiveExpText;

	[HideInInspector]public float player1Health;
	[HideInInspector]public float player1MaxHealth;
	[HideInInspector]public float player1ActiveExp;
	private float p1HealthRatio;

	[HideInInspector]public float player2Health;
	[HideInInspector]public float player2MaxHealth;
	[HideInInspector]public float player2ActiveExp;
	private float p2HealthRatio;

	[HideInInspector]public float player3Health;
	[HideInInspector]public float player3MaxHealth;
	[HideInInspector]public float player3ActiveExp;
	private float p3HealthRatio;

	[HideInInspector]public float player4Health;
	[HideInInspector]public float player4MaxHealth;
	[HideInInspector]public float player4ActiveExp;
	private float p4HealthRatio;


	// Use this for initialization
	void Start () 
	{
		GameObject gameManagerObject = GameObject.FindWithTag ("GameManager");
		gameManager = gameManagerObject.GetComponent<GameManager> ();
	}

	void Update () 
	{
		UpdateHealthBar ();
	}

	void UpdateHealthBar()
	{
		if (gameManager.activePlayerAmount <= 1) 
		{
			player2Hud.SetActive (false);
			player3Hud.SetActive (false);
			player4Hud.SetActive (false);
		}

		if (gameManager.activePlayerAmount == 2) 
		{
			player2Hud.SetActive (true);
			player3Hud.SetActive (false);
			player4Hud.SetActive (false);
		}

		if (gameManager.activePlayerAmount == 3) 
		{
			player2Hud.SetActive (true);
			player3Hud.SetActive (true);
			player4Hud.SetActive (false);
		}

		if (gameManager.activePlayerAmount == 4) 
		{
			player2Hud.SetActive (true);
			player3Hud.SetActive (true);
			player4Hud.SetActive (true);
		}

	// Player 1

		player1ActiveExpText.text = "A-Exp: " + player1ActiveExp;

		p1HealthRatio = player1Health / player1MaxHealth;

		if (p1HealthRatio > 1) 
		{
			p1HealthRatio = 1;
		}

		player1HealthBarBG.color = gameManager.players [0].playerColor;
		player1HealthBar.rectTransform.localScale = new Vector3 (p1HealthRatio, 1, 1);

		if (p1HealthRatio > 0.6f) 
		{
			player1HealthBar.color = Color.green;
		}

		if (p1HealthRatio <= 0.6f && p1HealthRatio > 0.4f)
		{
			player1HealthBar.color = Color.yellow;
		}

		if (p1HealthRatio < 0.4f) 
		{
			player1HealthBar.color = Color.red;
		}

	// Player 2

		if (gameManager.activePlayerAmount >= 2) 
		{
			player2ActiveExpText.text = "A-Exp: " + player2ActiveExp;

			p2HealthRatio = player2Health / player2MaxHealth;

			if (p2HealthRatio > 1) 
			{
				p2HealthRatio = 1;
			}

			player2HealthBarBG.color = gameManager.players [1].playerColor;
			player2HealthBar.rectTransform.localScale = new Vector3 (p2HealthRatio, 1, 1);

			if (p2HealthRatio <= 0.6f && p2HealthRatio > 0.4f) 
			{
				player2HealthBar.color = Color.yellow;
			}

			if (p2HealthRatio < 0.4f)
			{
				player2HealthBar.color = Color.red;
			}
		}

	// Player 3

		if (gameManager.activePlayerAmount >= 3) 
		{
			player3ActiveExpText.text = "A-Exp: " + player3ActiveExp;

			p3HealthRatio = player3Health / player3MaxHealth;

			if (p3HealthRatio > 1) 
			{
				p3HealthRatio = 1;
			}

			player3HealthBarBG.color = gameManager.players [2].playerColor;
			player3HealthBar.rectTransform.localScale = new Vector3 (p3HealthRatio, 1, 1);

			if (p3HealthRatio <= 0.6f && p3HealthRatio > 0.4f) 
			{
				player3HealthBar.color = Color.yellow;
			}

			if (p3HealthRatio < 0.4f) 
			{
				player3HealthBar.color = Color.red;
			}
		}

	// Player 4

		if (gameManager.activePlayerAmount >= 4) 
		{
			player4ActiveExpText.text = "A-Exp: " + player4ActiveExp;

			p4HealthRatio = player4Health / player4MaxHealth;

			if (p4HealthRatio > 1) 
			{
				p4HealthRatio = 1;
			}

			player4HealthBarBG.color = gameManager.players [3].playerColor;
			player4HealthBar.rectTransform.localScale = new Vector3 (p4HealthRatio, 1, 1);

			if (p4HealthRatio <= 0.6f && p4HealthRatio > 0.4f) 
			{
				player4HealthBar.color = Color.yellow;
			}

			if (p4HealthRatio < 0.4f) 
			{
				player4HealthBar.color = Color.red;
			}
		}

	}
}
