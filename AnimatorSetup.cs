using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//实现敌人动画功能的脚本
public class AnimatorSetup
{
    public float speedDampTime = 0.1f;//速度的缓冲时间
    public float angularSpeedDampTime = 0.7f;//角速度的缓冲时间
    public float angleResponseTime = 1f;//角速度的应答时间，控制角度的大小

    private Animator anim;
    private HashIDs hash;

    public AnimatorSetup(Animator anim, HashIDs hash)
    {
        this.anim = anim;
        this.hash = hash;
    }

    public void Setup(float speed, float angle)
    {
        float angularSpeed = angle / angleResponseTime;//角速度

        anim.SetFloat(hash.speedFloat, speed, speedDampTime, Time.deltaTime);
        anim.SetFloat(hash.angularSpeedFloat, angularSpeed, angularSpeedDampTime, Time.deltaTime);
    }
}