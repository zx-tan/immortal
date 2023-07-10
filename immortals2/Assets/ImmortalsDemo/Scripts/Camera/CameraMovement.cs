using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour, ICameraMovement {

	void MoveLeft()
	{
		transform.position += Vector3.left * speed * Time.deltaTime;
	}
	
	void MoveRight()
	{
		transform.position += Vector3.right * speed * Time.deltaTime;
	}

	void MoveForward()
	{
		transform.position += Vector3.forward * speed * Time.deltaTime;
	}

	void MoveBack()
	{
		transform.position += Vector3.back * speed * Time.deltaTime;
	}

	void Update () {

	}
}
