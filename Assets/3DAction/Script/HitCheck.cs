using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCheck : MonoBehaviour
{
    [SerializeField, Header("接地判定を行うタグ")]
    string TagName = "";

    bool m_isHitEnter = false;       //触れ始めたかどうか。
    bool m_isHitStay = false;        //触れているかどうか。

    /// <summary>
    /// 触れ始めたかどうか。
    /// </summary>
    /// <returns></returns>
    public bool GetIsHitEnter()
    {
        return m_isHitEnter;
    }

    /// <summary>
    /// 触れているか取得。
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
