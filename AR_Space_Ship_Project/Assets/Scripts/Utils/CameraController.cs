using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;

public enum CameraType:byte
{
	None = 0,
	ThirdPerson = 1,
	FirstPerson = 2,
}

public class CameraParamStruct
{
	//摄像机围绕物体旋转时的欧拉角度Y.
	public float newMouseX = 0;
	//摄像机围绕物体旋转时的欧拉角度X.
	public float newMouseY = 0;
	//摄像机距离目标的初始距离.
	public float newStartDistance = 0;
	//摄像机距离目标的最终距离.
	public float newDistance = 0;
	//摄像机距离目标的最短距离.
	public float DistanceMin = 1f;	
	//摄像机距离目标的最长距离.
	public float DistanceMax = 6f;

    //摄像机Y方向上转动最低限制
    public float YLimitMin = 0f;
    //摄像机Y方向上转动最大限制
    public float YLimitMax = 80f;

    //摄像机X方向平滑值
    public float XSMooth = 0.01f;
    //摄像机Y方向平滑值
    public float YSMooth = 0.02f;

    public float DistanceSmooth = 0.4f;

	public CameraParamStruct(float mouseX,float mouseY,float startDis, float dis,float disMin,float disMax,float yLimitMin = 0f,float yLimitMax = 80f,
        float xSmooth = 0.01f,float ySmooth = 0.02f, float distanceSmooth = 0.4f)
	{
		newMouseX = mouseX;
		newMouseY = mouseY;
		newStartDistance = startDis;
		newDistance = dis;
		DistanceMin = disMin;
		DistanceMax = disMax;

        YLimitMin = yLimitMin;
        YLimitMax = yLimitMax;

        XSMooth = xSmooth;
        YSMooth = ySmooth;

        DistanceSmooth = distanceSmooth;
	}
}

public class CameraController : MonoBehaviour
{
	private static CameraController _instance = null;
	public static CameraController Instance
	{
		get
		{
			return _instance;
		}
	}

	private int _maskForARCameraIgnore;
	public CameraParamStruct defaultParam;	// 默认相机参数
	public CameraParamStruct curParam;		// 当前相机参数

	void Awake()
	{
		_instance = this;
		//_maskForARCameraIgnore = LayerMask.NameToLayer(LayerMaskEnum.ARCameraIgnore.ToString());
		_maskForARCameraIgnore = LayerMask.NameToLayer("Water");
		_maskForARCameraIgnore = 1 << _maskForARCameraIgnore;
	}

	void Start()
	{
		// Camera uiCamera = UiController.Instance.d2Camera;
		// if(uiCamera != null)
		// {
		// 	//启动NGUI模式
		// 	EasyTouch.instance.enabledNGuiMode = true;
		// 	//把NGUI摄像机加入过滤队列
		// 	if(!EasyTouch.instance.nGUICameras.Contains(uiCamera))
		// 	{
		// 		EasyTouch.instance.nGUICameras.Add(uiCamera);
		// 	}
		// 	//设置需要过滤的UI层
		// 	EasyTouch.instance.nGUILayers = (1 << LayerMask.NameToLayer("UI"));
		// }
		currentCameraType = CameraType.ThirdPerson;
		defaultParam = new CameraParamStruct(-36, 10, 1.2f, 1.2f, 1.2f, 5, -40, 80, 0.01f, 0.01f, 0.05f);
		curParam = new CameraParamStruct(-36, 10, 1.2f, 1.2f, 1.2f, 5, -40, 80, 0.01f, 0.01f, 0.05f);
    }


	/*******************************   以下为第三人称摄像机部分代码   ********************************/
	public TP_Camera tpCameraScript;
	//锁定无人机镜头旋转和缩放功能.
	public bool isRotateAndScaleScreenLock = false;

	private Transform _camTransform = null;
	public Transform cameraRotateTarget;
	private float _camDragSpeed = 4f;
	private bool _isNowSwiping = false;
	private const float _thresholdValue = 0.05f;
	private float _lastDeltaX = 0;
	private float _lastDeltaY = 0;
	private const float _minDeltaX = -30;
	private const float _maxDeltaX = 30;

	public Camera fpCamera;

	public void SetMainCamera(Transform cameraTrans)
	{
		_camTransform = cameraTrans;
	}

	public CameraType currentCameraType = CameraType.None;

	void HandleOn_Swipe (Gesture gesture)
	{
		//判断是否锁定镜头操作，或者点击屏幕是否点在UI层上.
		if(isRotateAndScaleScreenLock)// || UICamera.hoveredObject)
		{
			return;
		} 

		if(gesture.touchCount>1)
		{
			return;
		}

		if(gesture.actionTime>_thresholdValue)
		{
			_isNowSwiping = true;
			_lastDeltaX = gesture.deltaPosition.x;
			_lastDeltaY = gesture.deltaPosition.y;
			//限制玩家滑动的速度，不可太快.
			_lastDeltaX = Mathf.Clamp(_lastDeltaX,_minDeltaX,_maxDeltaX);
			_lastDeltaY = Mathf.Clamp(_lastDeltaY,_minDeltaX,_maxDeltaX);

			if(currentCameraType == CameraType.ThirdPerson)
			{
				if(tpCameraScript!=null)
				{
					tpCameraScript.xDelta = _lastDeltaX;
					tpCameraScript.yDelta = _lastDeltaY;
				}
			}
			else
			{
				RotateBySelf(fpCamera,_lastDeltaX,-_lastDeltaY, -55f,55f);
			}
		}
	}

	//摄像机围绕自身旋转,eulerXLimit表示摄像机旋转的欧拉角度中的x的上下限.
	void RotateBySelf(Camera cam, float deltaX, float deltaY, float minEulerX, float maxEulerY)
	{
		float speed = 0.6f;
		deltaX *= speed;
		deltaY *= speed;
		float eulerX = 0;
		if(cam.transform.localEulerAngles.x>90)
		{
			//Unity会自动将负数的欧拉角度转换为正数的反向角度，如-30度转换为330度，这里为了处理方便，取消这种转换.
			eulerX = cam.transform.localEulerAngles.x-360+deltaY;
		}
		else
		{
			eulerX = cam.transform.localEulerAngles.x+deltaY;
		}

		eulerX = Mathf.Clamp(eulerX,minEulerX,maxEulerY);
		cam.transform.localEulerAngles = new Vector3(eulerX, 
			cam.transform.localEulerAngles.y+deltaX, 
			cam.transform.localEulerAngles.z);
	}

	void ResetSwipingStatus()
	{
		_isNowSwiping = false;
	}

	private void RotateHorizon(float deltaX)
	{
		_camTransform.RotateAround(cameraRotateTarget.position, Vector3.up, deltaX * _deltaSpeed);
	}

	private void RotateVerticle(float deltaY)
	{
		_camTransform.RotateAround(cameraRotateTarget.position, _camTransform.right, -deltaY * _deltaSpeed);
	}

	void SwipeEnd(Gesture gesture)
	{
		if(gesture.touchCount>1)
			return;
		ResetSwipingStatus();
		if(tpCameraScript!=null && currentCameraType == CameraType.ThirdPerson)
		{
			if(Mathf.Abs(_lastDeltaX)>0 && Mathf.Abs(_lastDeltaY)>0)
			{
				tpCameraScript.StopInertialAction();
				tpCameraScript.StartInertialAction(_lastDeltaX,_lastDeltaY);
				_lastDeltaX = 0;
				_lastDeltaY = 0;
			}
		}
	}


	//启动easyTouch插件的事件监听.
	void OnEnable()
	{
//		EasyTouch.On_SimpleTap += OnTouchStart;
		EasyTouch.On_Swipe += HandleOn_Swipe;
		EasyTouch.On_SwipeEnd += SwipeEnd;

		EasyTouch.On_TouchStart2Fingers += On_TouchStart2Fingers;
		EasyTouch.On_PinchIn += On_PinchIn;
		EasyTouch.On_PinchOut += On_PinchOut;
		EasyTouch.On_PinchEnd += On_PinchEnd;
		EasyTouch.On_Cancel2Fingers += On_Cancel2Fingers;
		EasyTouch.On_DoubleTap += EasyTouch_On_DoubleTap;
		EasyTouch.On_SimpleTap += EasyTouch_On_SimpleTap;
	}

	void EasyTouch_On_DoubleTap (Gesture gesture)
	{
//		Vuforia.CameraDevice.Instance.SetFocusMode(Vuforia.CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
//		AlertController.Instance.ShowAlert("相机自动对焦成功！");
	}

	//关闭easyTouch插件的事件监听.
	void OnDisable()
	{
//		EasyTouch.On_SimpleTap -= OnTouchStart;
		EasyTouch.On_Swipe -= HandleOn_Swipe;
		EasyTouch.On_SwipeEnd -= SwipeEnd;
		UnsubscribeEvent();
		EasyTouch.On_DoubleTap -= EasyTouch_On_DoubleTap;
		EasyTouch.On_SimpleTap -= EasyTouch_On_SimpleTap;
		//释放.
		EasyTouch.instance.enabledNGuiMode = false;
		EasyTouch.instance.nGUILayers =0;
		EasyTouch.instance.nGUICameras.Clear();
	}

	//关闭easyTouch插件的事件监听.
	public void DestroyMe()
	{
//		EasyTouch.On_SimpleTap -= OnTouchStart;
		EasyTouch.On_Swipe -= HandleOn_Swipe;
		EasyTouch.On_SwipeEnd -= SwipeEnd;
		UnsubscribeEvent();
		EasyTouch.On_DoubleTap -= EasyTouch_On_DoubleTap;
		EasyTouch.On_SimpleTap -= EasyTouch_On_SimpleTap;
		//释放.
		EasyTouch.instance.enabledNGuiMode = false;
		EasyTouch.instance.nGUILayers =0;
		EasyTouch.instance.nGUICameras.Clear();
		_instance = null;
	}

	// Unsubscribe to events
	void UnsubscribeEvent()
	{
		EasyTouch.On_TouchStart2Fingers -= On_TouchStart2Fingers;
		EasyTouch.On_PinchIn -= On_PinchIn;
		EasyTouch.On_PinchOut -= On_PinchOut;
		EasyTouch.On_PinchEnd -= On_PinchEnd;
		EasyTouch.On_Cancel2Fingers -= On_Cancel2Fingers;
	}

	// At the 2 fingers touch beginning
	private void On_TouchStart2Fingers( Gesture gesture)
	{
		//		EasyTouch.SetEnableTwist(false);
		//		EasyTouch.SetEnablePinch(true);
		if(isRotateAndScaleScreenLock)
			return;
//		Debug.Log("On_TouchStart2Fingers");
		//		_isPinch = true;
	}

	// If the two finger gesture is finished
	private void On_Cancel2Fingers(Gesture gesture)
	{
		if(isRotateAndScaleScreenLock)
			return;
//		Debug.Log("On_Cancel2Fingers");
		//		CancelInvoke("PinchEnd");
		//		Invoke("PinchEnd",0.5f);
	}

	//	private bool _isPinch = false;
	private float _deltaSpeed = 0;
	// 放大.
	private void On_PinchIn(Gesture gesture)
	{
		if(isRotateAndScaleScreenLock || currentCameraType == CameraType.FirstPerson)
			return;
//		Debug.Log("On_PinchIn, gesture.deltaPinch is: "+gesture.deltaPinch);
		tpCameraScript.pinchDelta = -gesture.deltaPinch;
	}

	// 缩小.
	private void On_PinchOut(Gesture gesture)
	{
		if(isRotateAndScaleScreenLock || currentCameraType == CameraType.FirstPerson)
			return;
//		Debug.Log("OutOutOut, gesture.deltaPinch is: "+gesture.deltaPinch);
		tpCameraScript.pinchDelta = gesture.deltaPinch;
	}

	// At the pinch end
	private void On_PinchEnd(Gesture gesture)
	{
		if(isRotateAndScaleScreenLock)
			return;
		//EasyTouch.SetEnableTwist(true);
//		Debug.Log("On_PinchEnd");
		//		CancelInvoke("PinchEnd");
		//		Invoke("PinchEnd",0.5f);
	}

	/// <summary>
	/// 用于新手指引的锁定主基地场景的镜头旋转和缩放功能，isLock为true表示锁定，false表示解锁.
	/// </summary>
	/// <param name="isLock">If set to <c>true</c> is lock.</param>
	public void LockCameraRotateAndScale(bool isLock)
	{
		isRotateAndScaleScreenLock = isLock;
	}

	public void ChangeTarget(Transform newTar, ChangeTargetTypeEnum changeType = ChangeTargetTypeEnum.CameraGotoTargetPosition)
	{
		tpCameraScript.ChangeTarget(newTar,changeType);
	}

    public void ChangeTarget(Vector3 pos, ChangeTargetTypeEnum changeType = ChangeTargetTypeEnum.CameraGotoTargetPosition)
    {
        tpCameraScript.ChangeTarget(pos, changeType);
    }

	/// <summary>
	/// 重置相机的关键参数.
	/// </summary>
	public void ResetCameraToNewParamSet(CameraParamStruct cps)
	{
		tpCameraScript.ResetToNewParamSet(cps);
	}

	private Ray _ray;
	private RaycastHit _hitInfo;
	void EasyTouch_On_SimpleTap(Gesture gesture)
	{
		// 如果没有处于分解状态，则不能点击分解后的组件
		if (!GameManager.Instance.mainPanel.isDevided())
		{
			return;
		}

		Vector2 touchPos = gesture.position;
		_ray = GameManager.Instance.curCamera.ScreenPointToRay(new Vector3(touchPos.x,touchPos.y));
		if(Physics.Raycast(_ray,out _hitInfo,1000, _maskForARCameraIgnore))
		{
			ChangeTarget(_hitInfo.collider.transform, ChangeTargetTypeEnum.ChangeTargetImmediatelyWithITween);
			
			switch (_hitInfo.collider.name)
			{
				case "jiashicang":
					curParam = new CameraParamStruct(-36, 10, 0.5f, 0.5f, 0.3f, 2, -40, 80, 0.01f, 0.01f, 0.05f);
					GameManager.Instance.mainPanel.setTextToTextIntroduction("驾驶舱：驾驶舱是宇宙飞船的控制中心，驾驶飞船、规划航线和通讯任务都在驾驶舱中由宇航员完成。\n");
					AudioManager.Instance.PlaySound(AudioName.jiashicang);
					break;
                case "shenghuocang":
                    curParam = new CameraParamStruct(-36, 10, 0.7f, 0.7f, 0.5f, 2, -40, 80, 0.01f, 0.01f, 0.05f);
                    GameManager.Instance.mainPanel.setTextToTextIntroduction("生活舱：对一艘长时间航行的宇宙飞船而言，舒适的生活环境至关重要，科学家设计了一种环形生活舱，可以通过旋转产生的离心力来模拟重力。\n");
					AudioManager.Instance.PlaySound(AudioName.shenghuocang);
                    break;
				case "hengjia":
					curParam = new CameraParamStruct(-36, 10, 0.7f, 0.7f, 0.5f, 2, -40, 80, 0.01f, 0.01f, 0.05f);
                    GameManager.Instance.mainPanel.setTextToTextIntroduction("桁架：桁架是连接宇宙飞船不同部分的结构组件，通常由轻质合金制成。\n");
					AudioManager.Instance.PlaySound(AudioName.hengjia);
                    break;
                case "taiyangnengdianchiban_1":
                    curParam = new CameraParamStruct(-36, 10, 0.5f, 0.5f, 0.4f, 2, -40, 80, 0.01f, 0.01f, 0.05f);
                    GameManager.Instance.mainPanel.setTextToTextIntroduction("太阳能电池板：太阳能电池板能将太阳光中的能量直接转化为电能，是绝大多数宇宙飞船的电力来源。\n");
					AudioManager.Instance.PlaySound(AudioName.taiyangnengdianchiban);
                    break;
                case "huocang":
                    curParam = new CameraParamStruct(-36, 10, 0.5f, 0.5f, 0.4f, 2, -40, 80, 0.01f, 0.01f, 0.05f);
                    GameManager.Instance.mainPanel.setTextToTextIntroduction("货仓：长时间的航行需要携带大量的物资，这些物资都被放在货仓中。\n");
					AudioManager.Instance.PlaySound(AudioName.huocang);
                    break;
                case "duijiekou":
                    curParam = new CameraParamStruct(-36, 10, 0.5f, 0.5f, 0.4f, 2, -40, 80, 0.01f, 0.01f, 0.05f);
                    GameManager.Instance.mainPanel.setTextToTextIntroduction("对接口：对接口的作用是和其它飞船、空间站或者卫星对接。\n");
					AudioManager.Instance.PlaySound(AudioName.duijiekou);
                    break;
                case "qizhacang_1":
                    curParam = new CameraParamStruct(-36, 10, 0.5f, 0.5f, 0.4f, 2, -40, 80, 0.01f, 0.01f, 0.05f);
                    GameManager.Instance.mainPanel.setTextToTextIntroduction("气闸舱：气闸舱是宇航员出舱活动的通道。在出舱活动前，宇航员穿上宇航服，进入气闸舱，关上内侧舱门，把气闸舱中的空气抽空，再打开外侧舱门，就可以进入宇宙的真空中，又不会让飞船中的空气流失。\n");
					AudioManager.Instance.PlaySound(AudioName.qizhacang);
                    break;
                case "fuwucang":
                    curParam = new CameraParamStruct(-36, 10, 0.5f, 0.5f, 0.4f, 2, -40, 80, 0.01f, 0.01f, 0.05f);
                    GameManager.Instance.mainPanel.setTextToTextIntroduction("服务舱：服务舱中安装着宇宙飞船必须的各种仪器和设备。\n");
					AudioManager.Instance.PlaySound(AudioName.fuwucang);
                    break;
                case "ranliaochuguan":
                    curParam = new CameraParamStruct(-36, 10, 0.5f, 0.5f, 0.4f, 2, -40, 80, 0.01f, 0.01f, 0.05f);
                    GameManager.Instance.mainPanel.setTextToTextIntroduction("燃料储罐：燃料储罐中储存着发动机使用的燃料，常见的燃料种类有液态氢、液态氧、液态甲烷、航空煤油、肼和氙气等。\n");
					AudioManager.Instance.PlaySound(AudioName.ranliaochuguan);
                    break;
                case "huojianfadongji":
                    curParam = new CameraParamStruct(-36, 10, 0.5f, 0.5f, 0.4f, 2, -40, 80, 0.01f, 0.01f, 0.05f);
                    GameManager.Instance.mainPanel.setTextToTextIntroduction("火箭发动机：火箭发动机是飞船的动力来源，通过高速喷出气体来产生推力。\n");
					AudioManager.Instance.PlaySound(AudioName.huojianfadongji);
                    break;
            }

			ResetCameraToNewParamSet(curParam);
		}

		if (gesture.touchCount>1)
			return;

//		if(UICamera.hoveredObject)
//		{
//			return;
//		} 

		// GameManager.Instance.wndMainPanel.BgMaskHandler(null);
	}

	public float GetMouseX()
	{
		return tpCameraScript.GetMouseX();
	}

	public float GetMouseY()
	{
		return tpCameraScript.GetMouseY();
	}
}
