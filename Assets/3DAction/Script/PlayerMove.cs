using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField, Header("�ړ����x")]
    float MoveSpeed = 5.0f;
    [SerializeField, Header("�W�����v��")]
    float JumpPower = 10.0f;
    [SerializeField, Header("�d��")]
    float Gravity = 2.0f;
    [SerializeField, Header("�X�e�B�b�N�̃f�b�h�G���A")]
    float DeadArea = 0.01f;
    [SerializeField, Header("�v���C���[�̉�]��")]
    float RotateSpeed = 5.0f;
    [SerializeField, Header("�ǃL�b�N�̑ҋ@����")]
    float WallJumpCoolDown = 2.0f;
    [SerializeField, Header("�ǂɌ��������C�̒���")]
    float WallRayLength = 2.0f;
    [SerializeField, Header("�ǂɂ���Ƃ��̏d��")]
    float WallGravity = 0.1f;

    [SerializeField, Header("GameCamera")]
    GameObject GameCamera;
    [SerializeField, Header("GroundCheck")]
    HitCheck GroundCheck;
    [SerializeField, Header("WallCheck")]
    HitCheck WallCheck;
    [SerializeField, Header("UnityChan���f��")]
    GameObject UnityChan;

    /// <summary>
    /// �v���C���[�X�e�[�g�B
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
    /// �W�����v���[�h�B
    /// </summary>
    enum JumpMode
    {
        enMode_None,
        enMode_Normal,
        enMode_Wall,
    }

    Rigidbody       m_rigidbody;                    //Rigidbody�B
    Animator        m_animator;                     //�A�j���[�^�[�B
    Vector3         m_wallNormal;                   //�����Ă���ǂ̃x�N�g���B
    PlayerState     m_playerState;                  //�v���C���[�X�e�[�g�B
    JumpMode        m_jumpMode;                     //�W�����v���[�h�B
    float           m_wallJumpTimer = 0.0f;         //�ǃL�b�N�^�C�}�[�B
    float           m_gravityScale = 1.0f;          //�d�͂̔{���B


    /// <summary>
    /// �����邩�ǂ������擾�B
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

        //�d�͂��X�N���v�g��ŕt�^���邽�߁A�K�p���Ȃ��B
        m_rigidbody.useGravity = false;

        //�ϐ����������B
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
    /// X���̈ړ������B
    /// </summary>
    void MoveX()
    {
        Vector2 stickL = Vector2.zero;         //�X�e�B�b�N�̓��͗ʁB
        Vector3 moveDirection = Vector3.zero;      //�ړ������B

        //�ړ��̓��͂��擾�B
        stickL.x = Input.GetAxisRaw("Horizontal");
        stickL.y = Input.GetAxisRaw("Vertical");

        //���ȏ�̌X��������Ȃ�A���͂𔽉f�B
        if (Mathf.Abs(stickL.x) > DeadArea)
        {
            moveDirection.x = stickL.x;
        }
        if (Mathf.Abs(stickL.y) > DeadArea)
        {
            moveDirection.z = stickL.y;
        }

        //�J�����̑O�����ƉE�������擾�B
        Vector3 forward = GameCamera.transform.forward;
        Vector3 right = GameCamera.transform.right;
        forward.y = 0.0f;
        right.y = 0.0f;

        right *= moveDirection.x;
        forward *= moveDirection.z;

        //�J�����̕����ɍ��킹�Ĉړ��ʂ��v�Z�B
        Vector3 moveSpeed = Vector3.zero;
        moveSpeed += right + forward;

        //���K�����Ď΂߈ړ��������Ȃ�Ȃ��悤�ɂ���B
        moveSpeed.Normalize();
        moveSpeed *= MoveSpeed;

        //�ړ�������B
        m_rigidbody.velocity = new Vector3(moveSpeed.x, m_rigidbody.velocity.y, moveSpeed.z);

        //�X�e�B�b�N���͂̑傫�����̐�Βl���A�j���[�V�����ɐݒ�B
        m_animator.SetFloat("MoveFlag", Mathf.Max(Mathf.Abs(stickL.x), Mathf.Abs(stickL.y)));

        if(moveDirection.sqrMagnitude > 0.0f)
        {
            //�i�s�����Ɍ������ď��X�ɉ�]�B
            Quaternion rotation = UnityChan.transform.rotation;
            rotation = Quaternion.RotateTowards(rotation, Quaternion.LookRotation(moveSpeed), RotateSpeed);
            UnityChan.transform.rotation = rotation;
        }
    }

    /// <summary>
    /// Y���̈ړ������B
    /// </summary>
    void MoveY()
    {
        if (m_wallJumpTimer >= 0.0f)
        {
            m_wallJumpTimer -= Time.deltaTime;

            Debug.Log(m_wallJumpTimer);
            if (m_jumpMode == JumpMode.enMode_Wall)
            {
                //�O�����Ƌt�����֗͂�������B
                Vector3 jumpPower = m_wallNormal * 100.0f;
                m_rigidbody.AddForce(jumpPower, ForceMode.Acceleration);
            }
        }

        if (!GroundCheck.GetIsHitStay())
        {
            //�d�͂�K�p�B
            m_rigidbody.AddForce(new Vector3(0.0f, Gravity * m_gravityScale, 0.0f), ForceMode.Force);
        }

        if(WallCheck.GetIsHitStay())
        {
            //�d�͂�����������B
            m_gravityScale = WallGravity;
        }
        else
        {
            m_gravityScale = 1.0f;
        }

        //���n������B
        if (GroundCheck.GetIsHitEnter())
        {
            m_animator.SetBool("JumpFlag", false);
            m_jumpMode = JumpMode.enMode_None;
        }

        //�����Ȃ���ԂȂ�B
        if (!GetCanMove())
        {
            return;
        }

        //�W�����v�{�^������������B
        if (Input.GetButtonDown("Jump"))
        {
            //�ǂɐG��Ă��� ���� �n�ʂɐG��Ă��Ȃ��Ȃ�B
            if (WallCheck.GetIsHitStay() && !GroundCheck.GetIsHitStay() && m_wallJumpTimer < 0.0f)
            {
                m_jumpMode = JumpMode.enMode_Wall;
                Jump();
            }
            //�n�ʂɐG��Ă���Ȃ�B
            else if(GroundCheck.GetIsHitStay())
            {
                m_jumpMode = JumpMode.enMode_Normal;
                Jump();
            }
        }
    }

    /// <summary>
    /// �W�����v�����B
    /// </summary>
    void Jump()
    {
        switch (m_jumpMode)
        {
            case JumpMode.enMode_Normal:
                //�͂�������B
                m_rigidbody.AddForce(new Vector3(0.0f, JumpPower, 0.0f), ForceMode.Impulse);
                break;

            case JumpMode.enMode_Wall:
                //�ǃW�����v�g���K�[�B
                m_animator.SetTrigger("DoubleJumpTrigger");
                m_wallJumpTimer = WallJumpCoolDown;
                break;
        }

        //�A�j���[�V�����t���O��ݒ�B
        m_animator.SetBool("JumpFlag", true);

        //�W�����v�X�e�[�g�֑J�ځA
        m_playerState = PlayerState.enState_Jump;
    }

    /// <summary>
    /// �X�e�[�g�����B
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
    /// �X�e�[�g�̋��ʏ����B
    /// </summary>
    void ProcessCommonState()
    {
        //�ǂɐG��n�߂���B
        if (WallCheck.GetIsHitEnter())
        {
            //���C���΂��B
            RaycastHit hit;
            if (Physics.Raycast(WallCheck.transform.position, WallCheck.transform.forward, out hit, WallRayLength))
            {
                //�v���C���[�̑O���������C�̖@���ƍ��킹��B
                m_wallNormal = hit.normal;

                //�n�ʂɐG��Ă���Ȃ�B
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
    /// �ړ��X�e�[�g�̑J�ڏ����B
    /// </summary>
    void ProcessMoveStateTransition()
    {
        ProcessCommonState();
    }

    /// <summary>
    /// �W�����v�X�e�[�g�̑J�ڏ����B
    /// </summary>
    void ProcessJumpStateTransition()
    {
        ProcessCommonState();
    }
}
