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

		float scroll = Input.GetAxis("Mouse ScrollWheel");
		transform.Translate(0, scroll * zoomSpeed, scroll * zoomSpeed, Space.World);

		if (Input.GetKey(KeyCode.RightArrow)){
			MoveRight();
		}
		if (Input.GetKey(KeyCode.LeftArrow)){
			MoveLeft();
		}
		if (Input.GetKey(KeyCode.UpArrow)){
			MoveForward();
		}
		if (Input.GetKey(KeyCode.DownArrow)){
			MoveBack();
		}
	}
}
