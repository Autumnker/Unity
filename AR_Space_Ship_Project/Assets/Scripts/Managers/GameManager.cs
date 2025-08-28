using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��Ϸ�����������й���������ڣ��������й������ĳ�ʼ��
/// </summary>

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;       // ������ʵ��
    public  GameObject spaceShipGo;             // ����ɴ����
    public  GameObject dialogPalenGo;           // �Ի������
    public MainPanel mainPanel;                 // ���Ի���������
    public Camera ARCamera;                     // AR���
    public Camera curCamera;                    // ��ǰ�����

    // ���Զ��壬ֱ�ӷ���˽�г�Ա
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
    }

    // ����ɴ�������
    public void OnTrackingFound()
    {
        spaceShipGo.SetActive(true);
        dialogPalenGo.SetActive(true);
        AudioManager.Instance.PlayMusic();  // ���ű�������
    }

    // ��������ɴ�����״̬
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
