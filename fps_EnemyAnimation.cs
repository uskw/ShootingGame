using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//用来控制敌人的动画
public class fps_EnemyAnimation : MonoBehaviour
{
    public float deadZone = 5f;//最小角度，当敌人旋转到离目标的旋转方向小于此值时，认为已经达到了旋转方向，将旋转设置为想要达到的目标的旋转方向

    private Transform player;
    private fps_EnemySight enemySight;
    private NavMeshAgent nav;
    private Animator anim;
    private HashIDs hash;
    private AnimatorSetup animSetup;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        enemySight = this.GetComponent<fps_EnemySight>();
        nav = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        animSetup = new AnimatorSetup(anim, hash);

        nav.updateRotation = false;//通过动画来控制敌人AI的旋转
        anim.SetLayerWeight(1, 1f);//设置动画状态机层级的权重，第一层为1
        anim.SetLayerWeight(2, 1f);//第二层为1
        //因为设置了骨骼作用的范围，设置权重就不会影响到其他层动画的播放

        deadZone *= Mathf.Deg2Rad;//把角度转换成弧度
    }

    void Update()
    {
        NavAnimatorSetup();
    }

    void OnAnimatorMove()//控制敌人移动的方法
    {
        nav.velocity = anim.deltaPosition / Time.deltaTime;//给寻路带一个速度
        transform.rotation = anim.rootRotation;//通过动画来设置敌人AI的旋转
    }

    void NavAnimatorSetup()//给AnimatorSetup传递参数
    {
        float speed;//速度参数
        float angle;

        if(enemySight.playerInSight)//如果玩家在敌人的视野当中
        {
            speed = 0;//玩家不需要移动
            angle = FindAngle(transform.forward, player.position-transform.position, transform.up);
        }
        else//玩家不在敌人的视野当中，计算寻路代理组件的需求速度在敌人正前方的投影作为速度
        {
            speed = Vector3.Project(nav.desiredVelocity, transform.forward).magnitude;//投影函数
            //nav.desiredVelocity：寻路组件的需求速度
            //transform.forward：正前方的投影
            //magnitude：只取投影的长度作为速度
            angle = FindAngle(transform.forward, nav.desiredVelocity, transform.up);

            if(Mathf.Abs(angle) < deadZone)
            {
                transform.LookAt(transform.position + nav.desiredVelocity);//直接达到目标的旋转
                angle = 0;
            }
        }
        animSetup.Setup(speed, angle);
    }

    private float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)//计算角度
    {
        if (toVector == Vector3.zero)
            return 0f;

        float angle = Vector3.Angle(fromVector, toVector);//计算两个方向夹角的绝对值
        Vector3 normal = Vector3.Cross(fromVector, toVector);//计算出两个向量的法向量
        angle *= Mathf.Sign(Vector3.Dot(normal, upVector));//通过点乘获取方向（只取符号）
        angle *= Mathf.Deg2Rad;//把角度转成弧度 

        return angle;
    }
}