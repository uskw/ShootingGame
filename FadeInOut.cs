using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//控制屏幕渐隐渐显
public class FadeInOut : MonoBehaviour
{
    public float fadeSpeed = 1.5f;//控制渐隐渐显的速度
    private bool sceneStarting = true;//用于表示场景是否开始，如果开始，使屏幕有一个渐隐的效果
    private GUITexture tex;//外部添加的GUI组件

    void Start()//最先被调用的函数
    {
        tex = this.GetComponent<GUITexture>();//获取对象的GUITexture组件
        tex.pixelInset = new Rect(0, 0, Screen.width, Screen.height);//更改组件的位置信息，坐标为（0，0），大小为屏幕的宽高。
                                                                  //用于GUITexture的初始化
    }

    void Update()//因为GUITexture的Lerp函数必须在update函数中调用才有用
    {
        if(sceneStarting)//如果游戏开始
        {
            StartScene();//开始游戏
        }
    }

    private void FadeToClear()//渐隐的方法
    {
        tex.color = Color.Lerp(tex.color, Color.clear, fadeSpeed*Time.deltaTime);
        //color.Lerp(a,b,time),参数从a到b变化需要time时间
        //Color.clear：清空颜色，即完全透明

        //Time.deltaTime：增量时间，保证现实中的1s和游戏中的1s是一样的，类似于速率。即fadeSpeed越大，速率越大，改变的越快。
    }

    private void FadeToBlack()//渐显的方法
    {
        tex.color = Color.Lerp(tex.color, Color.black, fadeSpeed*Time.deltaTime);
    }

    private void StartScene()//代表游戏开始
    {
        FadeToClear();//出现渐隐效果

        if(tex.color.a <= 0.05f)//如果tex的颜色的alpha通道透明度小于等于0.05
        {
            tex.color = Color.clear;//将tex的颜色改为完全透明
            tex.enabled = false;//设置组件为不可用的状态
            sceneStarting = false;//代表场景已经开始
        }
    }

    public void EndScene()//代表游戏结束 因为有其他场景需要调用所以为public
    {
        tex.enabled = true;//激活GUI组件
        FadeToBlack();//出现渐显效果

        if(tex.color.a >= 0.95f)//如果tex的颜色的alpha通道透明度大于等于0.95
        {
            SceneManager.LoadScene("MainScene");//重新加载主场景
        }
    }
}
