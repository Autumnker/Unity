using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���ذ�ť��
///     1.��ɨ����ɴ�ģ�ͺ󣬲�����ʾ; ������ɨ�����ʱ��������ʾ��
///     2.�����������ʱ������˰�ť����ɨ����棬�������ݹرա�
/// </summary>

public class Back : MonoBehaviour
{
    public Button  btn_back;

    // Start is called before the first frame update
    void Start()
    {
        btn_back.onClick.AddListener(BackToScan);
    }

    // ����󷵻�ɨ�����
    void BackToScan()
    {
        if (GameManager.Instance.dialogPalenGo.gameObject.activeSelf || GameManager.Instance.mainPanel.gameObject.activeSelf)
        {
            // �ر���Ч������
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.StopSound();

            GameManager.Instance.spaceShipGo.SetActive(false);
            GameManager.Instance.dialogPalenGo.SetActive(false);

            if (GameManager.Instance.mainPanel.gameObject.activeSelf)
            {
                GameManager.Instance.mainPanel.gameObject.SetActive(false);
                GameManager.Instance.mainPanel.OnBtnRestoreClicked();   // �ָ�����״̬
            }

            Transform child = GameManager.Instance.ARCamera.transform.Find("VideoBackground");
            if(child != null)
            {
                child.gameObject.SetActive(true);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
