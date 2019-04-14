using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//包括用户自定义的物体，用户输入包括三部分
public class fps_Input : MonoBehaviour
{
   public class fps_InputAxis//存储自定义轴向的类
    {
        public KeyCode positive;//正向
        public KeyCode negative;//负向
    }

    //定义三个集合

    public Dictionary<string, KeyCode> buttons = new Dictionary<string, KeyCode>();//以按键名称以及对应的键位进行键值对存储
                                                                                   //按键包括键盘按键以及鼠标按键
    //Dictionary<string, name>：string表示集合名称，name表示集合类型
    public Dictionary<string, fps_InputAxis> axis = new Dictionary<string, fps_InputAxis>();//自定义轴
    public List<string> unityAxis = new List<string>();//unity自定义的轴，只需要通过集合存储名称

    void Start()//调用初始化函数，一次性初始化所有按键
    {
        SetupDefaults();
    }

    private void SetupDefaults(string type = "")//设置默认的按键
    {
        if(type == "" || type == "buttons")//type为空串或buttons，为按键按钮的初始化
        {
            if(buttons.Count == 0)//如果buttons.Count为0，代表没有进行过初始化
            {
                AddButton("Fire", KeyCode.Mouse0);//开火键 鼠标左键
                AddButton("Reload", KeyCode.R);//装弹键
                AddButton("Jump", KeyCode.Space);//跳跃键 空格键
                AddButton("Crouch", KeyCode.C);//蹲伏键
                AddButton("Sprint", KeyCode.LeftShift);//冲刺键
            }
        }

        if (type == "" || type == "Axis")//type为空串或Axis，为轴向按钮的初始化
        {
            if (axis.Count == 0)
            {
                AddAxis("Horizontal", KeyCode.W, KeyCode.S);//角色横向移动 w前进 s后退 
                AddAxis("Vertical", KeyCode.A, KeyCode.D);//角色竖向移动 a左 d右
            }
        }

        if (type == "" || type == "UnityAxis")//type为空串或UnityAxis，为unity自带轴向按钮的初始化
        {
            if (unityAxis.Count == 0)
            {
                AddUnityAxis("Mouse X");//鼠标沿屏幕x移动时触发
                AddUnityAxis("Mouse Y");//鼠标沿屏幕y移动时触发
                AddUnityAxis("Horizontal");//键盘按上或下键时触发
                AddUnityAxis("Vertical");//键盘按左或右键时触发
            }
        }
    }

    private void AddButton(string n, KeyCode k)//添加按键的函数（名称，键位）
    {
        if (buttons.ContainsKey(n))//如果按键集合包含当前按键
            buttons[n] = k;//把当前按键重新赋值给新的键位
        else//如果不包含
            buttons.Add(n, k);//把当前按键添加进集合中
    }

    private void AddAxis(string n, KeyCode pk, KeyCode nk)//添加轴向，pk为正向，nk为负向
    {
        if (axis.ContainsKey(n))//如果自定义轴向集合中包含当前轴向
            axis[n] = new fps_InputAxis() { positive = pk, negative = nk };//更改轴向
        else//如果不包含
            axis.Add(n, new fps_InputAxis() { positive = pk, negative = nk});//添加轴向
    }

    private void AddUnityAxis(string n)//添加unity自带的轴
    {
        if (!unityAxis.Contains(n))//如果不包含
            unityAxis.Add(n);//添加
    }

    public bool GetButton(string button)//外界能够调用按钮的事件，以参数形式进行返回。只要按键被按下没有抬起，就会一直被调用
    {
        if (buttons.ContainsKey(button))//如果按键集合包含当前按键
            return Input.GetKey(buttons[button]);//返回true
        return false;//不存在此按键 无反应
    }

    public bool GetButtonDown(string button)//只获取按下的事件
    {
        if (buttons.ContainsKey(button))//如果当前按钮刚刚被按下
            return Input.GetKey(buttons[button]);//返回true
        return false;//不存在此按键 无反应
    }

    public float GetAxis(string axisname)//返回轴向具体的值，值在-1到1之间
    {
        if (this.unityAxis.Contains(axisname))
            return Input.GetAxis(axisname);//返回值是一个数，正负代表方向
        else
            return 0;
    }

    public float GetAxisRaw(string axisname)//获取轴向的方向，但是结果只有三个值：-1，0，1
    {
        if (this.axis.ContainsKey(axisname))//在自定义轴向中包含
        {
            float val = 0;
            if (Input.GetKey(this.axis[axisname].positive))//正向返回1
                return 1;
            if (Input.GetKey(this.axis[axisname].negative))//负向返回-1
                return -1;
            return val;//返回0
        }
        else if (unityAxis.Contains(axisname))//在unity自带的轴向中包含
            return Input.GetAxisRaw(axisname);
        else
            return 0;
    }
}