using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    public GameObject shenghuocang;   // �������
    public Vector3 rotationAxis = Vector3.back; // ��ת��
    public float rotationSpeed = 45f; // ��ת�ٶ�
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // ÿ֡��ת
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
