using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//控制敌人射击的脚本
public class fps_EnemyShoot : MonoBehaviour
{
    public float maximumDamage = 120f;//子弹的最大伤害
    public float minimumDamage = 45f;//子弹的最小伤害
    public AudioClip shotClip;//射击的音效
    public float flashIntensity = 3f;//射击时的闪光强度
    public float fadeSpeed = 10f;//发光渐变的速度

    private Animator anim;
    private HashIDs hash;
    private LineRenderer laserShotLine;
    private Light laserShotLight;
    private SphereCollider col;//表示触发的范围
    private Transform player;
    private fps_PlayerHealth playerHealth;//玩家的生命值
    private bool shooting;//是否能够开枪
    private float scaleDamage;//伤害的取值范围

    void Start()
    {
        anim = this.GetComponent<Animator>();
        laserShotLine = this.GetComponentInChildren<LineRenderer>();
        laserShotLight = laserShotLine.gameObject.GetComponent<Light>();
        col = GetComponentInChildren<SphereCollider>();
        player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        playerHealth = player.GetComponent<fps_PlayerHealth>();
        hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

        laserShotLine.enabled = false;
        laserShotLight.intensity = 0;

        scaleDamage = maximumDamage - minimumDamage;//计算伤害范围
    }

    void Update()
    {
        float shot = anim.GetFloat(hash.shotFloat);//shot动画的一个参数

        if (shot > 0.05f && !shooting)//表示在0.5s后开始射击
            Shoot();
        if(shot < 0.05f)
        {
            shooting = false;
            laserShotLine.enabled = false;
        }

        laserShotLight.intensity = Mathf.Lerp(laserShotLight.intensity, 0f, fadeSpeed * Time.deltaTime);
        //光照强度从当前强度渐变到0 
    }

    void OnAnimatorIK(int layerIndex)//指向玩家的方法（unity内置函数），默认传递当前动画的层
    {
        float aimWeight = anim.GetFloat(hash.aimWeightFloat);//代表当前动画播放的进度
        anim.SetIKPosition(AvatarIKGoal.RightHand, player.position + Vector3.up * 1.5f);//将枪指向的位置增高
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, aimWeight);//设置权重
    }

    private void Shoot()//敌人射击的方法
    {
        shooting = true;
        float fractionalDistance = ((col.radius - Vector3.Distance(transform.position, player.position)) / col.radius);
        //计算对应的距离，如果距离越近，伤害越高，距离越远，伤害越低
        float damage = scaleDamage * fractionalDistance + minimumDamage;//当前的伤害值

        playerHealth.TakeDamage(damage);
        ShotEffects();
    }

    private void ShotEffects()//射击特效
    {
        laserShotLine.SetPosition(0, laserShotLine.transform.position);
        laserShotLine.SetPosition(1, player.position + Vector3.up * 1.5f);
        //射线的坐标

        laserShotLine.enabled = true;
        laserShotLight.intensity = flashIntensity;

        AudioSource.PlayClipAtPoint(shotClip, laserShotLight.transform.position);//播放射击音效
    }
}