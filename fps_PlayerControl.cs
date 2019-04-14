using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState//通过枚举来表示玩家当前的状态，可以根据对应的状态来实现武器的动画
{
    None,
    Idle,
    Walk,
    Crouch,
    Run,
}

//玩家的控制脚本
public class fps_PlayerControl : MonoBehaviour
{
    private PlayerState state = PlayerState.None;//用户的状态默认值为None

    public PlayerState State
    {
        get//通过自定义标识位，来对状态进行赋值
        {
            if (runing)
                state = PlayerState.Run;
            else if (walking)
                state = PlayerState.Walk;
            else if (crouching)
                state = PlayerState.Crouch;
            else
                state = PlayerState.Idle;
            return state;
        }
    }

    //各种不同状态下玩家的速度
    public float sprintSpeed = 10.0f;//冲刺
    public float sprintJumpSpeed = 8.0f;//冲刺+跳
    public float normalSpeed = 6.0f;//正常移动
    public float normalJumpSpeed = 7.0f;//正常移动+跳
    public float crouchSpeed = 2.0f;//蹲伏
    public float crouchJumpSpeed = 5.0f;//蹲伏+跳

    public float crouchDeltaHeight = 0.5f;//在蹲伏下，角色控制器需要下降多高的高度（增量）

    public float gravity = 20.0f;//重力加速度
    public float cameraMoveSpeed = 8.0f;//相机的移动速度（渐变过程），用来表示下降或升高
    public AudioClip jumpAudio;//跳跃的声音

    private float speed;//代表玩家当前的移动速度
    private float jumpSpeed;//代表玩家当前的跳跃速度
    private Transform mainCamera;//改变相机的高度
    private float standardCamHeight;//相机的标准高度
    private float crouchingCamHeight;//蹲伏时相机的高度

    //记录玩家是否处于对应的状态
    private bool grounded = false;//表示玩家是否在地面上
    private bool walking = false;//表示玩家是否在走
    private bool crouching = false;//是否在蹲伏
    private bool stopCrouching = false;//是否停止蹲伏
    private bool runing = false;//是否在跑

    //记录玩家当前角色控制器的高度
    private Vector3 normalControllerCenter = Vector3.zero;//普通高度（中心点）
    private float normalControllerHeight = 0.0f;//蹲伏高度

    private float timer = 0;//自定义计时器
    private CharacterController controller;//角色控制器
    private AudioSource audioSource;//播放角色脚步声
    private fps_PlayerParameter parameter;//输入参数

    private Vector3 moveDirection = Vector3.zero;//移动时的辅助参数，默认为0

    void Start()//初始化
    {
        //将所有的标识位重置为false
        crouching = false;
        walking = false;
        runing = false;

        //初始化当前速度
        speed = normalSpeed;
        jumpSpeed = normalJumpSpeed;

        //相机的获取
        mainCamera = GameObject.FindGameObjectWithTag(Tags.mainCamera).transform;

        //存储相机所有的高度
        standardCamHeight = mainCamera.localPosition.y;//标准高度
        crouchingCamHeight = standardCamHeight - crouchDeltaHeight;//蹲伏时的高度

        audioSource = this.GetComponent<AudioSource>();
        controller = this.GetComponent<CharacterController>();
        parameter = this.GetComponent<fps_PlayerParameter>();//获取用户的输入

        //角色控制器的高度，根据是否为蹲伏状态来回切换。保存正常状态下的高度
        normalControllerCenter = controller.center;
        normalControllerHeight = controller.height;
    }

    public void FixedUpdate()//调用移动和播放音效的方法
    {
        UpdateMove();
        AudioManagement();
    }

    private void UpdateMove()//移动的方法
    {
        if(grounded)//玩家在地面上，通过键位进行计算方向
        {
            moveDirection = new Vector3(parameter.inputMoveVector.x, 0, parameter.inputMoveVector.y);
            //获取输入轴的方向，是相对于世界坐标系的方向
            //对应移动的方向：按键取x轴和y轴的输入，实际移动对应世界坐标系的x轴和z轴（y轴竖直方向没有移动）

            moveDirection = transform.TransformDirection(moveDirection);//将方向转变为玩家自身坐标系的方向
            //因为鼠标可以旋转，如果旋转了，方向需要根据旋转的方向改变
            moveDirection *= speed;//乘以移动速度

            if(parameter.inputJump)//用户输入为跳跃，只有在玩家处于地面时才可以跳跃
            {
                moveDirection.y = jumpSpeed;//将y轴设置为跳跃速度
                AudioSource.PlayClipAtPoint(jumpAudio, transform.position);//播放跳跃的音效
                CurrentSpeed();//为当前移动速度赋值
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;//移动是要下落的，表示重力，如果落到地面，则不进行下落
        CollisionFlags flag = controller.Move(moveDirection * Time.deltaTime);//让玩家移动
        //返回值代表碰撞信息，接下来需要进行判断
        grounded = (flag & CollisionFlags.CollidedBelow) != 0;//!= 0，说明碰撞器与地面接触了

        if (Mathf.Abs(moveDirection.x)>0 && grounded || Mathf.Abs(moveDirection.z)>0 && grounded)//判断玩家的状态
        //表示玩家在地面且有按键输入
        {
            if(parameter.inputSprint)//判断输入是否为冲刺
            {
                walking = false;
                runing = true;
                crouching = false;
            }
            else if(parameter.inputCrouch)
            {
                crouching = true;
                walking = false;
                runing = false;
            }
            else//默认为walking，因为有按键输入至少是移动状态
            {
                walking = true;
                crouching = false;
                runing = false;
            }
        }
        else//没有按键输入的情况下，也可以进行蹲伏
        {
            if (walking)
                walking = false;
            if (runing)
                runing = false;
            if (parameter.inputCrouch)//输入蹲伏
                crouching = true;
            else
                crouching = false;
        }

        if(crouching)//改变角色控制器的碰撞器的高度和中心点
        {
            controller.height = normalControllerHeight - crouchDeltaHeight;//标准高度减去增量，即为蹲伏高度
            controller.center = normalControllerCenter - new Vector3(0, crouchDeltaHeight/2, 0);
            //中心点高度下降为为一半
        }
        else//没有蹲伏
        {
            controller.height = normalControllerHeight;
            controller.center = normalControllerCenter;
        }
        UpdateCrouch();//更新相机位置
        CurrentSpeed();//给速度赋值
    }

    private void CurrentSpeed()//根据当前玩家的状态，为速度赋值
    {
        switch (State)
        {
            case PlayerState.Idle:
                speed = normalSpeed;
                jumpSpeed = normalJumpSpeed;
                break;
            case PlayerState.Walk:
                speed = normalSpeed;
                jumpSpeed = normalJumpSpeed;
                break;
            case PlayerState.Crouch:
                speed = crouchSpeed;
                jumpSpeed = crouchJumpSpeed;
                break;
            case PlayerState.Run:
                speed = sprintSpeed;
                jumpSpeed = sprintJumpSpeed;
                break;
        }
    }

    public void AudioManagement()//对玩家的脚步声的管理
    {
        if(State == PlayerState.Walk)//在走
        {
            audioSource.pitch = 1.0f;
            if (!audioSource.isPlaying)//如果没有处于播放状态
                audioSource.Play();//播放
        }
        else if(State == PlayerState.Run)//在跑
        {
            audioSource.pitch = 1.3f;//脚步声要急促一点
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else//其他状态停止播放声音
        {
            audioSource.Stop();
        }
    }

    private void UpdateCrouch()//对相机的封装（站立和蹲伏时相机的移动）
    {
        if(crouching)//是蹲伏状态
        {
            if(mainCamera.localPosition.y > crouchingCamHeight)//当前相机的高度大于蹲伏时相机高度，说明相机需要下降
            {
                if (mainCamera.localPosition.y - (crouchDeltaHeight * Time.deltaTime * cameraMoveSpeed) < crouchingCamHeight)
                    //判断当前帧下降一定高度后是否会低于蹲伏时相机高度，如果低于，直接设置为蹲伏时相机高度；如果不低于，则减去蹲伏时相机高度
                    mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, crouchingCamHeight, mainCamera.localPosition.z);
                else
                    mainCamera.localPosition -= new Vector3(0, crouchDeltaHeight * Time.deltaTime * cameraMoveSpeed, 0);
            }
            else//当前相机的高度小于等于蹲伏高度，直接让相机高度等于蹲伏高度即可
                mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, crouchingCamHeight, mainCamera.localPosition.z);
        }
        else//不是蹲伏状态
        {
            if(mainCamera.localPosition.y < standardCamHeight)//如果当前相机高度小于相机的标准高度
            {
                if (mainCamera.localPosition.y + (crouchDeltaHeight * Time.deltaTime * cameraMoveSpeed) > standardCamHeight)
                    //如果当前相机高度加上蹲伏高度超过了标准高度
                    //速度不是秒，是每一帧增量的速度
                    mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, standardCamHeight, mainCamera.localPosition.z);
                    //让当前高度等于标准高度
                else//否则加上蹲伏高度
                    mainCamera.localPosition += new Vector3(0, crouchDeltaHeight * Time.deltaTime * cameraMoveSpeed, 0);
            }
            else//当前相机高度大于等于标准高度，则直接设置为标准高度
                mainCamera.localPosition = new Vector3(mainCamera.localPosition.x, standardCamHeight, mainCamera.localPosition.z);
        }
    }
}