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
    

    public void PlayJumpUpAnimation()
    {
        m_playerAnimator.SetBool("isJumpUp", true);
    }

    public void PlayJumpDownAnimation()
    {
        m_playerAnimator.SetBool("isJumpDown", true);
    }

    //※アニメーションイベントで使用
    public void StopJumpUpAnimation()
    {
        m_playerAnimator.SetBool("isJumpUp", false);
        m_isJumpUpEnd = true;
    }

    public void StopJumpDownAnimation()
    {
        m_playerAnimator.SetBool("isJumpDown", false);
    }

    public bool GetIsJumpUpEnd()
    {
        return m_isJumpUpEnd;
    }

    public void SetIsJumpUpEnd(bool jumpUpEnd)
    {
        m_isJumpUpEnd = jumpUpEnd;
        
    }
}
