using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void PlayerShoot();//定义一个无返回值无参的委托

//武器开枪时出现火光动画的脚本
public class fps_GunScript : MonoBehaviour
{
    public static event PlayerShoot PlayerShootEvent;//监听玩家射击的事件
    public float fireRate = 0.1f;//射击的间隔
    public float damage = 40;//每颗子弹的伤害值
    public float reloadTime = 1.5f;//重新装弹的时间
    public float flashRate = 0.02f;//闪光效果持续的时间
    public AudioClip fireAudio;//开火的音效
    public AudioClip reloadAudio;//重新装弹的音效
    public AudioClip damageAudio;//受伤的音效
    public AudioClip dryFireAudio;//没有子弹时不能开火的音效
    public GameObject explosion;//子弹在攻击到敌人金属外壳时的爆炸特效
    public int bulletCount = 30;//武器最多可装弹数量
    public int chargerBulletCount = 60;//玩家可携带的弹夹数量
    public Text bulletText;//在右下角用来显示子弹数量的字符串

    //动画的名称
    private string reloadAnim = "Reload";//重新装弹
    private string fireAnim = "Single_Shot";
    private string walkAnim = "Walk";
    private string runAnim = "Run";
    private string jumpAnim = "Jump";
    private string idleAnim = "Idle";

    private Animation anim;
    private float nextFireTime = 0.0f;//实现射击间隔的辅助参数
    private MeshRenderer flash;//闪光效果
    private int currentBullet;//当前的子弹数量
    private int currentChargerBullet;//当前剩余子弹数量
    private fps_PlayerParameter parameter;//输入参数
    private fps_PlayerControl playerControl;//角色控制脚本

    void Start()
    {
        parameter = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerParameter>();
        playerControl = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerControl>();
        anim = this.GetComponent<Animation>();
        flash = this.transform.Find("muzzle_flash").GetComponent<MeshRenderer>(); //填闪光效果物体的名称
        flash.enabled = false;

        //初始化子弹数量
        currentBullet = bulletCount;
        currentChargerBullet = chargerBulletCount;

        bulletText.text = currentBullet + "/" + currentChargerBullet;//右下角显示的子弹数量
    }

    void Update()
    {
        if (parameter.inputReload && currentBullet < bulletCount)//如果输入重新装弹且当前子弹数量小于子弹上限
            Reload();//可以装弹
        if (parameter.inputFire && !anim.IsPlaying(reloadAnim))//如果输入开火按键且没有进行重新装弹
            Fire();//可以射击
        else if (!anim.IsPlaying(reloadAnim))//如果不是在重新装弹
            StateAnim(playerControl.State);//播放状态动画
    }

    private void ReloadAnim()//重新装弹的动画
    {
        anim.Stop(reloadAnim);
        anim[reloadAnim].speed = (anim[reloadAnim].clip.length / reloadTime);
        anim.Rewind(reloadAnim);
        anim.Play(reloadAnim);
    }

    private IEnumerator ReloadFinish()//装弹之后更新子弹数量显示文本的方法
    {
        yield return new WaitForSeconds(reloadTime);
        if(currentChargerBullet >= bulletCount - currentBullet)//如果当前剩余子弹数量大于等于可装弹数量
        {
            currentChargerBullet -= (bulletCount - currentBullet);//当前剩余子弹数量减去可装弹数量
            currentBullet = bulletCount;//当前子弹数量等于最大数量
        }
        else//如果剩余子弹数量不足
        {
            currentBullet += currentChargerBullet;//当前子弹数量加上当前剩余子弹数量
            currentChargerBullet = 0;//当前剩余子弹数量为0
        }
        bulletText.text = currentBullet + "/" + currentChargerBullet;
    }

    private void Reload()//重新装弹的方法
    {
        if(!anim.IsPlaying(reloadAnim))//如果当前不是重新装弹的状态
        {
            if (currentChargerBullet > 0)//如果当前剩余子弹数量大于0
                StartCoroutine(ReloadFinish());//开启使用程序，自动更新文本信息
            else//当前剩余子弹数量小于0
            {
                anim.Rewind(fireAnim);//播放正常攻击动画
                anim.Play(fireAnim);
                AudioSource.PlayClipAtPoint(dryFireAudio, transform.position);//播放没有子弹的音效
                return;
            }
            AudioSource.PlayClipAtPoint(reloadAudio, transform.position);//播放重新装弹的音效
            ReloadAnim();//播放重新装弹的动画
        }
    }

    private IEnumerator Flash()//闪光效果的持续时间
    {
        flash.enabled = true;//开启闪光效果
        yield return new WaitForSeconds(flashRate);//持续闪光时间之后
        flash.enabled = false;//禁用闪光效果
    }

    private void Fire()//开火方法
    {
        if(Time.time > nextFireTime)//如果当前时间大于下一次射击时间
        {
            if(currentBullet <= 0)//如果当前子弹数量小于等于0
            {
                Reload();//重新装弹
                nextFireTime = Time.time + fireRate;//让下一次射击时间=当前时间+射击间隔
                return;
            }
            currentBullet--;//当前有子弹，让子弹数量自减
            bulletText.text = currentBullet + "/" + currentChargerBullet;//更改文本信息
            DamageEnemy();
            if (PlayerShootEvent != null)//如果玩家在射击
                PlayerShootEvent();//发送射击事件
            AudioSource.PlayClipAtPoint(fireAudio, transform.position);//播放开火音效
            nextFireTime = Time.time + fireRate;//下一次射击时间=当前时间+射击间隔
            anim.Rewind(fireAnim);//播放开火动画
            anim.Play(fireAnim);
            StartCoroutine(Flash());//开启使用程序，自动关闭闪光效果
        }
    }

    private void DamageEnemy()//对敌人造成伤害的方法
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));//用射线检测是否碰撞到敌人
        RaycastHit hit;//碰撞信息

        if(Physics.Raycast(ray, out hit))//如果有碰撞信息的返回
        {
            if(hit.transform.tag == Tags.enemy && hit.collider is CapsuleCollider)//如果碰撞到了敌人
            {
                AudioSource.PlayClipAtPoint(damageAudio, transform.position);//播放受伤音效
                GameObject go = Instantiate(explosion, hit.point, Quaternion.identity);
                //Instantiate函数实例化是将original对象的所有子物体和子组件完全复制，成为一个新的对象。这个新的对象拥有与源对象完全一样的东西，包括坐标值等。 
                //original：用来做复制操作的对像物体，源对象
                //position：实例化的对象的位置坐标
                //rotation：实例化的对象的旋转坐标（旋转四元数）
                Destroy(go, 3);//3s后自动销毁
                hit.transform.GetComponent<fps_EnemyHealth>().TakeDamage(damage);//获取敌人生命值组件，调用伤害结算的方法，将伤害传递进去
            }
        }
    }

    private void PlayerStateAnim(string animName)//状态动画
    {
        if(!anim.IsPlaying(animName))//如果当前播放的动画不是要切换到的动画
        {
            anim.Rewind(animName);//播放对应的动画
            anim.Play(animName);
        }
    }

    private void StateAnim(PlayerState state)//判断不同状态
    {
        switch (state)
        {
            case PlayerState.Idle:
                PlayerStateAnim(idleAnim);
                break;
            case PlayerState.Walk:
                PlayerStateAnim(walkAnim);
                break;
            case PlayerState.Crouch:
                PlayerStateAnim(walkAnim);
                break;
            case PlayerState.Run:
                PlayerStateAnim(runAnim);
                break;
        }
    }
}