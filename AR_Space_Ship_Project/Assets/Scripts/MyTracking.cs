using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// �̳��Ը�ͨ���
/// �������ܣ�
///     1.ʶ��ɹ�������ɴ�����ʶ��ͼ����Ȼ��ʾ
/// </summary>

public class MyTracking : DefaultObserverEventHandler
{
    public GameObject arCamera;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        // ��������ɴ����
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
