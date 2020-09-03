using System;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	public float Speed = 12f;
	
	public bool FlipScrollWheel;
	public KeyCode RotateCameraLeft = KeyCode.Q;
	public KeyCode RotateCameraRight = KeyCode.E;
	public KeyCode ResetCameraRotation = KeyCode.R;
	
	public float MaxHeight;
	public float MinHeight;

	public float YAngleAtBottomPosition = 25f;
	public float ZoomSpeed;
	public float RotationSpeed;
	
	private float _initialRotationY;
	private float _updatedRotationY;
	private float _rotationX;
	private float _height;
	private float _zoomLerpTime;
	private Vector2 _zoomDirection;
	private float _cameraLerpPointY;
	private void Start()
	{
		_height = transform.position.y;
		_initialRotationY = transform.rotation.eulerAngles.y;
		_updatedRotationY = transform.rotation.eulerAngles.y;
		_cameraLerpPointY = _height;
		_rotationX = transform.rotation.eulerAngles.x;
	}

	void Update ()
	{
		MoveCameraWithKeyboard();
		ZoomCamera();
		_zoomLerpTime += Time.deltaTime * ZoomSpeed;

		ChangeCameraAngle();
		
		if(Input.GetKey(RotateCameraRight)) 
			_updatedRotationY += RotationSpeed * Time.deltaTime;
		else if(Input.GetKey(RotateCameraLeft)) 
			_updatedRotationY -= RotationSpeed * Time.deltaTime;
		if (Input.GetKeyDown(ResetCameraRotation))
			_updatedRotationY = _initialRotationY;
		
		transform.position = new Vector3(transform.position.x, Mathf.Lerp(_height, _cameraLerpPointY, _zoomLerpTime), transform.position.z);
		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, _updatedRotationY, 0);
	}

	private void MoveCameraWithKeyboard()
	{
		Vector3 inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		Vector3 moveVector = inputVector.magnitude > 1 ? inputVector.normalized : inputVector;
		transform.Translate(moveVector * Speed * Time.deltaTime);
	}

	private void ZoomCamera()
	{
		int dir = FlipScrollWheel ? -1 : 1;
		if(Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			MoveCameraVertically(Vector2.up * dir);
		}
		else if(Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			MoveCameraVertically(Vector2.down * dir);
		}
	}

	private void MoveCameraVertically(Vector2 dir)
	{
		if (_zoomDirection != dir || _zoomLerpTime >= 1 || _zoomLerpTime <= 0)
		{
			_height = transform.position.y;
			_zoomLerpTime = 0;
		}
			
		MoveCameraLerpPointY(dir);
		_zoomDirection = dir;
	}

	private void MoveCameraLerpPointY(Vector2 dir)
	{
		if (dir == Vector2.up)
			_cameraLerpPointY += _cameraLerpPointY * Mathf.Abs(Input.GetAxis("Mouse ScrollWheel"));
		else if (dir == Vector2.down)
			_cameraLerpPointY -= _cameraLerpPointY * Mathf.Abs(Input.GetAxis("Mouse ScrollWheel"));

		_cameraLerpPointY = Mathf.Clamp(_cameraLerpPointY, MinHeight, MaxHeight);
	}
	private void ChangeCameraAngle()
	{
		transform.rotation = Quaternion.Euler(Mathf.Lerp(YAngleAtBottomPosition, _rotationX, (transform.position.y - MinHeight) / (MaxHeight - MinHeight)), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
	}
}