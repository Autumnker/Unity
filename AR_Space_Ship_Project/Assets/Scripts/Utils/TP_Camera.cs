using UnityEngine;
using System.Collections;

public struct ClipPlanePoints
{
	public Vector3 UpperLeft;
	public Vector3 UpperRight;
	public Vector3 LowerLeft;
	public Vector3 LowerRight;
}

public enum ChangeTargetTypeEnum:byte
{
	None = 0,
	CameraGotoTargetPosition = 1,  //摄像机看向的目标移动到需求的目标点的位置和方向，然后如果目标移动，不会影响到摄像机.
	ChangeTargetImmediately = 2,    //立刻切换摄像机的目标，没有动画，移动目标摄像机也会跟随移动.
	ToBeChildrenOfTarget = 3,   //摄像机看向的目标做为新目标的子物体，有一秒的动画移动到目标点,移动目标摄像机也会跟随移动.
    ChangeTargetImmediatelyWithITween = 4,
}

public class TP_Camera : MonoBehaviour
{
	public Transform TargetLookAt;
	public float Distance = 5f;	
	public float DistanceMin = 3f;	
	public float DistanceMax = 10f;		
	private float mouseX = 0f;	
	private float mouseY = 0f;	
	private float startDistance = 0f;	
	private float desiredDistance = 0f;

	public float X_MouseSensitivity = 5f;	
	public float Y_MouseSensitivity = 5f;	
	public float MouseWheelSensitivity = 5f;	
	public float Y_MinLimit = -40f;	
	public float Y_MaxLimit = 80f;

	public float DistanceSmooth = 0.05f;
	private float velDistance = 0f;
	private Vector3 desiredPosition = Vector3.zero;

	//初始时镜头的x和y方向的旋转度.
	public float init_mouseX = -547f;	
	public float init_mouseY = 0f;	
	public bool needIgnoreOccluding = false;
	public Transform defaultParent;

	void Start()
	{
		_cameraTransform = transform;
		_groundLayerMask = LayerMask.NameToLayer("TPCameraOcclusion");
		_groundLayerMask = 1 << _groundLayerMask;
//		Distance = Mathf.Clamp(Distance,DistanceMin,DistanceMax);
		startDistance = Distance;		
		Reset();
		if(CameraController.Instance!=null)
		{
			CameraController.Instance.tpCameraScript = this;
			CameraController.Instance.SetMainCamera(_cameraTransform);
		}
	}

	public void SetStartDistance(float value)
	{
		startDistance = value;
	}

	public void SetDesiredDistance(float value)
	{
		desiredDistance = value;
		preOccludedDistance = DistanceMin;
	}

	public void SetMouseXAndY(float valueX, float valueY)
	{
		mouseX = valueX;
		mouseY = valueY;
	}

	public void Reset()
	{
		mouseX = init_mouseX;	
		mouseY = init_mouseY;	
		Distance = startDistance;	
		desiredDistance = Distance;
		preOccludedDistance = Distance;
	}

	void LateUpdate()
	{	
		if(TargetLookAt == null)	
			return;		
		HandlePlayerInput();

		//忽略碰撞检测.
		if(needIgnoreOccluding)
		{
			CalculateDesiredPosition();
		}
		else
		{
			int count = 0;
			do
			{
				CalculateDesiredPosition();
				count++;
			}while(CheckIfOccluded(count));
		}
//		Debug.Log("mouseX is: "+mouseX+"    mouseY is: "+mouseY+"   Distance is: "+Distance);
//		Debug.Log("preOccludedDistance is: "+preOccludedDistance+"    desiredDistance is: "+desiredDistance);

		//画线，可注释掉.
//		checkCameraPoints(TargetLookAt.position,desiredPosition);
		UpdatePosition();	
	}

	//玩家在x方向的旋转输入.
	public float xDelta = 0;
	public float yDelta = 0;
	//玩家的缩放输入.
	public float pinchDelta = 0;
	private float deadZone = 0.1f;
	void HandlePlayerInput()
	{
		//1代表鼠标右键
//		if(Input.GetMouseButton(1))
//		{
//			mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;			
//			mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;	
//		}	
		if(xDelta!=0)
		{
			mouseX += xDelta * X_MouseSensitivity;	
			xDelta = 0;
		}
		if(yDelta!=0)
		{
			mouseY -= yDelta * Y_MouseSensitivity;
			yDelta = 0;
		}

		// 限制mouseY	.
		mouseY = ClampAngle(mouseY,Y_MinLimit,Y_MaxLimit);
//		if(Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
//		{
//			desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity, DistanceMin , DistanceMax);	
//			preOccludedDistance = desiredDistance;
//			distanceSmooth = DistanceSmooth;
//		}

		if(Mathf.Abs(pinchDelta)>deadZone)
		{
			desiredDistance = Mathf.Clamp(Distance - pinchDelta * MouseWheelSensitivity, DistanceMin , DistanceMax);	
			preOccludedDistance = desiredDistance;
			distanceSmooth = DistanceSmooth;
			pinchDelta = 0;
		}
	}

	float ClampAngle(float angle, float min, float max)
	{	
		do
		{
			if(angle < -360)
				angle += 360;
			if(angle > 360)	
				angle -= 360;	
		}while(angle < -360 || angle > 360);

		return Mathf.Clamp(angle,min,max);
	}

	void CalculateDesiredPosition()
	{
		ResetDesiredDistance();
		//velDistance用于存储平滑移动中每一个点的距离，DistanceSmooth用于存储平滑移动的时间.
		Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, distanceSmooth);
		desiredPosition = CalculatePosition(mouseY,mouseX,Distance);
	}

	Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
	{
		Vector3 direction = new Vector3(0,0,-distance);
		Quaternion rotation = Quaternion.Euler(rotationX,rotationY,0);
		//方向乘以旋转角度就等于偏移量.
		return TargetLookAt.position + rotation * direction;
	}

	void ResetDesiredDistance()
	{
		if(desiredDistance<preOccludedDistance)
		{
			var pos = CalculatePosition(mouseY,mouseX,preOccludedDistance);
			var nearestDistance = checkCameraPoints(TargetLookAt.position,pos);
			if(nearestDistance == -1 || nearestDistance > preOccludedDistance)
			{
				desiredDistance = preOccludedDistance;
			}
		}
	}


	public float X_Smooth = 0.05f;	
	public float Y_Smooth = 0.1f;
	private float velX = 0f;	
	private float velY = 0f;	
	private float velZ = 0f;	
	private Vector3 position = Vector3.zero;
	private Transform _cameraTransform = null;

	void UpdatePosition()
	{
		var posX = Mathf.SmoothDamp(position.x,desiredPosition.x, ref velX, X_Smooth);	
		var posY = Mathf.SmoothDamp(position.y,desiredPosition.y, ref velY, Y_Smooth);	
		var posZ = Mathf.SmoothDamp(position.z,desiredPosition.z, ref velZ, X_Smooth);	
		position = new Vector3(posX,posY,posZ);
		_cameraTransform.position = position;	
		_cameraTransform.LookAt(TargetLookAt);	
	}

	public ClipPlanePoints ClipPlaneAtNear (Vector3 pos)
	{
		//参数pos表示摄像机的坐标.
		var clipPlanePoints = new ClipPlanePoints();
		if(Camera.main == null)
			return clipPlanePoints;
		if(_cameraTransform == null)
			_cameraTransform = transform;

		var halfFOV = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;
		//摄像机的FOV是以角度为单位的，但是正切函数.
		//只能接受弧度作为参数，故需要进行转换.
		var aspect = Camera.main.aspect;
		// aspect是摄像机屏幕的宽高比，如4:3，16:9等.
		var distance = Camera.main.nearClipPlane;
		//摄像机的nearClipPlane参数就是摄像机离近点平面的距离.
		var height = distance * Mathf.Tan(halfFOV);
		var width = height * aspect;
		//先将摄像机的位置坐标pos向右平移width的距离.
		clipPlanePoints.LowerRight = pos + _cameraTransform.right * width;
		//再将该点向下平移height的距离.
		clipPlanePoints.LowerRight -= _cameraTransform.up * height;
		//之所以向下平移时，用transform.up，是因为transform对象没有down这个成员变量.
		//再将该点向摄像机的Z方向平移distance的距离.
		//这样，就获得了近点平面上右下角的点的坐标.
		clipPlanePoints.LowerRight += _cameraTransform. forward * distance;
		//剩下的3个点的坐标的计算方法类似.
		clipPlanePoints.LowerLeft = pos - _cameraTransform. right * width;
		clipPlanePoints.LowerLeft -= _cameraTransform.up * height;
		clipPlanePoints.LowerLeft += _cameraTransform.forward * distance;
		clipPlanePoints.UpperRight = pos + _cameraTransform. right * width;
		clipPlanePoints.UpperRight += _cameraTransform.up * height;
		clipPlanePoints.UpperRight += _cameraTransform. forward * distance;
		clipPlanePoints.UpperLeft = pos - _cameraTransform.right * width;
		clipPlanePoints.UpperLeft += _cameraTransform.up * height;
		clipPlanePoints.UpperLeft += _cameraTransform.forward * distance;
		return clipPlanePoints;
	}

	//from参数表示摄像机看向的目标的坐标，to参数表示摄像机的坐标.
	float checkCameraPoints(Vector3 from,Vector3 to)
	{
		var nearestDistance = -1f;
		RaycastHit hitInfo;
		ClipPlanePoints clipPlanePoints = ClipPlaneAtNear(to);
		//draw lines in the editor to make it easier to visualize
//		Debug.DrawLine(from,to + _cameraTransform.forward * -Camera.main.nearClipPlane,Color.red);
//		Debug.DrawLine(from,clipPlanePoints.UpperLeft);
//		Debug.DrawLine(from,clipPlanePoints.UpperRight);
//		Debug.DrawLine(from,clipPlanePoints.LowerLeft);
//		Debug.DrawLine(from,clipPlanePoints.LowerRight);
//		Debug.DrawLine(clipPlanePoints.UpperLeft,clipPlanePoints.UpperRight);
//		Debug.DrawLine(clipPlanePoints.UpperRight,clipPlanePoints.LowerRight);
//		Debug.DrawLine(clipPlanePoints.LowerRight,clipPlanePoints.LowerLeft);
//		Debug.DrawLine(clipPlanePoints.LowerLeft,clipPlanePoints.UpperLeft);


		if(Physics.Linecast(from,clipPlanePoints.UpperLeft,out hitInfo,_groundLayerMask)) //&& hitInfo.collider.tag != "Player")
			nearestDistance = hitInfo.distance;
		if(Physics.Linecast(from,clipPlanePoints.LowerLeft,out hitInfo,_groundLayerMask))// && hitInfo.collider.tag != "Player")
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		if(Physics.Linecast(from,clipPlanePoints.UpperRight,out hitInfo,_groundLayerMask))// && hitInfo.collider.tag != "Player")
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		if(Physics.Linecast(from,clipPlanePoints.LowerRight,out hitInfo,_groundLayerMask))// && hitInfo.collider.tag != "Player")
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		if(Physics.Linecast(from,to + _cameraTransform.forward * -Camera.main.nearClipPlane,out hitInfo,_groundLayerMask))// && hitInfo.collider.tag != "Player")
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;


		return nearestDistance;
	}

	//两个变量中，前者用来保存每次摄像机前移的步长，后者保存最大前移次数.
	public float OcclusionDistanceStep = 5f;
	public int MaxOcclusionChecks = 20;
	private int _groundLayerMask;

	bool CheckIfOccluded(int count)
	{
		var isOccluded = false;
		var nearestDistance = checkCameraPoints(TargetLookAt.position,desiredPosition);

		//摄像机和目标之间有障碍物.
		if(nearestDistance != -1)
		{
			if(count<MaxOcclusionChecks)
			{
				isOccluded = true;
				Distance -= OcclusionDistanceStep;
				if(Distance<0.25f)
					Distance = 0.25f;
			}
			else
			{
				Distance = nearestDistance -	Camera.main.nearClipPlane; 	
			}
			desiredDistance = Distance;
			distanceSmooth = DistanceResumeSmooth;

		}
		return isOccluded;
	}

	//distanceSmooth变量用于保存摄像机的移动平滑指数，它和DistanceSmooth变量的区别在于，DistanceSmooth变量用于保存在用户滚动鼠标滚轮时，摄像机的移动平滑指数.
	//而DistanceResumeSmooth变量则用于保存当摄像机回位的时候的移动平滑指数.
	public float DistanceResumeSmooth = 1f;
	private float distanceSmooth = 0f;
	private float preOccludedDistance = 0f;

	private float _inertialDeltaX = 0;
	private float _inertialDeltaY = 0;
	//玩家输入结束后，开启惯性模式，让镜头继续旋转一段时间.
	public void StartInertialAction(float inertialDeltaX, float inertialDeltaY)
	{
		_inertialDeltaX = inertialDeltaX;
		_inertialDeltaY = inertialDeltaY;
		StopCoroutine("InertialCoroutine");
		StartCoroutine("InertialCoroutine");
	}

	public void StopInertialAction()
	{
		StopCoroutine("InertialCoroutine");
		_inertialDeltaX = 0;
		_inertialDeltaY = 0;
		xDelta = 0;
		yDelta = 0;
	}

	IEnumerator InertialCoroutine()
	{
		yield return null;
		int loopTimes = 10;
		float decentDelta = 1f/loopTimes;
		while(loopTimes>0)
		{
			_inertialDeltaX -= _inertialDeltaX*decentDelta;
			_inertialDeltaY -= _inertialDeltaY*decentDelta;
			xDelta = _inertialDeltaX;
			yDelta = _inertialDeltaY;
			loopTimes --;
			yield return null;
		}
	}
    private Transform mTarget = null;
	public void ChangeTarget(Transform newTar, ChangeTargetTypeEnum changeType = ChangeTargetTypeEnum.CameraGotoTargetPosition)
	{
        switch (changeType)
        {
            case ChangeTargetTypeEnum.CameraGotoTargetPosition:
                TargetLookAt.parent = defaultParent;
                TargetLookAt.position = newTar.position;
                //			iTween.Stop(gameObject);
                //			iTween.MoveTo(TargetLookAt.gameObject, iTween.Hash("position",newTar.position,"islocal", false, "time", 1f, "easeType", iTween.EaseType.linear,"oncomplete", "MoveOver", "oncompletetarget", gameObject));
                //			iTween.RotateTo(TargetLookAt.gameObject, iTween.Hash("rotation", newTar.eulerAngles, "islocal", false, "time", 1f, "easeType", iTween.EaseType.linear));
                break;
            case ChangeTargetTypeEnum.ChangeTargetImmediately:
                //			TargetLookAt = newTar;
                //			TargetLookAt.parent = newTar;
                //			TargetLookAt.localPosition = Vector3.zero;
                TargetLookAt.parent = newTar;
                TargetLookAt.position = newTar.position;
                break;
            case ChangeTargetTypeEnum.ToBeChildrenOfTarget:
                TargetLookAt.parent = newTar;
                //			iTween.Stop(gameObject);
                //			iTween.MoveTo(TargetLookAt.gameObject, iTween.Hash("position",Vector3.zero,"islocal", true, "time", 1f, "easeType", iTween.EaseType.linear,"oncomplete", "MoveOver", "oncompletetarget", gameObject));
                //			iTween.RotateTo(TargetLookAt.gameObject, iTween.Hash("rotation", Vector3.zero, "islocal", true, "time", 1f, "easeType", iTween.EaseType.linear));
                break;
            case ChangeTargetTypeEnum.ChangeTargetImmediatelyWithITween:
                {
                    iTween.Stop(gameObject);
                    iTween.MoveTo(TargetLookAt.gameObject, iTween.Hash("position",newTar.position,"islocal", false, "time", 0.5f, "easeType", iTween.EaseType.linear,"oncomplete", "MoveOver", "oncompleteparams", true ,"oncompletetarget", gameObject));
                    iTween.RotateTo(TargetLookAt.gameObject, iTween.Hash("rotation", newTar.eulerAngles, "islocal", false, "time", 0.5f, "easeType", iTween.EaseType.linear));
                    mTarget = newTar;
                }
                break;
        }
	}

    public void ChangeTarget(Vector3 pos, ChangeTargetTypeEnum changeType = ChangeTargetTypeEnum.CameraGotoTargetPosition)
    {
        switch (changeType)
        {
            case ChangeTargetTypeEnum.CameraGotoTargetPosition:
                TargetLookAt.parent = defaultParent;
                TargetLookAt.position = pos;
                //			iTween.Stop(gameObject);
                //			iTween.MoveTo(TargetLookAt.gameObject, iTween.Hash("position",newTar.position,"islocal", false, "time", 1f, "easeType", iTween.EaseType.linear,"oncomplete", "MoveOver", "oncompletetarget", gameObject));
                //			iTween.RotateTo(TargetLookAt.gameObject, iTween.Hash("rotation", newTar.eulerAngles, "islocal", false, "time", 1f, "easeType", iTween.EaseType.linear));
                break;
        }
    }

	void MoveOver(bool changeTarget)
	{
        if (!changeTarget || mTarget == null) return;

        TargetLookAt.position = mTarget.position;
        TargetLookAt.parent = mTarget;
	}

	/// <summary>
	/// 重置相机的关键参数.
	/// </summary>
	public void ResetToNewParamSet(CameraParamStruct cps)
	{
		mouseX = cps.newMouseX;	
		mouseY = cps.newMouseY;	
		Distance = cps.newStartDistance;	
		desiredDistance = cps.newDistance;
		preOccludedDistance = cps.newDistance;
		DistanceMin = cps.DistanceMin;
		DistanceMax = cps.DistanceMax;

        Y_MinLimit = cps.YLimitMin;
        Y_MaxLimit = cps.YLimitMax;

        X_Smooth = cps.XSMooth;
        Y_Smooth = cps.YSMooth;

        DistanceSmooth = cps.DistanceSmooth;
	}

	public float GetMouseX()
	{
		return mouseX;
	}

	public float GetMouseY()
	{
		return mouseY;
	}
}
