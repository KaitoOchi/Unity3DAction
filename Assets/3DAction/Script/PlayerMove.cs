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

    [SerializeField, Header("GameCamera")]
    GameObject GameCamera;
    [SerializeField, Header("GroundCheck")]
    GroundCheck IsGround;

    Rigidbody m_rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        //�d�͂��X�N���v�g��ŕt�^���邽�߁A�K�p���Ȃ��B
        m_rigidbody.useGravity = false;
    }

    private void FixedUpdate()
    {
        MoveX();
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
        Vector2 moveDirection = Vector2.zero;      //�ړ������B

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
            moveDirection.y = stickL.y;
        }

        //�J�����̑O�����ƉE�������擾�B
        Vector3 forward = GameCamera.transform.forward;
        Vector3 right = GameCamera.transform.right;
        forward.y = 0.0f;
        right.y = 0.0f;

        right *= moveDirection.x;
        forward *= moveDirection.y;

        //�J�����̕����ɍ��킹�Ĉړ��ʂ��v�Z�B
        Vector3 moveSpeed = Vector3.zero;
        moveSpeed += right + forward;

        //���K�����Ď΂߈ړ��������Ȃ�Ȃ��悤�ɂ���B
        moveSpeed.Normalize();
        moveSpeed *= MoveSpeed;

        m_rigidbody.velocity = new Vector3(moveSpeed.x, m_rigidbody.velocity.y, moveSpeed.z);
    }

    /// <summary>
    /// Y���̈ړ������B
    /// </summary>
    void MoveY()
    {
        if (!IsGround.GetIsGround())
        {
            //�d�͂�K�p�B
            m_rigidbody.AddForce(new Vector3(0.0f, Gravity, 0.0f), ForceMode.Force);
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_rigidbody.AddForce(new Vector3(0.0f, JumpPower, 0.0f), ForceMode.Impulse);
        }
    }
}
