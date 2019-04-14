using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//代表玩家背包内容的脚本
public class fps_PlayerInventory : MonoBehaviour
{
    //通过集合对钥匙卡id的存储，通过id号进行钥匙和门的匹配

    private List<int> keysArr;//钥匙卡的集合

    void Start()
    {
        keysArr = new List<int>();//初始化
    }

    public void AddKey(int keyId)//拾取时添加钥匙卡
    {
        if (!keysArr.Contains(keyId))//如果集合中不包含此钥匙卡
            keysArr.Add(keyId);//添加进集合
    }

    public bool HasKey(int doorId)//判断用户是否拥有钥匙卡，用来判断用户可否开启当前门
    {
        if (keysArr.Contains(doorId))//如果拥有
            return true;
        return false;
    }
}