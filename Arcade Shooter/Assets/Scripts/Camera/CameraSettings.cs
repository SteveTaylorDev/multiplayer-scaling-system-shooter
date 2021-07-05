using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettings : MonoBehaviour {

	static public Vector3 firstCameraPos = new Vector3 (0, 16, 5);
	static public Vector3 secondCameraPos = new Vector3 (0, 16, 5);
	[HideInInspector]public Quaternion cameraRotation;

	public float targetAspectWidth;					// Usually 10 for shooter play area
	public float targetAspectHeight;					// Usually 16 for shooter play area
	public bool multiplayerCamera;
	public int cameraNumber;

	public GameObject bgCameraObject;
	public bool bgCamera;
	public float rotateSpeed;
	private Vector3 rotateVector;

	private float cameraOffset;

	private CameraSettings bgCameraSettings;


	// Use this for initialization
	void Start () 
	{
		if (bgCameraObject != null) 
		{
			bgCameraSettings = bgCameraObject.GetComponent<CameraSettings> ();
		}
	}

	void Update()
	{
		MainSetup ();
		BackgroundRotate ();
		UpdateBackground ();
	}

	void MainSetup()
	{
		// Get the camera component
		Camera camera = GetComponent<Camera>();

		// Set the default camera rotation
		cameraRotation = Quaternion.LookRotation(new Vector3 (0,-90,0));

		// Check if multiplayer is enabled
		if (multiplayerCamera == true) 
		{
			if (cameraNumber == 1) 
			{
				transform.position = firstCameraPos;
				transform.rotation = cameraRotation;
				cameraOffset = -0.25f;
			}

			if (cameraNumber == 2) 
			{
				transform.position = secondCameraPos;
				transform.rotation = cameraRotation;
				cameraOffset = 0.25f;
			}
		} 

		// Else set the camera to single player mode (center screen)
		else 
		{
			transform.position = firstCameraPos;
			transform.rotation = cameraRotation;
			cameraOffset = 0;
		}

		// set the desired aspect ratio
		float targetaspect = targetAspectWidth / targetAspectHeight;

		// determine the game window's current aspect ratio
		float windowaspect = (float)Screen.width / (float)Screen.height;

		// current viewport height should be scaled by this amount
		float scaleheight = windowaspect / targetaspect;

		// if scaled height is less than current height, add letterbox
		if (scaleheight < 1.0f)
		{  
			Rect rect = camera.rect;

			rect.width = 1.0f;
			rect.height = scaleheight;
			rect.x = 0;
			rect.y = (1.0f - scaleheight) / 2.0f;

			camera.rect = rect;
		}
		else // add pillarbox
		{
			float scalewidth = 1.0f / scaleheight;

			Rect rect = camera.rect;

			rect.width = scalewidth;
			rect.height = 1.0f;
			rect.x = (1.0f - scalewidth) / 2.0f + cameraOffset;				// offsets the play area depending on the camera number
			rect.y = 0;

			camera.rect = rect;
		}
	}

	void BackgroundRotate()
	{
		if (bgCamera == true) 
		{
			rotateVector += new Vector3 (-rotateSpeed, 0, 0) * Time.deltaTime; 
			transform.rotation = Quaternion.Euler (rotateVector);
		}
	}

	void UpdateBackground()
	{
		if (bgCameraObject == null) 
		{
			return;
		} 

		else 
		{
			bgCameraSettings.cameraNumber = cameraNumber;
			bgCameraSettings.multiplayerCamera = multiplayerCamera;
		}
	}
}
