using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimManager : MonoBehaviour
{

    [SerializeField]
    private Transform m_spineTransform;

    private PlayerManager m_playerManager;

    private PlayerInputAction m_inputAciton;

    private Quaternion m_aa;

    private float m_value;
    private float m_value2;

    private Vector3 m_defaultRight;

    // Start is called before the first frame update
    void Start()
    {
        m_defaultRight = m_spineTransform.right;
        m_value = 0.0f;
        m_value2 = 0.0f;
        m_playerManager = GetComponent<PlayerManager>();
        m_inputAciton = m_playerManager.getPlayerInputAction();
    }

    // Update is called once per frame


    private void LateUpdate()
    {
        Vector2 inputValue = m_inputAciton.Player.Aim.ReadValue<Vector2>();
        m_value += inputValue.x;
        m_value2 += inputValue.y;

        Vector3 aa = new Vector3();
        Vector3 newRight = m_spineTransform.right - m_defaultRight + Vector3.right;
        newRight.y = 0;
        aa = Vector3.up * m_value + newRight.normalized * m_value2;

        Vector2 vec = new Vector2(m_value, m_value2);

        //// x�������ɂ��Ė��b2�x�A��]������Quaternion���쐬�i�ϐ���rot�Ƃ���j
        Quaternion rot = Quaternion.AngleAxis(vec.magnitude, aa.normalized);


        //// ���݂̎��M�̉�]�̏����擾����B
        Quaternion q = m_spineTransform.transform.rotation;
        //// �������āA���g�ɐݒ�
        m_spineTransform.rotation = q * rot;

        //Debug.Log(m_spineTransform.rotation);
    }


}
