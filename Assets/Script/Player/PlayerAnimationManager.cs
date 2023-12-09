using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationManager : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("アニメーター"),SerializeField]
    private Animator playerAnimator;

    private bool isJumpUpEnd = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveAnimation(Vector2 animationParameter,float animationSpeedRate)
    {
        if (animationParameter.magnitude < 0.1)
        {
            animationSpeedRate = 1.0f;
        }
        playerAnimator.SetFloat("SpeedRate", animationSpeedRate);
        playerAnimator.SetFloat("BlendX", animationParameter.x);
        playerAnimator.SetFloat("BlendY", animationParameter.y);
    }
    

    public void PlayJumpUpAnimation()
    {
        playerAnimator.SetBool("isJumpUp", true);
    }

    public void PlayJumpDownAnimation()
    {
        playerAnimator.SetBool("isJumpDown", true);
    }

    public void PlayJumpUpEnd()
    {
        playerAnimator.SetBool("isJumpUp", false);
        isJumpUpEnd = true;
        Debug.Log("isJumpUpEnd");
    }

    public void IsJumpDownEnd()
    {
        playerAnimator.SetBool("isJumpDown", false);
    }

    public bool GetIsJumpUpEnd()
    {
        return isJumpUpEnd;
    }

    public void SetIsJumpUpEnd(bool jumpUpEnd)
    {
        isJumpUpEnd = jumpUpEnd;
        
    }
}
