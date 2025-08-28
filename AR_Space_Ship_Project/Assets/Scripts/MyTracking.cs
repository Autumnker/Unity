using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 继承自高通插件
/// 新增功能：
///     1.识别成功后，宇宙飞船脱离识别图后依然显示
/// </summary>

public class MyTracking : DefaultObserverEventHandler
{
    public GameObject arCamera;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        // 激活宇宙飞船组件
        GameManager.Instance.OnTrackingFound();
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        //spaceShip.SetActive(false);

        if (GameManager.Instance.SpaceShipSelfActive)
        {
            Transform child = arCamera.transform.Find("VideoBackground");
            if (child != null)
            {
                child.gameObject.SetActive(false);
            }
        }

    }
}
