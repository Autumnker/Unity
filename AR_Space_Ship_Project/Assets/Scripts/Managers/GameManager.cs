using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏管理器，所有管理程序的入口，负责所有管理程序的初始化
/// </summary>

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;       // 管理器实例
    public  GameObject spaceShipGo;             // 宇宙飞船组件
    public  GameObject dialogPalenGo;           // 对话框组件
    public MainPanel mainPanel;                 // 主对话框控制组件
    public Camera ARCamera;                     // AR相机
    public Camera curCamera;                    // 当前摄像机

    // 属性定义，直接返回私有成员
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
    }

    // 宇宙飞船相关组件
    public void OnTrackingFound()
    {
        spaceShipGo.SetActive(true);
        dialogPalenGo.SetActive(true);
        AudioManager.Instance.PlayMusic();  // 播放背景音乐
    }

    // 返回宇宙飞船存在状态
    public bool SpaceShipSelfActive
    {
        get { return spaceShipGo.activeSelf; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
