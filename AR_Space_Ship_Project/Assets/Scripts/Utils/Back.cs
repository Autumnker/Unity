using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 返回按钮：
///     1.当扫描出飞船模型后，才能显示; 当处于扫描界面时，不能显示。
///     2.处于任意界面时，点击此按钮返回扫描界面，其它内容关闭。
/// </summary>

public class Back : MonoBehaviour
{
    public Button  btn_back;

    // Start is called before the first frame update
    void Start()
    {
        btn_back.onClick.AddListener(BackToScan);
    }

    // 点击后返回扫描界面
    void BackToScan()
    {
        if (GameManager.Instance.dialogPalenGo.gameObject.activeSelf || GameManager.Instance.mainPanel.gameObject.activeSelf)
        {
            // 关闭音效、音乐
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.StopSound();

            GameManager.Instance.spaceShipGo.SetActive(false);
            GameManager.Instance.dialogPalenGo.SetActive(false);

            if (GameManager.Instance.mainPanel.gameObject.activeSelf)
            {
                GameManager.Instance.mainPanel.gameObject.SetActive(false);
                GameManager.Instance.mainPanel.OnBtnRestoreClicked();   // 恢复重置状态
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
