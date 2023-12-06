using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    //入力管理
    private PlayerInputAction playerInput;

    private Rigidbody rigidbody;

    [Header("移動速度"), SerializeField]
    private float moveSpeed;

    [Header("移動速度"), SerializeField]
    private float moveSpeedRate = 0.0f;

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
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GravityManager.instance.gravityUpdate(rigidbody);

        //移動
        Move();

        //ジャンプ
        Jump();

        //射撃
        Fire();

        //横移動かつジャンプ→ローリング
        //前後移動またはジャンプ単体→ジャンプ
    }

    private void Move()
    {
        Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();

        Vector3 moveVelocity = new Vector3(
            inputValue.x * moveSpeed * moveSpeedRate * Time.deltaTime,
            0.0f,//ジャンプ
            inputValue.y * moveSpeed * moveSpeedRate * Time.deltaTime);

        transform.position += moveVelocity;
    }

    private void Fire()
    {
        fireInterval++;

        if (playerInput.Player.Fire.ReadValue<float>() > 0.3f)
        {
            if (fireInterval % 10 == 0)
            {
                Instantiate(bullet, this.transform.position, this.transform.rotation);
            }
        }
    }

    private void Jump()
    {

        if (isGrounded())
        {
            moveSpeedRate = 1.0f;
            if (playerInput.Player.Jump.triggered)
            {
                rigidbody.AddForce(Vector3.up * 5.0f, ForceMode.VelocityChange);
            }
        }
        else
        {
            moveSpeedRate = 0.25f;
        }
    }

    private bool isGrounded()
    {
        Vector3 rayPos = transform.position + new Vector3(0.0f, +0.1f, 0.0f);
        Ray ray = new Ray(rayPos, Vector3.down);
        return Physics.Raycast(ray, rayLength);

    }
}
