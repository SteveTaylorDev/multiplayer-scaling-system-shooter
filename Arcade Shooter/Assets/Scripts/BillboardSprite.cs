using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour 
{
	public GameObject camera1Object;

	// Use this for initialization
	void Start () 
	{
		camera1Object = GameObject.FindWithTag ("Camera1");
	}

	void LateUpdate () 
	{
		transform.forward = camera1Object.transform.forward;
	}
}
