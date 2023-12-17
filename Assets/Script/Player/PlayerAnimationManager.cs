using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationManager : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("�A�j���[�^�["),SerializeField]
    private Animator m_playerAnimator;

    private bool m_isJumpUpEnd = false;

    private PlayerManager m_playerManager;

    private Rigidbody m_rb;

    public void Awake()
    {
        m_playerManager = transform.parent.GetComponent<PlayerManager>();
        m_rb = m_playerManager.GetRigidbody();
    }

    /// <summary>
    /// �ړ��A�j���[�V�����u�����h
    /// </summary>
    /// <param name="animationParameter"></param>
    /// <param name="animationSpeedRate"></param>
    public void MoveAnimationBlend(Vector2 animationParameter,float animationSpeedRate)
    {
        if (animationParameter.magnitude < 0.1)
        {
            animationSpeedRate = 1.0f;
        }
        m_playerAnimator.SetFloat("SpeedRate", animationSpeedRate);
        m_playerAnimator.SetFloat("BlendX", animationParameter.x);
        m_playerAnimator.SetFloat("BlendY", animationParameter.y);
    }
    
    /// <summary>
    /// �W�����v�A�b�v�A�j���[�V�����Đ�
    /// </summary>
    public void PlayJumpUpAnimation()
    {
        m_playerAnimator.SetBool("isJumpUp", true);
    }

    /// <summary>
    /// �W�����v�_�E���A�j���[�V�����Đ�
    /// </summary>
    public void PlayJumpDownAnimation()
    {
        m_playerAnimator.SetBool("isJumpDown", true);
    }

    /// <summary>
    /// �W�����v�A�b�v�A�j���[�V������~
    /// ���A�j���[�V�����C�x���g�Ŏg�p
    /// </summary>
    public void StopJumpUpAnimation()
    {
        m_playerAnimator.SetBool("isJumpUp", false);
        m_isJumpUpEnd = true;
    }

    /// <summary>
    /// �W�����v�_�E���A�j���[�V������~
    /// </summary>
    public void StopJumpDownAnimation()
    {
        m_playerAnimator.SetBool("isJumpDown", false);
    }

    /// <summary>
    /// ���[�����O�A�j���[�V�����Đ�
    /// </summary>
    public void PlayRollingAnimation()
    {
        m_playerAnimator.SetBool("isRolling", true);

        float rotateAngle = 0.0f;

        Vector3 velocity = m_playerManager.GetRollingVelocity();
        //��]�����擾
        Vector3 rotateAxis = Vector3.Cross(transform.forward,velocity);
        //Debug.Log(rotateAxis);

        //���ς̒l���擾
        float dotValue = Vector3.Dot(transform.forward, velocity);
        if (!float.IsNaN(dotValue))
        {
            //���ς̒l���烉�W�A���p���擾
            rotateAngle = Mathf.Acos(dotValue);
            rotateAngle = Mathf.Rad2Deg * rotateAngle;
        }

        //�o������]���ɉ�]�p�x��^����
        transform.rotation = Quaternion.AngleAxis(rotateAngle, rotateAxis);

    }

    /// <summary>
    /// ���[�����O�A�j���[�V������~
    /// ���A�j���[�V�����C�x���g�Ŏg�p
    /// </summary>
    public void StopRollingAnimation()
    {
        m_playerAnimator.SetBool("isRolling", false);
        m_playerManager.SetIsRolling(false);
        transform.rotation = Quaternion.identity;

    }

    /// <summary>
    /// ���[�����O���O�i
    /// ���A�j���[�V�����C�x���g�Ŏg�p
    /// </summary>
    public void RollingAddForce()
    {
        //�X�e�B�b�N���͎擾
        Vector3 velocity = m_playerManager.GetRollingVelocity();

        m_rb.AddForce(velocity * 10.0f, ForceMode.VelocityChange);
    }

    /// <summary>
    /// �W�����v�A�b�v�I���擾
    /// </summary>
    public bool GetIsJumpUpEnd()
    {
        return m_isJumpUpEnd;
    }

    /// <summary>
    /// �W�����v�A�b�v�I���ݒ�
    /// </summary>
    /// <param name="jumpUpEnd"></param>
    public void SetIsJumpUpEnd(bool jumpUpEnd)
    {
        m_isJumpUpEnd = jumpUpEnd;
        
    }
}
