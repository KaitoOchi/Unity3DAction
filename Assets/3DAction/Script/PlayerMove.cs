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

    [SerializeField, Header("GameCamera")]
    GameObject GameCamera;
    [SerializeField, Header("GroundCheck")]
    GroundCheck IsGround;

    Rigidbody m_rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        //重力をスクリプト上で付与するため、適用しない。
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
    /// X軸の移動処理。
    /// </summary>
    void MoveX()
    {
        Vector2 stickL = Vector2.zero;         //スティックの入力量。
        Vector2 moveDirection = Vector2.zero;      //移動方向。

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
            moveDirection.y = stickL.y;
        }

        //カメラの前方向と右方向を取得。
        Vector3 forward = GameCamera.transform.forward;
        Vector3 right = GameCamera.transform.right;
        forward.y = 0.0f;
        right.y = 0.0f;

        right *= moveDirection.x;
        forward *= moveDirection.y;

        //カメラの方向に合わせて移動量を計算。
        Vector3 moveSpeed = Vector3.zero;
        moveSpeed += right + forward;

        //正規化して斜め移動が速くならないようにする。
        moveSpeed.Normalize();
        moveSpeed *= MoveSpeed;

        m_rigidbody.velocity = new Vector3(moveSpeed.x, m_rigidbody.velocity.y, moveSpeed.z);
    }

    /// <summary>
    /// Y軸の移動処理。
    /// </summary>
    void MoveY()
    {
        if (!IsGround.GetIsGround())
        {
            //重力を適用。
            m_rigidbody.AddForce(new Vector3(0.0f, Gravity, 0.0f), ForceMode.Force);
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_rigidbody.AddForce(new Vector3(0.0f, JumpPower, 0.0f), ForceMode.Impulse);
        }
    }
}
