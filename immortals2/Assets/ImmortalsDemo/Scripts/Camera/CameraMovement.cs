using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour, ICameraMovement
{
	[SerializeField]
	private float speed = 2.0f;
	[SerializeField]
	private float zoomSpeed = 2.0f;

	// rotation related members
	public float minX = -360.0f;
	public float maxX = 360.0f;
	
	public float minY = -45.0f;
	public float maxY = 45.0f;

	public float sensX = 100.0f;
	public float sensY = 100.0f;
	
	private float rotationY = 0.0f;
	private float rotationX = 0.0f;

	private float yaw = 0.0f;
	private float pitch = 0.0f;

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

	void Rotate(float x, float y)
	{
		rotationX += x * sensX * Time.deltaTime;
		rotationY += y * sensY * Time.deltaTime;
		rotationY = Mathf.Clamp (rotationY, minY, maxY);
		transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
	}

	void Zoom(float factor)
	{
		transform.Translate(0, factor * zoomSpeed, factor * zoomSpeed, Space.World);
	}

	void Update () 
	{
		Zoom(Input.GetAxis("Mouse ScrollWheel"));

		if (Input.GetKey(KeyCode.RightArrow))
			MoveRight();
		if (Input.GetKey(KeyCode.LeftArrow))
			MoveLeft();
		if (Input.GetKey(KeyCode.UpArrow))
			MoveForward();
		if (Input.GetKey(KeyCode.DownArrow))
			MoveBack();

		if (Input.GetMouseButton (0))
			Rotate(Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"));
	}
}
