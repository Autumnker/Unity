using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 开局提示框
/// </summary>

public class DialogPanel : MonoBehaviour
{
    private Button btn_ok;
    public GameObject mainPanelGo;

    // Start is called before the first frame update
    void Start()
    {
        btn_ok = transform.Find("btn_ok").GetComponent<Button>();
        btn_ok.onClick.AddListener(OnBtnClicked);
    }

    void OnBtnClicked()
    {
        gameObject.SetActive(false);
        mainPanelGo.SetActive(true);
    }
}
