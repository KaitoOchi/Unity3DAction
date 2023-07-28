using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField, Header("感度")]
    float sensityvity = 3.0f;
    [SerializeField, Header("カメラ上限")]
    Vector2 CameraLimit;

    Quaternion m_characterRot;

    // Start is called before the first frame update
    void Start()
    {
        //自身の回転を取得。
        m_characterRot = transform.localRotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float xRot = Input.GetAxisRaw("Horizontal2") * sensityvity;
        float yRot = Input.GetAxis("Vertical2") * sensityvity;

        m_characterRot *= Quaternion.Euler(-yRot, xRot, 0.0f);

        ClampRotation();

        transform.localRotation = m_characterRot;
    }

    /// <summary>
    /// 回転の上限を設定。
    /// </summary>
    void ClampRotation()
    {
        m_characterRot.x /= m_characterRot.w;
        m_characterRot.y /= m_characterRot.w;
        m_characterRot.z /= m_characterRot.w;
        m_characterRot.w = 1.0f;

        float angleX = Mathf.Atan(m_characterRot.x) * Mathf.Rad2Deg * 2.0f;
        angleX = Mathf.Clamp(angleX, CameraLimit.x, CameraLimit.y);

        m_characterRot.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);
    }
}
