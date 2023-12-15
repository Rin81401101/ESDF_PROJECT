using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBullet : MonoBehaviour
{
    private GameObject m_targetObj;
    private float m_rotationSpeedRatio= 0.01f;
    private float m_moveSpeed = 10;
    private Vector3 m_targetPos;

    private void Start() {
        m_targetPos = Vector3.zero;
    }

    private void Update() {
        if (m_targetObj != null) {
            m_targetPos = m_targetObj.transform.position;
        } else {
            m_targetPos = Vector3.zero;
        }
        Vector3 dist=m_targetPos - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dist.normalized), m_rotationSpeedRatio*Time.deltaTime*50);

        transform.position += transform.forward * m_moveSpeed * Time.deltaTime;
    }


    public void SetTarget(GameObject targetObj) {
        m_targetObj = targetObj;
    }
    public void SetRotationSpeedRatio(float rotationSpeedRatio) {
        m_rotationSpeedRatio = rotationSpeedRatio;
    }

    public void SetMoveSpeed(float moveSpeed) {
        m_moveSpeed=moveSpeed;
    }
}
