using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//控制门的滑动的脚本
public class fps_DoorControl : MonoBehaviour
{
    public int doorId;//门的id（只有门和钥匙的id相匹配，才能打开）

    //实现门打开和关闭的动画（通过位置的改变）
    public Vector3 from;//门的初始位置
    public Vector3 to;//滑动之后的位置

    public float fadeSpeed = 5;//门打开和关闭动画渐变的速度
    public bool requireKey = false;//当前门的打开是否需要钥匙
    public AudioClip doorSwitchClip;//门打开或关闭时要播放的音效
    public AudioClip accessDeniedClip;//当前门不允许打开时播放的音效

    private Transform door;//门
    private GameObject player;//玩家
    private AudioSource audioSources;//播放音效
    private fps_PlayerInventory playerInventory;//存储玩家钥匙信息
    private int count;//代表在门的附近，触发器之内，人物的数量

    public int Count//封装Count变量
    {
        get { return count; }
        set
        {
            if(count == 0 && value == 1 || count == 1 && value == 0)//value为输入的值
                //无论数量是从0到1还是从1到0，都要进行门的打开或关闭操作
                //从0到1：门的周围有人，需要自动打开
                //从1到0：没有人，门要自动关闭
            {
                audioSources.clip = doorSwitchClip;//门在切换时需要播放的音效
                audioSources.Play();
            }
            count = value;//给count赋值
        }
    }

    void Start()//初始化
    {
        if (transform.childCount > 0)//transform.childCount：获取子物体数量
            door = transform.GetChild(0);//获取子物体
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerInventory = player.GetComponent<fps_PlayerInventory>();
        audioSources = this.GetComponent<AudioSource>();
        door.localPosition = from;//设置门的初始位置
    }

    void Update()
    {
        if (Count > 0)//如果有人
            door.localPosition = Vector3.Lerp(door.localPosition, to, fadeSpeed * Time.deltaTime);
            //门打开
        else//否则门关闭
            door.localPosition = Vector3.Lerp(door.localPosition, from, fadeSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)//触发函数（如果门周围是玩家或是敌人）
    {
        if (other.gameObject == player)//如果是玩家
        {
            if (requireKey)//如果当前的门打开需要钥匙
            {
                if (playerInventory.HasKey(doorId))//如果玩家拥有的钥匙和门的id一致
                    Count++;//人物数量加一
                            //用Count是因为Count中有set，可以进行赋值
                else//如果不一致
                {
                    audioSources.clip = accessDeniedClip;//播放不允许打开门的音效
                    audioSources.Play();
                }
            }
            else//如果不需要钥匙
                Count++;//人物数量加一
        }
        else if (other.gameObject.tag == Tags.enemy && other is CapsuleCollider)
            //说明触发门的是敌人且发生碰撞的碰撞对象必须是胶囊碰撞器才可以打开（因为敌人还有侦听触发器，这个不能打开）
            Count++;
    }

    void OnTriggerExit(Collider other)//退出触发函数
    {
        if (other.gameObject == player || (other.gameObject.tag == Tags.enemy && other is CapsuleCollider))
            //如果对象是玩家，或者是敌人并且是胶囊碰撞器
            Count = Mathf.Max(0, Count-1);//限制大小
    }
}