using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField, Header("移動速度")]
    float MoveSpeed = 5.0f;
    [SerializeField, Header("ジャンプ量")]
    float JumpPower = 10.0f;
    [SerializeField, Header("重力")]
    float Gravity = 2.0f;
    [SerializeField, Header("スティックのデッドエリア")]
    float DeadArea = 0.01f;
    [SerializeField, Header("プレイヤーの回転量")]
    float RotateSpeed = 5.0f;
    [SerializeField, Header("壁キックの待機時間")]
    float WallJumpCoolDown = 2.0f;
    [SerializeField, Header("壁に向かうレイの長さ")]
    float WallRayLength = 2.0f;
    [SerializeField, Header("壁にいるときの重力")]
    float WallGravity = 0.1f;

    [SerializeField, Header("GameCamera")]
    GameObject GameCamera;
    [SerializeField, Header("GroundCheck")]
    HitCheck GroundCheck;
    [SerializeField, Header("WallCheck")]
    HitCheck WallCheck;
    [SerializeField, Header("UnityChanモデル")]
    GameObject UnityChan;

    /// <summary>
    /// プレイヤーステート。
    /// </summary>
    enum PlayerState
    {
        enState_Idle,
        enState_Move,
        enState_Jump,
        enState_WallGround,
        enState_WallAir,
        enState_Damage,
        enState_Num,
    }

    /// <summary>
    /// ジャンプモード。
    /// </summary>
    enum JumpMode
    {
        enMode_None,
        enMode_Normal,
        enMode_Wall,
    }

    Rigidbody       m_rigidbody;                    //Rigidbody。
    Animator        m_animator;                     //アニメーター。
    Vector3         m_wallNormal;                   //這っている壁のベクトル。
    PlayerState     m_playerState;                  //プレイヤーステート。
    JumpMode        m_jumpMode;                     //ジャンプモード。
    float           m_wallJumpTimer = 0.0f;         //壁キックタイマー。
    float           m_gravityScale = 1.0f;          //重力の倍率。


    /// <summary>
    /// 動けるかどうかを取得。
    /// </summary>
    /// <returns></returns>
    public bool GetCanMove()
    {
        return m_playerState != PlayerState.enState_Damage &&
            m_playerState != PlayerState.enState_Num;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = UnityChan.GetComponent<Animator>();

        //重力をスクリプト上で付与するため、適用しない。
        m_rigidbody.useGravity = false;

        //変数を初期化。
        m_playerState = PlayerState.enState_Idle;
        m_jumpMode = JumpMode.enMode_None;
    }

    private void FixedUpdate()
    {
        MoveX();

        State();
    }

    // Update is called once per frame
    void Update()
    {
        MoveY();
    }

    /// <summary>
    /// X軸の移動処理。
    /// </summary>
    void MoveX()
    {
        Vector2 stickL = Vector2.zero;         //スティックの入力量。
        Vector3 moveDirection = Vector3.zero;      //移動方向。

        //移動の入力を取得。
        stickL.x = Input.GetAxisRaw("Horizontal");
        stickL.y = Input.GetAxisRaw("Vertical");

        //一定以上の傾きがあるなら、入力を反映。
        if (Mathf.Abs(stickL.x) > DeadArea)
        {
            moveDirection.x = stickL.x;
        }
        if (Mathf.Abs(stickL.y) > DeadArea)
        {
            moveDirection.z = stickL.y;
        }

        //カメラの前方向と右方向を取得。
        Vector3 forward = GameCamera.transform.forward;
        Vector3 right = GameCamera.transform.right;
        forward.y = 0.0f;
        right.y = 0.0f;

        right *= moveDirection.x;
        forward *= moveDirection.z;

        //カメラの方向に合わせて移動量を計算。
        Vector3 moveSpeed = Vector3.zero;
        moveSpeed += right + forward;

        //正規化して斜め移動が速くならないようにする。
        moveSpeed.Normalize();
        moveSpeed *= MoveSpeed;

        //移動させる。
        m_rigidbody.velocity = new Vector3(moveSpeed.x, m_rigidbody.velocity.y, moveSpeed.z);

        //スティック入力の大きい方の絶対値をアニメーションに設定。
        m_animator.SetFloat("MoveFlag", Mathf.Max(Mathf.Abs(stickL.x), Mathf.Abs(stickL.y)));

        if(moveDirection.sqrMagnitude > 0.0f)
        {
            //進行方向に向かって徐々に回転。
            Quaternion rotation = UnityChan.transform.rotation;
            rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(moveSpeed), RotateSpeed);
            UnityChan.transform.rotation = rotation;
        }
    }

    /// <summary>
    /// Y軸の移動処理。
    /// </summary>
    void MoveY()
    {
        if (m_wallJumpTimer >= 0.0f)
        {
            m_wallJumpTimer -= Time.deltaTime;

            Debug.Log(m_wallJumpTimer);
            if (m_jumpMode == JumpMode.enMode_Wall)
            {
                //前方向と逆方向へ力を加える。
                Vector3 jumpPower = m_wallNormal * 100.0f;
                m_rigidbody.AddForce(jumpPower, ForceMode.Acceleration);
            }
        }

        if (!GroundCheck.GetIsHitStay())
        {
            //重力を適用。
            m_rigidbody.AddForce(new Vector3(0.0f, Gravity * m_gravityScale, 0.0f), ForceMode.Force);
        }

        if(WallCheck.GetIsHitStay())
        {
            //重力を少し下げる。
            m_gravityScale = WallGravity;
        }
        else
        {
            m_gravityScale = 1.0f;
        }

        //着地したら。
        if (GroundCheck.GetIsHitEnter())
        {
            m_animator.SetBool("JumpFlag", false);
            m_jumpMode = JumpMode.enMode_None;
        }

        //動けない状態なら。
        if (!GetCanMove())
        {
            return;
        }

        //ジャンプボタンを押したら。
        if (Input.GetButtonDown("Jump"))
        {
            //壁に触れている かつ 地面に触れていないなら。
            if (WallCheck.GetIsHitStay() && !GroundCheck.GetIsHitStay() && m_wallJumpTimer < 0.0f)
            {
                m_jumpMode = JumpMode.enMode_Wall;
                Jump();
            }
            //地面に触れているなら。
            else if(GroundCheck.GetIsHitStay())
            {
                m_jumpMode = JumpMode.enMode_Normal;
                Jump();
            }
        }
    }

    /// <summary>
    /// ジャンプ処理。
    /// </summary>
    void Jump()
    {
        switch (m_jumpMode)
        {
            case JumpMode.enMode_Normal:
                //力を加える。
                m_rigidbody.AddForce(new Vector3(0.0f, JumpPower, 0.0f), ForceMode.Impulse);
                break;

            case JumpMode.enMode_Wall:
                //壁ジャンプトリガー。
                m_animator.SetTrigger("DoubleJumpTrigger");
                m_wallJumpTimer = WallJumpCoolDown;
                break;
        }

        //アニメーションフラグを設定。
        m_animator.SetBool("JumpFlag", true);

        //ジャンプステートへ遷移、
        m_playerState = PlayerState.enState_Jump;
    }

    /// <summary>
    /// ステート処理。
    /// </summary>
    void State()
    {
        switch (m_playerState)
        {
            case PlayerState.enState_Idle:
                ProcessCommonState();
                break;

            case PlayerState.enState_Move:
                ProcessMoveStateTransition();
                break;

            case PlayerState.enState_Jump:
            case PlayerState.enState_WallAir:
                ProcessJumpStateTransition();
                break;

            case PlayerState.enState_WallGround:
                ProcessCommonState();
                break;
        }
    }

    /// <summary>
    /// ステートの共通処理。
    /// </summary>
    void ProcessCommonState()
    {
        //壁に触れ始めたら。
        if (WallCheck.GetIsHitEnter())
        {
            //レイを飛ばす。
            RaycastHit hit;
            if (Physics.Raycast(WallCheck.transform.position, WallCheck.transform.forward, out hit, WallRayLength))
            {
                //プレイヤーの前方向をレイの法線と合わせる。
                m_wallNormal = hit.normal;

                //地面に触れているなら。
                if (GroundCheck.GetIsHitStay())
                {
                    Debug.Log("A");
                    m_playerState = PlayerState.enState_WallGround;
                }
                else
                {
                    Debug.Log("B");
                    m_playerState = PlayerState.enState_WallAir;
                }
            }
            Debug.DrawRay(WallCheck.transform.position, WallCheck.transform.forward, Color.red, 10.0f);
        }

        if (GroundCheck.GetIsHitStay())
        {
            m_playerState = PlayerState.enState_Jump;
        }

        m_playerState = PlayerState.enState_Idle;
    }

    /// <summary>
    /// 移動ステートの遷移処理。
    /// </summary>
    void ProcessMoveStateTransition()
    {
        ProcessCommonState();
    }

    /// <summary>
    /// ジャンプステートの遷移処理。
    /// </summary>
    void ProcessJumpStateTransition()
    {
        ProcessCommonState();
    }
}
