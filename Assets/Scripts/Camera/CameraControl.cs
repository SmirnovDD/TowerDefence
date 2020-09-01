using System;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	public float Speed = 12f;

	//public KeyCode Left = KeyCode.A;
	//public KeyCode Right = KeyCode.D;
	//public KeyCode Up = KeyCode.W;
	//public KeyCode Down = KeyCode.S;
	public KeyCode RotateCameraLeft = KeyCode.Q;
	public KeyCode RotateCameraRight = KeyCode.E;
	public KeyCode ResetCameraRotation = KeyCode.R;
	
	public float RotationX;
	public float MaxHeight;
	public float MinHeight;

	public float YAngleAtBottomPosition = 25f;
	public float ZoomSpeed;
	
	private float _initialRotationY;
	private float _height;
	private float _zoomLerpTime;
	private Vector2 _zoomDirection;
	
	private void Start()
	{
		_height = transform.position.y;
		_zoomLerpTime = (_height - MinHeight) / (MaxHeight - MinHeight);
		_initialRotationY = transform.rotation.eulerAngles.y;
		RotationX = transform.rotation.eulerAngles.x;
	}

	void Update ()
	{
		MoveCameraWithKeyboard();
		ZoomCamera();
		ChangeCameraAngle();
		if(Input.GetKey(RotateCameraRight)) 
			_initialRotationY -= 3;
		else if(Input.GetKey(RotateCameraLeft)) 
			_initialRotationY += 3;

		transform.position = new Vector3(transform.position.x, _height, transform.position.z);
		transform.rotation = Quaternion.Euler(RotationX, _initialRotationY, 0);
	}

	private void MoveCameraWithKeyboard()
	{
		Vector3 moveVector = Vector3.right * Input.GetAxis("Horizontal") + Vector3.forward * Input.GetAxis("Vertical");
		transform.Translate(moveVector * Speed * Time.deltaTime);
	}

	private void ZoomCamera()
	{
		
		if(Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			MoveCameraVertically(Vector2.up);
		}
		else if(Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			MoveCameraVertically(Vector2.down);
		}
		
		_height = Mathf.SmoothStep(MinHeight, MaxHeight, _zoomLerpTime);
	}

	private void MoveCameraVertically(Vector2 dir)
	{
		if(_zoomDirection != dir || _zoomLerpTime >= 1)
			_zoomLerpTime = (_height - MinHeight) / (MaxHeight - MinHeight);
			
		_zoomLerpTime += Time.deltaTime * Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
		_zoomDirection = dir;
	}
	private void ChangeCameraAngle()
	{
		//Debug.Log(_height/(MaxHeight-MinHeight));
		//	RotationX = Mathf.Lerp(RotationX, YAngleAtBottomPosition, _initialRotationY, (MaxHeight  / MinHeight))
	}
}