using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//控制敌人状态的切换脚本：巡逻，追击，射击
public class fps_EnemyAI : MonoBehaviour
{
    public float patrolSpeed = 2f;//巡逻的速度
    public float chaseSpeed = 5f;//追击速度
    public float chaseWaitTime = 5f;//当敌人AI丢失玩家视野后等待的时间
    public float patrolWaitTime = 1f;//巡逻到达一个路点，在该路点等待的时间
    public Transform[] patrolWayPoint;//巡逻点的数组

    private fps_EnemySight enemySight;
    private NavMeshAgent nav;
    private Transform player;
    private fps_PlayerHealth playerHealth;
    private float chaseTimer;//追击计时器
    private float patrolTimer;//巡逻计时器
    private int wayPointIndex;//当前巡逻点的索引

    void Start()
    {
        enemySight = this.GetComponent<fps_EnemySight>();
        nav = this.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        playerHealth = player.GetComponent<fps_PlayerHealth>();
    }

    void Update()//进行条件判断，以此进行状态的切换
    {
        if (enemySight.playerInSight && playerHealth.hp > 0f)
            Shooting();
        else if (enemySight.playerPosition != enemySight.resetPosition && playerHealth.hp > 0)
            //enemySight脚本中玩家的位置与默认位置不同时，说明侦听到了玩家的声音,这时需要向玩家进行移动，即追击状态
            Chasing();
        else
            Patrolling();
    }

    private void Shooting()//向玩家射击的方法
    {
        nav.SetDestination(transform.position);//让巡逻点为自身，即不会巡逻
    }

    private void Chasing()//追击玩家的方法
    {
        Vector3 sightingDeltaPos = enemySight.playerPosition - transform.position;
        //敌人朝玩家坐标点方向的向量
        if (sightingDeltaPos.sqrMagnitude > 4f)//向量的平方大于4
            nav.destination = enemySight.playerPosition;//敌人向玩家坐标点追击
        nav.speed = chaseSpeed;

        if (nav.remainingDistance < nav.stoppingDistance)//如果剩余距离小于停止距离
        {
            chaseTimer += Time.deltaTime;//如果在这个点还未发现玩家，需要在此坐标点等待
            if (chaseTimer >= chaseWaitTime)//如果增量时间大于等于等待时间
            {
                enemySight.playerPosition = enemySight.resetPosition;//已经丢失玩家位置，重置玩家位置为默认位置
                chaseTimer = 0f;
            }
        }
        else//还未到达目标点
            chaseTimer = 0f;
    }

    private void Patrolling()//巡逻的方法
    {
        nav.speed = patrolSpeed;
        if (nav.destination == enemySight.resetPosition || nav.remainingDistance < nav.stoppingDistance)
        //如果目标点为敌人的默认位置，或者剩余的距离小于停止距离，说明找到了一个巡逻点，需要停止一段时间并切换到下一个巡逻点
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)//如果计时器大于等于停止时间
            {
                if (wayPointIndex == patrolWayPoint.Length - 1)//如果当前索引为数组长度减一，即找到最后一个
                    wayPointIndex = 0;//再去第一个巡逻点
                else
                    wayPointIndex++;

                patrolTimer = 0;
            }
        }
        else
            patrolTimer = 0;

        nav.destination = patrolWayPoint[wayPointIndex].position;
    }
}