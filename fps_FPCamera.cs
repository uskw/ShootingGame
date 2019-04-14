using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class fps_FPCamera : MonoBehaviour
{
    public Vector2 mouseLookSensitivity = new Vector2(5, 5);//鼠标灵敏度
    public Vector2 rotationXLimit = new Vector2(87, -87);//旋转上下视角的限制
    public Vector2 rotationYLimit = new Vector2(-360, 360);//旋转左右视角的限制
    public Vector3 positionOffset = new Vector3(0, 2, -0.2f);//相机的位置

    private Vector2 currentMouseLook = Vector2.zero;//当前的输入状态
    private float x_Angle = 0;//默认x轴的旋转角度
    private float y_Angle = 0;//默认y轴的旋转角度
    private fps_PlayerParameter parameter;//需要一个参数获取鼠标输入
    private Transform m_Transform;//缓存避免重复调用，Transform组件

    void Start()
    {
        parameter = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerParameter>();
        //获取输入信息，该组件存储了player的状态
        m_Transform = transform;//因为Transform是挂载的组件，可以直接赋值
        m_Transform.localPosition = positionOffset;//防止误操作，在初始情况下赋值
    }

    void Update()//对输入进行处理
    {
        UpdateInput();
    }

    void LateUpdate()//调整旋转
    {
        Quaternion xQuaternion = Quaternion.AngleAxis(y_Angle, Vector3.up);//绕Vector3.up这个轴进行旋转，即水平偏移
        Quaternion yQuaternion = Quaternion.AngleAxis(0, Vector3.left);//旋转角度一开始设置为0
        //AngleAxis (angle : float, axis : Vector3)
        //绕axis轴旋转angle，创建一个旋转
        m_Transform.parent.rotation = xQuaternion * yQuaternion;//让camera的父辈，即camera旋转

        yQuaternion = Quaternion.AngleAxis(-x_Angle, Vector3.left);//player不会旋转，但是人可以旋转
        m_Transform.rotation = xQuaternion * yQuaternion;
    }

    private void UpdateInput()
    {
        if (parameter.inputSmoothLook == Vector2.zero)//没有用户输入
            return;
        GetMouseLook();//如果有，先获取用户输入
        y_Angle += currentMouseLook.x;//y轴的旋转对应的是左右视角的变换，即由鼠标x轴的数值来控制
        //y_Angle：y代表旋转的轴向
        //currentMouseLook.x：x代表鼠标输入的轴向
        x_Angle += currentMouseLook.y;

        y_Angle = y_Angle < -360 ? y_Angle += 360 : y_Angle;
        y_Angle = y_Angle > 360 ? y_Angle -= 360 : y_Angle;
        y_Angle = Mathf.Clamp(y_Angle, rotationYLimit.x, rotationYLimit.y);
        // Mathf.Clamp 为一个限制函数，Clamp (value : float, min : float, max : float) 
        // 限制value的值在min和max之间， 如果value小于min，返回min。 如果value大于max，返回max，否则返回value

        x_Angle = x_Angle < -360 ? x_Angle += 360 : x_Angle;
        x_Angle = x_Angle > 360 ? x_Angle -= 360 : x_Angle;
        x_Angle = Mathf.Clamp(x_Angle, -rotationXLimit.x, -rotationXLimit.y);
    }

    private void GetMouseLook()//对用户的输入进行处理
    {
        currentMouseLook.x = parameter.inputSmoothLook.x * mouseLookSensitivity.x;
        currentMouseLook.y = parameter.inputSmoothLook.y * mouseLookSensitivity.y;
        //获取鼠标输入的信息，再乘以鼠标的灵敏度
        currentMouseLook.y *= -1;//修改y轴方向
    }
}