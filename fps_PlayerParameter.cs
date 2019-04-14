using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
//自动为游戏对象添加CharacterController组件
public class fps_PlayerParameter : MonoBehaviour//用来存储一些变量，用来表示用户的输入状态
{
    //都是通过用户的输入状态为其进行赋值，但是不需要在面板中显示出来
    [HideInInspector]//特性：隐藏
    public Vector2 inputSmoothLook;//表示鼠标的输入 Vector2：代表一个二维变量
    [HideInInspector]
    public Vector2 inputMoveVector;//按键（向量）
    [HideInInspector]
    public bool inputCrouch;//是否蹲伏
    [HideInInspector]
    public bool inputJump;//是否跳跃
    [HideInInspector]
    public bool inputSprint;//是否冲刺
    [HideInInspector]
    public bool inputFire;//是否开火
    [HideInInspector]
    public bool inputReload;//是否装弹
}