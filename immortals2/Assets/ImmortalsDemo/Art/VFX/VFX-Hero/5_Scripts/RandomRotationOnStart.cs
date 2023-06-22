using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple and small script that will randomly rotate on start.
/// Written by Timothy Bermanseder.
/// </summary>

public class RandomRotationOnStart : MonoBehaviour {

	public float rotateAmount = 360;
	public bool rotX, rotY, rotZ;

	float rotXAmount, rotYAmount, rotZAmount;

	// Use this for initialization
	void Start () {

		if (rotX)
			rotXAmount = Random.Range(0, rotateAmount);
		if (rotY)
			rotYAmount = Random.Range(0, rotateAmount);
		if (rotZ)
			rotZAmount = Random.Range(0, rotateAmount);

		transform.Rotate(rotXAmount, rotYAmount, rotZAmount);

	}
	
}
