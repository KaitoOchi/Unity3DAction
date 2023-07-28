using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCheck : MonoBehaviour
{
    [SerializeField, Header("�ڒn������s���^�O")]
    string TagName = "";

    bool m_isHitEnter = false;       //�G��n�߂����ǂ����B
    bool m_isHitStay = false;        //�G��Ă��邩�ǂ����B

    /// <summary>
    /// �G��n�߂����ǂ����B
    /// </summary>
    /// <returns></returns>
    public bool GetIsHitEnter()
    {
        return m_isHitEnter;
    }

    /// <summary>
    /// �G��Ă��邩�擾�B
    /// </summary>
    public bool GetIsHitStay()
    {
        return m_isHitStay;
    }

    private void FixedUpdate()
    {
        m_isHitEnter = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagName))
        {
            m_isHitEnter = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag(TagName))
        {
            m_isHitStay = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagName))
        {
            m_isHitStay = false;
        }
    }
}
