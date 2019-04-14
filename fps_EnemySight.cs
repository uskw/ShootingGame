using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//用来表示敌人的视野
public class fps_EnemySight : MonoBehaviour
{
    public float fieldOfViewAngle = 110f;//敌人视野的角度
    public bool playerInSight;//用来判断玩家是否在敌人视野内
    public Vector3 playerPosition;//代表玩家的位置
    public Vector3 resetPosition = Vector3.zero;//默认的位置

    private NavMeshAgent nav;//寻路的组件，寻路代理
    private SphereCollider col;//侦听范围的触发器
    private Animator anim;//动画组件
    private GameObject player;//玩家
    private fps_PlayerHealth playerHealth;//玩家血量的控制脚本
    private HashIDs hash;//存储动画参数的脚本
    private fps_PlayerControl playerControl;//玩家的控制脚本

    void Start()
    {
        nav = this.GetComponent<NavMeshAgent>();
        col = GetComponentInChildren<SphereCollider>();
        anim = this.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerHealth = player.GetComponent<fps_PlayerHealth>();
        playerControl = player.GetComponent<fps_PlayerControl>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

        fps_GunScript.PlayerShootEvent += ListenPlayer;//武器是否开枪的委托
    }

    void Update()//判断
    {
        if (playerHealth.hp > 0)
            anim.SetBool(hash.playerInSightBool, playerInSight);
        else
            anim.SetBool(hash.playerInSightBool, false);
    }

    void OnTriggerStay(Collider other)//用来侦听玩家
    {
        if(other.gameObject == player)
        {
            playerInSight = false;
            Vector3 direction = other.transform.position - transform.position;//方向
            //用玩家的坐标-AI的坐标，即从敌人指向玩家的一个向量
            float angle = Vector3.Angle(direction, transform.forward);//角度
            if(angle < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;//用视线判断玩家和敌人之间是否隔墙
                if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
                    //通过一个点和一个方向确定一条射线，通过触发器的半径判断玩家是否在敌人视野范围内
                {
                    if (hit.collider.gameObject == player)
                    {
                        playerInSight = true;
                        playerPosition = player.transform.position;
                    }
                }
            }
            if(playerControl.State == PlayerState.Walk || playerControl.State == PlayerState.Run)
                //如果玩家在走或者跑
            {
                ListenPlayer();//开始侦听玩家的位置
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)//如果玩家退出的话
        {
            playerInSight = false;
        }
    }

    private void ListenPlayer()//侦听事件
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= col.radius)
            //通过触发器的半径判断玩家的位置
            playerPosition = player.transform.position;
    }

    void OnDestroy()//取消对视线的侦听
    {
        fps_GunScript.PlayerShootEvent -= ListenPlayer;
    }
}