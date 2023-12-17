using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class PlayerManager : MonoBehaviour
{
    //���͊Ǘ�
    private PlayerInputAction playerInput;

    [Header("�A�j���[�V�����}�l�[�W���["), SerializeField]
    private PlayerAnimationManager m_playerAnimationManager;

    private Rigidbody m_rb;

    [Header("�ړ����x"), SerializeField]
    private float m_moveSpeed;

    [Header("�ړ����x���[�g"), SerializeField]
    private float m_moveSpeedRate = 0.0f;

    [Header("�W�����v��"), SerializeField]
    private float m_jumpPower;

    private bool m_isRolling = false;
    private Vector3 m_rollingVelocity = Vector3.zero;

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

        //��]���擾
        Vector3 rotateAxis = Vector3.Cross(m_weapon.gameObject.transform.forward, this.transform.forward);

        //��]�p�x�擾
        float rotateAngle = Vector3.Dot(m_weapon.gameObject.transform.forward, this.transform.forward);

        //�o������]���ɉ�]�p�x��^����
        m_weapon.gameObject.transform.rotation = Quaternion.AngleAxis(rotateAngle, rotateAxis);
    }

    private void OnEnable()
    {
        //PlayerInputAction�L��
        playerInput.Enable();
    }

    private void OnDisable()
    {
        //PlayerInputAction����
        playerInput.Disable();
    }

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
    /// �ړ�����
    /// </summary>
    private void Move()
    {
        //�X�e�B�b�N���͎擾
        Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();


        if ((inputValue.sqrMagnitude != 0.0f) && !m_isRolling)
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
    /// �ˌ�/�����[�h����
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
    /// �W�����v����
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
                if (Mathf.Abs(inputValue.x) < 0.2f)
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
    /// ���[�����O����
    /// </summary>
    private void Rolling()
    {
        //�X�e�B�b�N���͎擾
        Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();
        if (playerInput.Player.Jump.triggered)
        {
            if (Mathf.Abs(inputValue.x) >= 0.2f && !m_isRolling)
            {
                m_isRolling = true;
                m_rollingVelocity = new Vector3(inputValue.x, 0.0f, inputValue.y);
                m_playerAnimationManager.PlayRollingAnimation();

            }
        }
    }

    /// <summary>
    /// �n�ʒ��n����
    /// </summary>
    /// <returns></returns>
    private bool isGrounded(bool isAnimation = false)
    {
        float lengthRate = 1.0f;
        if (isAnimation)
        {
            lengthRate = 2.0f;
        }

        Vector3 rayPos = transform.position + new Vector3(0.0f, +0.1f, 0.0f);
        Ray ray = new Ray(rayPos, Vector3.down);
        return Physics.Raycast(ray, m_rayLength * lengthRate);
    }

    /// <summary>
    /// ���W�b�h�{�f�B�擾
    /// </summary>
    /// <returns></returns>
    public Rigidbody GetRigidbody()
    {
        return m_rb;
    }

    /// <summary>
    /// ���[�����O�����擾
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRollingVelocity()
    {
        return m_rollingVelocity;
    }

    /// <summary>
    /// ���[�����O�t���O�ݒ�
    /// </summary>
    /// <param name="flag"></param>
    public void SetIsRolling(bool flag)
    {
        m_isRolling = flag;
    }

}
