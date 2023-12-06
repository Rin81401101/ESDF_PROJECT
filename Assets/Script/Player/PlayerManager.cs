using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    //���͊Ǘ�
    private PlayerInputAction playerInput;

    private Rigidbody rigidbody;

    [Header("�ړ����x"), SerializeField]
    private float moveSpeed;

    [Header("�ړ����x"), SerializeField]
    private float moveSpeedRate = 0.0f;

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

        //�ړ�
        Move();

        //�W�����v
        Jump();

        //�ˌ�
        Fire();

        //���ړ����W�����v�����[�����O
        //�O��ړ��܂��̓W�����v�P�́��W�����v
    }

    private void Move()
    {
        Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();

        Vector3 moveVelocity = new Vector3(
            inputValue.x * moveSpeed * moveSpeedRate * Time.deltaTime,
            0.0f,//�W�����v
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
