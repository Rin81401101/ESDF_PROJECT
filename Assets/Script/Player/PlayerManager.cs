using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    //入力管理
    private PlayerInputAction playerInput;

    private Rigidbody rigidbody;

    [Header("アニメーター"), SerializeField]
    private Animator animator;

    [Header("移動速度"), SerializeField]
    private float moveSpeed;

    [Header("移動速度レート"), SerializeField]
    private float moveSpeedRate = 0.0f;

    private bool isRolling = false;

    private bool isJumpEnd = false;

    private bool isGroundTmp = false;

    [Header("rayの長さ"), SerializeField]
    private float rayLength;

    [Header("弾丸Prefab"), SerializeField]
    private GameObject bullet;

    //発射間隔
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

        //スティック入力取得
        Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();
        if (inputValue.sqrMagnitude == 0.0f)
        {
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalk", false);
        }

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
            animator.SetBool("isWalk", true);
            animator.SetBool("isIdle", false);

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
        //射撃インターバル
        fireInterval++;

        //射撃判定(トリガー入力に余裕を持たせる)
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
            //スティック入力取得
            Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();
            moveSpeedRate = 1.0f;

            //前後移動中またはジャンプ入力したらジャンプ
            if (playerInput.Player.Jump.triggered)
            {
                //横移動中かつジャンプ入力したらローリング
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

        //1フレーム前の値を取得
        //移動処理
        //1フレーム前の判定処理

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
