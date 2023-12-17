using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationManager : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("アニメーター"),SerializeField]
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
    /// 移動アニメーションブレンド
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
    /// ジャンプアップアニメーション再生
    /// </summary>
    public void PlayJumpUpAnimation()
    {
        m_playerAnimator.SetBool("isJumpUp", true);
    }

    /// <summary>
    /// ジャンプダウンアニメーション再生
    /// </summary>
    public void PlayJumpDownAnimation()
    {
        m_playerAnimator.SetBool("isJumpDown", true);
    }

    /// <summary>
    /// ジャンプアップアニメーション停止
    /// ※アニメーションイベントで使用
    /// </summary>
    public void StopJumpUpAnimation()
    {
        m_playerAnimator.SetBool("isJumpUp", false);
        m_isJumpUpEnd = true;
    }

    /// <summary>
    /// ジャンプダウンアニメーション停止
    /// </summary>
    public void StopJumpDownAnimation()
    {
        m_playerAnimator.SetBool("isJumpDown", false);
    }

    /// <summary>
    /// ローリングアニメーション再生
    /// </summary>
    public void PlayRollingAnimation()
    {
        m_playerAnimator.SetBool("isRolling", true);

        float rotateAngle = 0.0f;

        Vector3 velocity = m_playerManager.GetRollingVelocity();
        //回転軸を取得
        Vector3 rotateAxis = Vector3.Cross(transform.forward,velocity);
        //Debug.Log(rotateAxis);

        //内積の値を取得
        float dotValue = Vector3.Dot(transform.forward, velocity);
        if (!float.IsNaN(dotValue))
        {
            //内積の値からラジアン角を取得
            rotateAngle = Mathf.Acos(dotValue);
            rotateAngle = Mathf.Rad2Deg * rotateAngle;
        }

        //出した回転軸に回転角度を与える
        transform.rotation = Quaternion.AngleAxis(rotateAngle, rotateAxis);

    }

    /// <summary>
    /// ローリングアニメーション停止
    /// ※アニメーションイベントで使用
    /// </summary>
    public void StopRollingAnimation()
    {
        m_playerAnimator.SetBool("isRolling", false);
        m_playerManager.SetIsRolling(false);
        transform.rotation = Quaternion.identity;

    }

    /// <summary>
    /// ローリング中前進
    /// ※アニメーションイベントで使用
    /// </summary>
    public void RollingAddForce()
    {
        //スティック入力取得
        Vector3 velocity = m_playerManager.GetRollingVelocity();

        m_rb.AddForce(velocity * 10.0f, ForceMode.VelocityChange);
    }

    /// <summary>
    /// ジャンプアップ終了取得
    /// </summary>
    public bool GetIsJumpUpEnd()
    {
        return m_isJumpUpEnd;
    }

    /// <summary>
    /// ジャンプアップ終了設定
    /// </summary>
    /// <param name="jumpUpEnd"></param>
    public void SetIsJumpUpEnd(bool jumpUpEnd)
    {
        m_isJumpUpEnd = jumpUpEnd;
        
    }
}
