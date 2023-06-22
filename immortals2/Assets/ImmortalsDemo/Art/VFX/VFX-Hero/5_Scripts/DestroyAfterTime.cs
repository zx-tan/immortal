using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple script that will destroy itself or a target object after X amount of time has passed
/// Written by Timothy Bermanseder
/// </summary>

public class DestroyAfterTime : MonoBehaviour {

	[SerializeField]
	[Tooltip("The time before destorying the target object.")]
	float waitTime;

	[SerializeField]
	[Tooltip("Leave empty for the script destroy its own object.")]
	GameObject targetObject;

	IEnumerator Start ()
	{
		if (targetObject == null)
			targetObject = transform.gameObject;

		yield return new WaitForSeconds(waitTime);

		Destroy(targetObject);

		yield return null;
	}
	

}
