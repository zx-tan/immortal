using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// A simple script which causes one transform to match the position of another.

public class MatchPosition : MonoBehaviour
{

	public Transform transformToMatch;
	public float matchSpeed = 10;

	public bool matchParent = true;
	// Use this for initialization
	IEnumerator Start ()
	{

		if (matchParent)
		{
			transformToMatch = transform.parent;
			transform.parent = null;

		}
		while(true) // Cheap update
		{
			transform.position = Vector3.Lerp(transform.position, transformToMatch.position, Time.deltaTime * matchSpeed);

			yield return null;
		}
	}
	
}
