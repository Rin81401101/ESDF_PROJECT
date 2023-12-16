using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    //���͊Ǘ�
    private PlayerInputAction playerInput;

    [Header("�A�j���[�V�����}�l�[�W���["), SerializeField]
    private PlayerAnimationManager m_playerAnimationManager ;

    private Rigidbody m_rb;

    [Header("�ړ����x"), SerializeField]
    private float m_moveSpeed;

    [Header("�ړ����x���[�g"), SerializeField]
    private float m_moveSpeedRate = 0.0f;

    [Header("�W�����v��"), SerializeField]
    private float m_jumpPower;

    private bool m_isRolling = false;

    private bool m_isGroundTmp = false;

    [Header("ray�̒���"), SerializeField]
    private float m_rayLength;

    [Header("����A�^�b�`�ӏ�"), SerializeField]
    private GameObject m_weaponAttachParent;

    private WeaponBase m_weapon;
    private WeaponUI m_weaponUI;

    [SerializeField]
    Vector3 m_weaponScale;

    private void Awake()
    {
        playerInput = new PlayerInputAction();
        m_rb = GetComponent<Rigidbody>();
        m_weaponUI = WeaponManager.m_instance.m_weaponUI.GetComponent<WeaponUI>();

        m_weapon = WeaponManager.m_instance.AttachWeapon(m_weaponAttachParent, "Missile");
        m_weapon.gameObject.transform.localScale = m_weaponScale;

        //��]���@����̃x�N�g���ƌ������������̃x�N�g���ŊO��
        Vector3 rotateAxis = Vector3.Cross(m_weapon.gameObject.transform.forward, this.transform.forward);
        Debug.Log(rotateAxis);

        //��]�p�x ����̃x�N�g���ƌ������������̃x�N�g���œ���
        float rotateAngle = Vector3.Dot(m_weapon.gameObject.transform.forward, this.transform.forward);
        Debug.Log(rotateAngle);


        //�o������]���ɉ�]�p�x��^����
        m_weapon.gameObject.transform.rotation = Quaternion.AxisAngle(rotateAxis, rotateAngle);
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        GravityManager.m_instance.gravityUpdate(m_rb);

        m_weaponUI.m_reloadUIMain.fillAmount = m_weapon.GetReloadRatio();
        m_weaponUI.m_reloadUIParent.SetActive(m_weapon.GetIsReload());

        //�ړ�
        Move();

        //�W�����v
        Jump();

        //�ˌ�
        Fire();

        //���[�����O
        Rolling();

        //���ړ����W�����v�����[�����O
        //�O��ړ��܂��̓W�����v�P�́��W�����v
    }

    /// <summary>
    /// 
    /// </summary>
    private void Move()
    {
        //�X�e�B�b�N���͎擾
        Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();


        if (inputValue.sqrMagnitude != 0.0f)
        {
            //�v���C���[�ړ��A�j���[�V����
            m_playerAnimationManager.MoveAnimationBlend(inputValue, inputValue.magnitude);

            //�v���C���[�ړ�
            Vector3 moveVelocity = new Vector3(
                inputValue.x * m_moveSpeed * m_moveSpeedRate * inputValue.magnitude * Time.deltaTime,
                0.0f,//�W�����v
                inputValue.y * m_moveSpeed * m_moveSpeedRate * inputValue.magnitude * Time.deltaTime);

            transform.position += moveVelocity;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Fire()
    {

        //�ˌ�����(�g���K�[���͂ɗ]�T����������)
        if (playerInput.Player.Fire.ReadValue<float>() > 0.3f)
        {
            m_weapon.Shot();
        }

        if (playerInput.Player.Reload.triggered)
        {
            m_weapon.Reload();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Jump()
    {
        m_playerAnimationManager.StopJumpDownAnimation();
        if (isGrounded())
        {
            //�X�e�B�b�N���͎擾
            Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();
            m_moveSpeedRate = 1.0f;

            //�O��ړ����܂��̓W�����v���͂�����W�����v
            if (playerInput.Player.Jump.triggered)
            {
                //���ړ������W�����v���͂����烍�[�����O
                if (Mathf.Abs(inputValue.x) > 0.2f)
                {
                    m_isRolling = true;
                }
                else
                {
                    m_playerAnimationManager.PlayJumpUpAnimation();
                }
            }

            if (m_playerAnimationManager.GetIsJumpUpEnd())
            {
                m_rb.AddForce(Vector3.up * m_jumpPower, ForceMode.VelocityChange);
                if (m_isGroundTmp != isGrounded(true))
                {
                    m_playerAnimationManager.SetIsJumpUpEnd(false);
                    m_playerAnimationManager.PlayJumpDownAnimation();
                }
            }
        }
        else
        {
            m_moveSpeedRate = 0.25f;
        }

        m_isGroundTmp = isGrounded();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Rolling()
    {
        if (m_isRolling)
        {
            Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();

            Vector3 velocity = new Vector3(
                inputValue.x,
                0.0f,
                inputValue.y);

            //���[�����O����
            if (inputValue.x != 0.0f)
            {
                m_rb.AddForce(velocity * 10.0f, ForceMode.VelocityChange);
                m_isRolling = false;
            }

        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool isGrounded(bool isAnimation = false)
    {
        float lengthRate = 1.0f;
        if(isAnimation)
        {
            lengthRate = 2.0f;
        }

        Vector3 rayPos = transform.position + new Vector3(0.0f, +0.1f, 0.0f);
        Ray ray = new Ray(rayPos, Vector3.down);
        return Physics.Raycast(ray, m_rayLength * lengthRate);
    }
}
