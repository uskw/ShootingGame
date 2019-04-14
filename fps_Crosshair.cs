using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器十字准线的绘制脚本
public class fps_Crosshair : MonoBehaviour
{
    public float length;//准星的长度
    public float width;//准星的宽度
    public float distance;//表示左右准星以及上下距离
    public Texture2D crosshairTexture;//表示准星的背景图

    private GUIStyle lineStyle;//设置背景图
    private Texture tex;//辅助的变量

    void Start()
    {
        lineStyle = new GUIStyle();//生成一个GUIStyle的对象
        lineStyle.normal.background = crosshairTexture;//将GUIStyle正常状态下的背景图设置为crosshairTexture
    }

    void OnGUI()//进行准星的绘制
    {
        GUI.Box(new Rect((Screen.width - distance) / 2 - length, (Screen.height - width) / 2, length, width), tex, lineStyle);//左
        GUI.Box(new Rect((Screen.width + distance) / 2, (Screen.height - width) / 2, length, width), tex, lineStyle);//右
        GUI.Box(new Rect((Screen.width - width) / 2, (Screen.height - distance) / 2 - length, width, length), tex, lineStyle);//上
        GUI.Box(new Rect((Screen.width - width) / 2, (Screen.height + distance) / 2, width, length), tex, lineStyle);//下
    }
}