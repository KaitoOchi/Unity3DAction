using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    bool m_isGround;        //地面に触れているかどうか。


    /// <summary>
    /// 地面に触れているか取得。
    /// </summary>
    public bool GetIsGround()
    {
        return m_isGround;
    }

    private void FixedUpdate()
    {
        m_isGround = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            m_isGround = true;
        }
    }
}
