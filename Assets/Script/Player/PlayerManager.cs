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
    private PlayerAnimationManager playerAnimationManager ;

    private Rigidbody rb;

    [Header("�A�j���[�^�["), SerializeField]
    private Animator animator;

    [Header("�ړ����x"), SerializeField]
    private float moveSpeed;

    [Header("�ړ����x���[�g"), SerializeField]
    private float moveSpeedRate = 0.0f;

    [Header("�W�����v��"), SerializeField]
    private float jumpPower;

    private bool isRolling = false;

    private bool isGroundTmp = false;

    [Header("ray�̒���"), SerializeField]
    private float rayLength;

    [Header("����attach�ӏ�"), SerializeField]
    private GameObject WeaponAttachParent;

    WeaponBase weapon;
    WeaponUI weaponUI;

    [SerializeField]
    Vector3 scale;

    //���ˊԊu
    private int fireInterval = 0;

    private void Awake()
    {
        playerInput = new PlayerInputAction();
        rb = GetComponent<Rigidbody>();
        weaponUI = WeaponManager.m_instance.m_weaponUI.GetComponent<WeaponUI>();

        weapon = WeaponManager.m_instance.AttachWeapon(WeaponAttachParent, "Missile"); 
        weapon.gameObject.transform.localScale = scale;

        //��]���@����̃x�N�g���ƌ������������̃x�N�g���ŊO��
        Vector3 rotateAxis = Vector3.Cross(weapon.gameObject.transform.forward, this.transform.forward);
        Debug.Log(rotateAxis);

        //��]�p�x ����̃x�N�g���ƌ������������̃x�N�g���œ���
        float rotateAngle = Vector3.Dot(weapon.gameObject.transform.forward, this.transform.forward);
        Debug.Log(rotateAngle);


        //�o������]���ɉ�]�p�x��^����
        weapon.gameObject.transform.rotation = Quaternion.AxisAngle(rotateAxis, rotateAngle);
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
        GravityManager.instance.gravityUpdate(rb);

        weaponUI.m_reloadUIMain.fillAmount = weapon.GetReloadRatio();
        weaponUI.m_reloadUIParent.SetActive(weapon.GetIsReload());

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
            playerAnimationManager.MoveAnimation(inputValue, inputValue.magnitude);

            //�v���C���[�ړ�
            Vector3 moveVelocity = new Vector3(
                inputValue.x * moveSpeed * moveSpeedRate * inputValue.magnitude * Time.deltaTime,
                0.0f,//�W�����v
                inputValue.y * moveSpeed * moveSpeedRate * inputValue.magnitude * Time.deltaTime);

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
            weapon.Shot();
        }

        if (playerInput.Player.Reload.triggered)
        {
            weapon.Reload();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Jump()
    {
        playerAnimationManager.IsJumpDownEnd();
        if (isGrounded())
        {
            //�X�e�B�b�N���͎擾
            Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();
            moveSpeedRate = 1.0f;

            //�O��ړ����܂��̓W�����v���͂�����W�����v
            if (playerInput.Player.Jump.triggered)
            {
                //���ړ������W�����v���͂����烍�[�����O
                if (Mathf.Abs(inputValue.x) > 0.2f)
                {
                    isRolling = true;
                }
                else
                {
                    playerAnimationManager.PlayJumpUpAnimation();
                }
            }

            if (playerAnimationManager.GetIsJumpUpEnd())
            {
                rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                if (isGroundTmp != isGrounded(true))
                {
                    Debug.Log("isGroundTmp" + isGroundTmp);
                    playerAnimationManager.SetIsJumpUpEnd(false);
                    playerAnimationManager.PlayJumpDownAnimation();
                }
            }
        }
        else
        {
            moveSpeedRate = 0.25f;
        }

        isGroundTmp = isGrounded();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Rolling()
    {
        if (isRolling)
        {
            Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();

            Vector3 velocity = new Vector3(
                inputValue.x,
                0.0f,
                inputValue.y);

            //���[�����O����
            if (inputValue.x != 0.0f)
            {
                rb.AddForce(velocity * 10.0f, ForceMode.VelocityChange);
                isRolling = false;
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
        return Physics.Raycast(ray, rayLength * lengthRate);
    }
}
