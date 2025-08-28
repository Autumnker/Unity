using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    private Button btn_devide;      // 分解按钮
    private Button btn_restore;     // 重组按钮
    private Image img_TextBG;       // 飞船介绍背景衬底
    private Text txt_Introduction;  // 飞船介绍文本框
    private bool isDevide = false;  // 当前是否处于分解状态
    public Transform spaceShip;     // 宇宙飞船
    public GameObject[] partArray;  // 宇宙飞船零件数组
    
    // Start is called before the first frame update
    void Start()
    {
        // 初始化成员变量(将组件与成员关联起来)
        btn_devide = transform.Find("btn_devide").GetComponent<Button>();
        btn_devide.onClick.AddListener(OnBtnDevideClicked);

        btn_restore = transform.Find("btn_restore").GetComponent<Button>();
        btn_restore.onClick.AddListener(OnBtnRestoreClicked);

        img_TextBG = transform.Find("img_TextBG").GetComponent<Image>();
        img_TextBG.gameObject.SetActive(false); // 默认未启用
        txt_Introduction = transform.Find("img_TextBG/txt_Introduction").GetComponent<Text>();
        txt_Introduction.text = ""; // 初始化文本为空

    }

    // 点击分解按钮后被执行的函数
    public void OnBtnDevideClicked()
    {
        if (!isDevide)
        {
            for(int i = 0; i < partArray.Length; i++)
            {
                // 活得目标位置
                Transform dest = spaceShip.Find("Dest_" + partArray[i].name);
                // 移动组件到目标位置
                iTween.MoveTo(partArray[i],
                    iTween.Hash(
                        "position", dest,
                        "islocal", false,
                        "easetype", iTween.EaseType.linear,
                        "time", 1f
                    ));
            }

            isDevide = true;
        }
    }

    // 点击重组按钮后被执行的函数
    public void OnBtnRestoreClicked()
    {
        if (isDevide)
        {
            for (int i = 0; i < partArray.Length; i++)
            {
                // 活得目标位置
                Transform dest = spaceShip.Find("Source_" + partArray[i].name);
                // 移动组件到目标位置
                iTween.MoveTo(partArray[i],
                    iTween.Hash(
                        "position", dest,
                        "islocal", false,
                        "easetype", iTween.EaseType.linear,
                        "time", 1f
                    ));
            }
            
            isDevide = false;
        }

        // 重组后还原相机参数
        CameraController.Instance.ChangeTarget(spaceShip, ChangeTargetTypeEnum.ChangeTargetImmediatelyWithITween);
        CameraController.Instance.ResetCameraToNewParamSet(CameraController.Instance.defaultParam);

        // 重组后关闭提示框
        img_TextBG.gameObject.SetActive(false);
        txt_Introduction.text = "";           // 清空文本
    }

    // 向介绍文本框中传入文字
    public void setTextToTextIntroduction(string str)
    {
        img_TextBG.gameObject.SetActive(true);
        txt_Introduction.text = str;
    }

    // 返回当前是否处于分解状态
    public bool isDevided()
    {
        return isDevide;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
