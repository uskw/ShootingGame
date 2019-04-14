using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//拾取钥匙的脚本
public class fps_KeyPickUp : MonoBehaviour
{
    public AudioClip keyGrab;//拾取钥匙的音效
    public int keyId;//当前钥匙的id

    private GameObject player;//玩家
    private fps_PlayerInventory playerInventory;

    void Start()//初始化
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerInventory = player.GetComponent<fps_PlayerInventory>();
    }

    void OnTriggerEnter(Collider other)//碰撞检测
    {
        if(other.gameObject == player)//如果进行碰撞的是玩家
        {
            AudioSource.PlayClipAtPoint(keyGrab, transform.position);//播放拾取音效
            playerInventory.AddKey(keyId);//添加当前的钥匙id
            Destroy(this.gameObject);//销毁自身
        }
    }
}