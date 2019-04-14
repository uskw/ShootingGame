using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//出口脚本
public class fps_ExitTrigger : MonoBehaviour
{
    public float timeToInactivePlayer = 2.0f;//触发出口的时间（即在出口停留两秒后，禁用玩家的控制）
    public float timeToRestart = 5.0f;//重新开始的时间（即在出口停留五秒后，游戏重新开始）

    private GameObject player;//玩家
    private bool playerInExit;//判断玩家是否在出口
    private float timer;//计时器
    private FadeInOut fader;//渐隐渐显效果

    void Start()//初始化
    {
        fader = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<FadeInOut>();
        player = GameObject.FindGameObjectWithTag(Tags.player);
    }

    void Update()//调用方法
    {
        if (playerInExit)//如果玩家在出口
            InExitActivation();//调用出口激活方法
    }

    void OnTriggerEnter(Collider other)//玩家刚开始与出口发生碰撞的方法
    {
        if(other.gameObject == player)//如果产生碰撞的是玩家
        {
            playerInExit = true;//表示玩家在出口范围内
        }
    }

    void OnTriggerExit(Collider other)//退出出口的方法
    {
        if(other.gameObject == player)//如果退出的是玩家
        {
            playerInExit = false;
            timer = 0;
        }
    }

    private void InExitActivation()//出口被激活时
    {
        timer += Time.deltaTime;//开始计时

        if (timer >= timeToInactivePlayer)//出口已被触发
            player.GetComponent<fps_PlayerHealth>().DisableInput();//禁用用户的输入
        if (timer >= timeToRestart)//重新开始游戏被触发
            fader.EndScene();//重新加载场景
    }
}