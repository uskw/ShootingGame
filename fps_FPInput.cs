using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//对之前定义的参数进行赋值操作
public class fps_FPInput : MonoBehaviour
{
    public bool LockCursor//游戏开始时，将屏幕的光标进行锁定（不显示光标）
    {
        get { return Cursor.lockState == CursorLockMode.Locked ? true : false; }
        //判断当前光标状态，如果锁定为true，否则为false
        //Cursor.lockState：鼠标当前状态
        //CursorLockMode.Locked：鼠标被锁定
        set//赋值操作
        {
            Cursor.visible = value;//根据赋值设置光标是否可见
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;//根据输入的值设置光标
            //当锁定时，光标将自动居视图中间，并使其从不离开视图
            //这将主要用于Cursor.visible = false，隐藏光标
            //CursorLockMode.none是解锁光标，但是不会把隐藏的光标显示出来
        }
    }

    private fps_PlayerParameter parameter;//需要给角色进行赋值，所以需要一个对象
    private fps_Input input;//输入信息记录的脚本

    void Start()
    {
        LockCursor = true;//将光标锁定
        parameter = this.GetComponent<fps_PlayerParameter>();//获取组件
        input = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<fps_Input>();
        //通过根据物体的名称找到物体
    }

    void Update()//每帧都要监听，查看状态是否改变
    {
        InitialInput();
    }

    private void InitialInput()//对变量进行赋值
    {
        parameter.inputMoveVector = new Vector2(input.GetAxis("Horizontal"), input.GetAxis("Vertical"));
        //鼠标移动的输入
        parameter.inputSmoothLook = new Vector2(input.GetAxisRaw("Mouse X"), input.GetAxisRaw("Mouse Y"));
        //按键，相机的控制（上下左右）
        parameter.inputCrouch = input.GetButton("Crouch");
        parameter.inputJump = input.GetButton("Jump");
        parameter.inputSprint = input.GetButton("Sprint");
        parameter.inputFire = input.GetButton("Fire");
        parameter.inputReload = input.GetButtonDown("Reload");//因为装弹只需要按一次，所以只鉴定按下的信息
    }
}
