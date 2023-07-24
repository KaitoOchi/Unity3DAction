using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField, Header("感度")]
    float sensityvity = 3.0f;
    [SerializeField, Header("カメラ上限")]
    Vector2 CameraLimit;

    [SerializeField, Header("GameCamera")]
    GameObject GameCamera;


    Quaternion m_characterRot;
    Quaternion m_cameraRot;

    // Start is called before the first frame update
    void Start()
    {
        //自身とカメラの回転を取得。
        m_cameraRot = GameCamera.transform.localRotation;
        m_characterRot = transform.localRotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float xRot = Input.GetAxisRaw("Horizontal2") * sensityvity;
        float yRot = Input.GetAxis("Vertical2") * sensityvity;

        Debug.Log(xRot);

        m_characterRot *= Quaternion.Euler(0.0f, xRot, 0.0f);
        m_cameraRot *= Quaternion.Euler(-yRot, 0.0f, 0.0f);

        ClampRotation();

        transform.localRotation = m_characterRot;
        //GameCamera.transform.localRotation = m_characterRot;

        //GameCamera.transform.LookAt(transform.position);
    }

    /// <summary>
    /// 回転の上限を設定。
    /// </summary>
    void ClampRotation()
    {
        m_cameraRot.x /= m_cameraRot.w;
        m_cameraRot.y /= m_cameraRot.w;
        m_cameraRot.z /= m_cameraRot.w;
        m_cameraRot.w = 1.0f;

        float angleX = Mathf.Atan(m_cameraRot.x) * Mathf.Rad2Deg * 2.0f;
        angleX = Mathf.Clamp(angleX, CameraLimit.x, CameraLimit.y);

        m_cameraRot.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);
    }
}
