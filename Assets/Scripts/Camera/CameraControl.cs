using System;
using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float Speed = 5;

	//public KeyCode Left = KeyCode.A;
	//public KeyCode Right = KeyCode.D;
	//public KeyCode Up = KeyCode.W;
	//public KeyCode Down = KeyCode.S;
	public KeyCode RotateCameraLeft = KeyCode.Q;
	public KeyCode RotateCameraRight = KeyCode.E;
	public KeyCode ResetCameraRotation = KeyCode.R;
	
	public float RotationX;
	public float MaxHeight = 15;
	public float MinHeight = 5;

	private float _rotationY;
	private float _height;


	private void Start()
	{
		_height = transform.position.y;
		_rotationY = transform.rotation.eulerAngles.y;
		RotationX = transform.rotation.eulerAngles.x;
	}

	void Update ()
	{
		MoveCamera();
		
		if(Input.GetKey(RotateCameraRight)) 
			_rotationY -= 3;
		else if(Input.GetKey(RotateCameraLeft)) 
			_rotationY += 3;

		var oldHeight = _height;
		
		if(Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			if(_height < MaxHeight) 
				_height += 1;
		}
		if(Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if(_height > MinHeight) 
				_height -= 1;
		}

		_height = Mathf.Lerp(oldHeight, _height, 3 * Time.deltaTime);

		transform.position = new Vector3(transform.position.x, _height, transform.position.z);
		transform.rotation = Quaternion.Euler(RotationX, _rotationY, 0);
	}

	private void MoveCamera()
	{
		Vector3 moveVector = Vector3.right * Input.GetAxis("Horizontal") + Vector3.forward * Input.GetAxis("Vertical");
		transform.Translate(moveVector * Speed * Time.deltaTime);
	}
}