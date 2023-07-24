using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    bool m_isGround;        //�n�ʂɐG��Ă��邩�ǂ����B


    /// <summary>
    /// �n�ʂɐG��Ă��邩�擾�B
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
