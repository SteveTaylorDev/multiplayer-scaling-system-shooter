using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWithBoundary : MonoBehaviour 
{

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 8) 
		{
			other.gameObject.SetActive (false);
		} 

		else if (other.gameObject.layer == 11) 
		{
			ExpPickupBehaviour expBehaviour = other.gameObject.GetComponent<ExpPickupBehaviour> ();
			expBehaviour.startBoundaryDestroyCounter = true;
		} 

		else 
		{
			Destroy (other.gameObject);
		}


	}

	void OnCollisionExit(Collision other)
	{
		if (other.gameObject.layer != 11) 
		{
			Destroy (other.gameObject);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.layer == 11) 
		{
			ExpPickupBehaviour expBehaviour = other.gameObject.GetComponent<ExpPickupBehaviour> ();
			expBehaviour.boundaryDestroyCounter = 2;
			expBehaviour.startBoundaryDestroyCounter = false;
		}
	}
}
