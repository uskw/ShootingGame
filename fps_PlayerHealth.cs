using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySampleAssets.ImageEffects;

//代表玩家生命值的脚本
public class fps_PlayerHealth : MonoBehaviour
{
    public bool isDead;//表示玩家是否死亡的标识位
    public float resetAfterDeathTime = 5;//表示玩家死亡时等待多长时间进行场景的重置
    public AudioClip deathClip;//死亡时要播放的音频
    public AudioClip damageClip;//受伤时要播放的音频
    public float maxHp = 100;//玩家最大生命值
    public float hp = 100;//玩家当前生命值
    public float recoverSpeed = 1;//生命值的恢复速度（未受伤时）

    private float timer = 0;//辅助计时器
    private FadeInOut fader;//玩家在死亡时需要场景渐隐的效果
    private ColorCorrectionCurves colorCurves;//实现死亡时相机画面逐渐变为黑白效果的脚本（内置资源包）

    void Start()
    {
        hp = maxHp;//玩家当前生命值为最大值
        fader = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<FadeInOut>();
        //出现渐隐效果
        colorCurves = GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<ColorCorrectionCurves>();
        //出现黑白效果
        BleedBehavior.BloodAmount = 0;//血红效果为0（在武器相机中设置）
    }

    void Update()//调用方法
    {
        if(!isDead)//如果处于非死亡状态
        {
            hp += recoverSpeed * Time.deltaTime;//生命值+=恢复速度*时间
            if (hp > maxHp)//生命值大于最大值
                hp = maxHp;//直接等于最大值
        }
        if(hp < 0)//如果生命值<0，即玩家受到致命伤
        {
            if (!isDead)//如果还没有死亡
                PlayerDead();//死亡
            else//已经死亡
                LevelReset();//重置关卡（有计时功能）
        }
    }

    public void TakeDamage(float damage)//受攻击的函数
    {
        if (isDead)
            return;
        AudioSource.PlayClipAtPoint(damageClip, transform.position);//播放受攻击的音效
        BleedBehavior.BloodAmount += Mathf.Clamp01(damage / hp);//血红效果增加（表示浓郁程度），受到伤害越高，血红效果越明显
        hp -= damage;//受到伤害，生命值下降
    }

    public void DisableInput()//在通关或死亡时，禁止用户输入
    {
        transform.Find("FP_Camera/Weapon_Camera").gameObject.SetActive(false);
        //禁用武器相机的武器和UI
        this.GetComponent<AudioSource>().enabled = false;//禁用播放的音效
        this.GetComponent<fps_PlayerControl>().enabled = false;//禁用角色的控制脚本
        this.GetComponent<fps_FPInput>().enabled = false;//禁用输入
        if (GameObject.Find("Canvas") != null)//UI显示弹夹的数量
            GameObject.Find("Canvas").SetActive(false);//禁用
        colorCurves.gameObject.GetComponent<fps_FPCamera>().enabled = false;//禁用相机控制
    }

    public void PlayerDead()//玩家死亡的方法
    {
        isDead = true;
        colorCurves.enabled = true;//出现黑白效果
        DisableInput();//禁用所有的输入，因为玩家已经死亡
        AudioSource.PlayClipAtPoint(deathClip, transform.position);//播放死亡时的音效

        //Debug.Log(colorCurves.enabled);
    }

    public void LevelReset()//关卡重置
    {
        timer += Time.deltaTime;//计时器
        colorCurves.saturation -= (Time.deltaTime / 2);//逐渐变为黑白的效果
        colorCurves.saturation = Mathf.Max(0, colorCurves.saturation);//保证值大于0
        if (timer >= resetAfterDeathTime)//如果超过了死亡等待时间
            fader.EndScene();//重新加载场景的方法
    }
}