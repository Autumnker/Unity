using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    private Button btn_devide;      // �ֽⰴť
    private Button btn_restore;     // ���鰴ť
    private Image img_TextBG;       // �ɴ����ܱ����ĵ�
    private Text txt_Introduction;  // �ɴ������ı���
    private bool isDevide = false;  // ��ǰ�Ƿ��ڷֽ�״̬
    public Transform spaceShip;     // ����ɴ�
    public GameObject[] partArray;  // ����ɴ��������
    
    // Start is called before the first frame update
    void Start()
    {
        // ��ʼ����Ա����(��������Ա��������)
        btn_devide = transform.Find("btn_devide").GetComponent<Button>();
        btn_devide.onClick.AddListener(OnBtnDevideClicked);

        btn_restore = transform.Find("btn_restore").GetComponent<Button>();
        btn_restore.onClick.AddListener(OnBtnRestoreClicked);

        img_TextBG = transform.Find("img_TextBG").GetComponent<Image>();
        img_TextBG.gameObject.SetActive(false); // Ĭ��δ����
        txt_Introduction = transform.Find("img_TextBG/txt_Introduction").GetComponent<Text>();
        txt_Introduction.text = ""; // ��ʼ���ı�Ϊ��

    }

    // ����ֽⰴť��ִ�еĺ���
    public void OnBtnDevideClicked()
    {
        if (!isDevide)
        {
            for(int i = 0; i < partArray.Length; i++)
            {
                // ���Ŀ��λ��
                Transform dest = spaceShip.Find("Dest_" + partArray[i].name);
                // �ƶ������Ŀ��λ��
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

    // ������鰴ť��ִ�еĺ���
    public void OnBtnRestoreClicked()
    {
        if (isDevide)
        {
            for (int i = 0; i < partArray.Length; i++)
            {
                // ���Ŀ��λ��
                Transform dest = spaceShip.Find("Source_" + partArray[i].name);
                // �ƶ������Ŀ��λ��
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

        // �����ԭ�������
        CameraController.Instance.ChangeTarget(spaceShip, ChangeTargetTypeEnum.ChangeTargetImmediatelyWithITween);
        CameraController.Instance.ResetCameraToNewParamSet(CameraController.Instance.defaultParam);

        // �����ر���ʾ��
        img_TextBG.gameObject.SetActive(false);
        txt_Introduction.text = "";           // ����ı�
    }

    // ������ı����д�������
    public void setTextToTextIntroduction(string str)
    {
        img_TextBG.gameObject.SetActive(true);
        txt_Introduction.text = str;
    }

    // ���ص�ǰ�Ƿ��ڷֽ�״̬
    public bool isDevided()
    {
        return isDevide;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
