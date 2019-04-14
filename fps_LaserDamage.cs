using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//激光墙的脚本
public class fps_LaserDamage : MonoBehaviour
{
    public int damage = 30;//激光墙对玩家造成的伤害值
    public float damageDelay = 1;//如果玩家一直处于激光墙的范围内，隔多少秒进行一次伤害

    private float lastDamageTime = 0;//辅助计算
    private GameObject player;//角色

    void Start()//对玩家初始化
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
    }

    void OnTriggerStay(Collider other)//检测碰撞信息 
    {
        if(other.gameObject == player && Time.time > lastDamageTime+damageDelay)
            //如果碰撞对象为玩家并且时间允许
        {
            player.GetComponent<fps_PlayerHealth>().TakeDamage(damage);//对玩家造成伤害
            lastDamageTime = Time.time;//对时间进行重置
        }
    }
}