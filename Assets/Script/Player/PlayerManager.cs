using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    //入力管理
    private PlayerInputAction playerInput;

    [Header("アニメーションマネージャー"), SerializeField]
    private PlayerAnimationManager playerAnimationManager ;

    private Rigidbody rb;

    [Header("アニメーター"), SerializeField]
    private Animator animator;

    [Header("移動速度"), SerializeField]
    private float moveSpeed;

    [Header("移動速度レート"), SerializeField]
    private float moveSpeedRate = 0.0f;

    [Header("ジャンプ力"), SerializeField]
    private float jumpPower;

    private bool isRolling = false;

    private bool isGroundTmp = false;

    [Header("rayの長さ"), SerializeField]
    private float rayLength;

    [Header("武器attach箇所"), SerializeField]
    private GameObject WeaponAttachParent;

    WeaponBase weapon;
    WeaponUI weaponUI;

    [SerializeField]
    Vector3 scale;

    //発射間隔
    private int fireInterval = 0;

    private void Awake()
    {
        playerInput = new PlayerInputAction();
        rb = GetComponent<Rigidbody>();
        weaponUI = WeaponManager.m_instance.m_weaponUI.GetComponent<WeaponUI>();

        weapon = WeaponManager.m_instance.AttachWeapon(WeaponAttachParent, "Missile"); 
        weapon.gameObject.transform.localScale = scale;

        //回転軸　武器のベクトルと向けたい方向のベクトルで外積
        Vector3 rotateAxis = Vector3.Cross(weapon.gameObject.transform.forward, this.transform.forward);
        Debug.Log(rotateAxis);

        //回転角度 武器のベクトルと向けたい方向のベクトルで内積
        float rotateAngle = Vector3.Dot(weapon.gameObject.transform.forward, this.transform.forward);
        Debug.Log(rotateAngle);


        //出した回転軸に回転角度を与える
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

        //移動
        Move();

        //ジャンプ
        Jump();

        //射撃
        Fire();

        //ローリング
        Rolling();

        //横移動かつジャンプ→ローリング
        //前後移動またはジャンプ単体→ジャンプ
    }

    /// <summary>
    /// 
    /// </summary>
    private void Move()
    {
        //スティック入力取得
        Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();


        if (inputValue.sqrMagnitude != 0.0f)
        {
            //プレイヤー移動アニメーション
            playerAnimationManager.MoveAnimation(inputValue, inputValue.magnitude);

            //プレイヤー移動
            Vector3 moveVelocity = new Vector3(
                inputValue.x * moveSpeed * moveSpeedRate * inputValue.magnitude * Time.deltaTime,
                0.0f,//ジャンプ
                inputValue.y * moveSpeed * moveSpeedRate * inputValue.magnitude * Time.deltaTime);

            transform.position += moveVelocity;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Fire()
    {

        //射撃判定(トリガー入力に余裕を持たせる)
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
            //スティック入力取得
            Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();
            moveSpeedRate = 1.0f;

            //前後移動中またはジャンプ入力したらジャンプ
            if (playerInput.Player.Jump.triggered)
            {
                //横移動中かつジャンプ入力したらローリング
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

            //ローリング加速
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
