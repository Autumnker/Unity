using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    public GameObject shenghuocang;   // 组件名称
    public Vector3 rotationAxis = Vector3.back; // 旋转轴
    public float rotationSpeed = 45f; // 旋转速度
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 每帧旋转
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}
