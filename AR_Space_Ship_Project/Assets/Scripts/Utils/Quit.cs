using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quit : MonoBehaviour
{
    public Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        quitButton.onClick.AddListener(QuitGame);
    }

    void QuitGame()
    {
        // 如果在编辑器中
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 停止播放模式
#else
            Application.Quit(); // 退出游戏
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
