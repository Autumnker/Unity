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
        // ����ڱ༭����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // ֹͣ����ģʽ
#else
            Application.Quit(); // �˳���Ϸ
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
