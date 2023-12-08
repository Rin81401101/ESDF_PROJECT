using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    //���͊Ǘ�
    private PlayerInputAction playerInput;

    private Rigidbody rigidbody;

    [Header("�A�j���[�^�["), SerializeField]
    private Animator animator;

    [Header("�ړ����x"), SerializeField]
    private float moveSpeed;

    [Header("�ړ����x���[�g"), SerializeField]
    private float moveSpeedRate = 0.0f;

    private bool isRolling = false;

    private bool isJumpEnd = false;

    private bool isGroundTmp = false;

    [Header("ray�̒���"), SerializeField]
    private float rayLength;

    [Header("�e��Prefab"), SerializeField]
    private GameObject bullet;

    //���ˊԊu
    private int fireInterval = 0;

    private void Awake()
    {
        playerInput = new PlayerInputAction();
        rigidbody = GetComponent<Rigidbody>();

        DG.Tweening.DOTween.SetTweensCapacity(tweenersCapacity: 200, sequencesCapacity: 500);
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
        GravityManager.instance.gravityUpdate(rigidbody);

        //�X�e�B�b�N���͎擾
        Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();
        if (inputValue.sqrMagnitude == 0.0f)
        {
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalk", false);
        }

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
            animator.SetBool("isWalk", true);
            animator.SetBool("isIdle", false);

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
        //�ˌ��C���^�[�o��
        fireInterval++;

        //�ˌ�����(�g���K�[���͂ɗ]�T����������)
        if (playerInput.Player.Fire.ReadValue<float>() > 0.3f)
        {
            if (fireInterval % 10 == 0)
            {
                Instantiate(bullet, this.transform.position, this.transform.rotation);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Jump()
    {

        if (isGrounded())
        {
            //�X�e�B�b�N���͎擾
            Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();
            moveSpeedRate = 1.0f;

            //�O��ړ����܂��̓W�����v���͂�����W�����v
            if (playerInput.Player.Jump.triggered)
            {
                //���ړ������W�����v���͂����烍�[�����O
                if (Mathf.Abs(inputValue.x)>0.2f)
                {
                    isRolling = true;
                }
                else
                {
                    animator.SetBool("isJumpUp", true);
                    if(isJumpEnd)
                    {
                        rigidbody.AddForce(Vector3.up * 5.0f, ForceMode.VelocityChange);
                        if (isGroundTmp != isGrounded())
                        {
                            Debug.Log("isGroundTmp" + isGroundTmp);
                            animator.SetBool("isJumDown", true);
                        }
                    }
                }
            }
        }
        else
        {
            moveSpeedRate = 0.25f;
        }

        //1�t���[���O�̒l���擾
        //�ړ�����
        //1�t���[���O�̔��菈��

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
                rigidbody.AddForce(velocity * 10.0f, ForceMode.VelocityChange);
                isRolling = false;
            }
            
        }
    }

    public void isJumpUpEnd()
    {
        animator.SetBool("isJumpUpEnd", true);
        isJumpEnd = true;
        Debug.Log("isJumpUpEnd");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool isGrounded()
    {
        Vector3 rayPos = transform.position + new Vector3(0.0f, +0.1f, 0.0f);
        Ray ray = new Ray(rayPos, Vector3.down);
        return Physics.Raycast(ray, rayLength);

    }
}
