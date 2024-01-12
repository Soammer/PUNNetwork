using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviourPun, IPunObservable
{
    //玩家组件
    private Rigidbody2D RB;
    private SpriteRenderer SR;
    private Animator animator;
    public TMP_Text NameText;
    public Image HealthBar;

    [Header("玩家属性")]
    public float MoveSpeed = 5;
    private float MoveX;
    private float MoveY;

    [Header("玩家当前状态")]
    public bool canMove = true;
    public bool canAttack = true;
    private int Health = 5;

    [Header("玩家挂载物体")]
    public Projectile proj;

    //简单状态机
    private PlayerState state = PlayerState.Idle;
    //客户端单例
    public static PlayerController localPlayer;

    public void Awake()
    {
        if(photonView.IsMine) localPlayer = this;
    }

    public void Start()
    {
        //获取组件，初始化名称显示
        if (!RB) RB = GetComponent<Rigidbody2D>();
        if(!SR) SR = GetComponent<SpriteRenderer>();
        if (!animator) animator = GetComponent<Animator>();
        if(photonView.IsMine) NameText.text = PhotonNetwork.NickName;
        else NameText.text = photonView.Owner.NickName;
        //初始化相机显示
        
        if (Camera.main.TryGetComponent<CameraController>(out var _camera))
        {
            if (photonView.IsMine) _camera.SetTarget(transform);
        }
        else
        {
            Debug.LogError("Camera缺少CameraWork组件", this);
        }

        //初始化玩家数据
        Health = 5;
        HealthBar.fillAmount = 1;
    }


    public void Update()
    {
        if (Time.timeScale == 0) return;
        if (photonView.IsMine)
        {
            if (!PhotonNetwork.IsConnected) return;
            if (canMove)
            {
                MoveX = Input.GetAxisRaw("Horizontal") * MoveSpeed;
                MoveY = Input.GetAxisRaw("Vertical") * MoveSpeed;
                RB.velocity = new(MoveX, MoveY);
                SwitchState();
            }
            if (canAttack)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 attackPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    photonView.RPC(nameof(Fire), RpcTarget.AllViaServer, attackPos);
                }
            }
        }
        SetFlip();
    }

    /// <summary>
    /// 简单状态机的切换方法
    /// </summary>
    public void SwitchState()
    {
        if(state == PlayerState.Idle)
        {
            if (MoveX != 0 || MoveY != 0)
            {
                animator.SetBool("Moving", true);
                state = PlayerState.Move;
                return;
            }
                
        }
        else
        {
            if (MoveX == 0 && MoveY == 0)
            {
                animator.SetBool("Moving", false);
                state = PlayerState.Idle;
                return;
            }
            
        }
    }

    /// <summary>
    /// 更新转向
    /// </summary>
    public void SetFlip()
    {
        if(MoveX > 0) SR.flipX = false;
        else if(MoveX < 0) SR.flipX = true;
    }


    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
            Health -= damage;
            if (Health <= 0)
            {
                GameManager.instance.ShowMessage("你死了");
                GameManager.instance.LeaveRoom();
                HealthBar.fillAmount = 0;
                return;
            }
            HealthBar.fillAmount = (float)Health / 5;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(Health);
            stream.SendNext(MoveX);
        }
        else
        {
            // Network player, receive data
            int curHealth = (int)stream.ReceiveNext();
            if (Health != curHealth)
            {
                Health = curHealth;
                HealthBar.fillAmount = (float)Health / 5;
            }
            MoveX = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void Fire(Vector2 target, PhotonMessageInfo info)
    {
        Projectile new_proj = Instantiate(proj, transform.position, Quaternion.identity);
        new_proj.InitializeBullet(this, target);
    }
}

public enum PlayerState
{
    Idle,
    Move
}
