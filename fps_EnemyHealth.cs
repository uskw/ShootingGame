using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//控制敌人生命值的脚本
public class fps_EnemyHealth : MonoBehaviour
{
    public float hp = 100f;//敌人生命值

    private Animator anim;//动画组件
    private HashIDs hash;//获取动画的参数
    private bool isDead = false;//表示敌人是否死亡
    
    void Start()
    {
        anim = this.GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
    }

    public void TakeDamage(float damage)//敌人伤害结算
    {
        hp -= damage;
        if(hp <= 0 && !isDead)//敌人已死亡
        {
            isDead = true;

            //禁用
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<fps_EnemyAnimation>().enabled = false;
            GetComponent<fps_EnemyAI>().enabled = false;
            GetComponent<fps_EnemySight>().enabled = false;
            GetComponent<fps_EnemyShoot>().enabled = false;
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            GetComponentInChildren<Light>().enabled = false;
            GetComponentInChildren<LineRenderer>().enabled = false;

            anim.SetBool(hash.playerInSightBool, false);//不在视图中显示
            anim.SetBool(hash.deadBool, true);//已死亡
        }
    }
}