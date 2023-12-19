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
    private PlayerAnimationManager m_playerAnimationManager;

    private Rigidbody m_rb;

    [Header("移動速度"), SerializeField]
    private float m_moveSpeed;

    [Header("移動速度レート"), SerializeField]
    private float m_moveSpeedRate = 0.0f;

    [Header("ジャンプ力"), SerializeField]
    private float m_jumpPower;

    private bool m_isRolling = false;
    private Vector3 m_rollingVelocity = Vector3.zero;

    private bool m_isGroundTmp = false;

    [Header("rayの長さ"), SerializeField]
    private float m_rayLength;

    [Header("武器アタッチ箇所"), SerializeField]
    private GameObject m_weaponAttachParent;

    private WeaponBase[] m_weapon = new WeaponBase[2];
    private WeaponUI m_weaponUI;

    [SerializeField]
    Vector3 m_weaponScale;

    [SerializeField]
    private List<string> m_weaponNames;

    private int m_weaponIndex = 0;
    private bool m_isUsedScope = false;

    private void Awake()
    {
        playerInput = new PlayerInputAction();
        m_rb = GetComponent<Rigidbody>();
        m_weaponUI = WeaponManager.m_instance.m_weaponUI.GetComponent<WeaponUI>();

        m_weapon[0] = WeaponManager.m_instance.AttachWeapon(m_weaponAttachParent, m_weaponNames[0].ToString());
        m_weapon[1] = WeaponManager.m_instance.AttachWeapon(m_weaponAttachParent, m_weaponNames[1].ToString());
        m_weapon[1].SetVaild(false);
        SetWeaponTransform(m_weaponIndex);
    }
  
    private void OnEnable()
    {
        //PlayerInputAction有効
        playerInput.Enable();
    }

    private void OnDisable()
    {
        //PlayerInputAction無効
        playerInput.Disable();
    }

    void Update()
    {
        GravityManager.m_instance.gravityUpdate(m_rb);

        m_weaponUI.m_reloadUIMain.fillAmount = m_weapon[m_weaponIndex].GetReloadRatio();
        m_weaponUI.m_reloadUIParent.SetActive(m_weapon[m_weaponIndex].GetIsReload());

        //移動
        Move();

        //ジャンプ
        Jump();

        //射撃
        Fire();

        //武器切り替え
        ChangeWeapon();

        //ローリング
        Rolling();

        //武器のスコープをのぞく
        if (playerInput.Player.Scope.triggered)
        {
            m_isUsedScope = !m_isUsedScope;
            m_weapon[m_weaponIndex].ViewScope(m_isUsedScope);

        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        //スティック入力取得
        Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();


        if ((inputValue.sqrMagnitude != 0.0f) && !m_isRolling)
        {
            //プレイヤー移動アニメーション
            m_playerAnimationManager.MoveAnimationBlend(inputValue, inputValue.magnitude);

            //プレイヤー移動
            Vector3 moveVelocity = new Vector3(
                inputValue.x * m_moveSpeed * m_moveSpeedRate * inputValue.magnitude * Time.deltaTime,
                0.0f,//ジャンプ
                inputValue.y * m_moveSpeed * m_moveSpeedRate * inputValue.magnitude * Time.deltaTime);

            transform.position += moveVelocity;
        }
    }

    /// <summary>
    /// 射撃/リロード処理
    /// </summary>
    private void Fire()
    {

        //射撃判定(トリガー入力に余裕を持たせる)
        if (playerInput.Player.Fire.ReadValue<float>() > 0.3f)
        {
            m_weapon[m_weaponIndex].Shot();
        }

        if (playerInput.Player.Reload.triggered)
        {
            m_weapon[m_weaponIndex].Reload();
        }
    }

    /// <summary>
    /// 武器変更
    /// </summary>
    private void ChangeWeapon() {

        if (playerInput.Player.WeaponChange.triggered)
        {
            //武器交換ボタンの値で武器を切り替える
            if (m_weaponIndex != 0)
            {
                m_weapon[0].SetVaild(true);
                m_weapon[1].SetVaild(false);
                m_weaponIndex = 0;
            }
            else
            {
                m_weapon[0].SetVaild(false);
                m_weapon[1].SetVaild(true);
                m_weaponIndex = 1;
            }

            //武器のトランスフォーム設定
            SetWeaponTransform(m_weaponIndex);
        }
    }


    /// <summary>
    /// ジャンプ処理
    /// </summary>
    private void Jump()
    {
        m_playerAnimationManager.StopJumpDownAnimation();
        if (isGrounded())
        {
            //スティック入力取得
            Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();
            m_moveSpeedRate = 1.0f;

            //前後移動中またはジャンプ入力したらジャンプ
            if (playerInput.Player.Jump.triggered)
            {
                if (Mathf.Abs(inputValue.x) < 0.2f)
                {
                    m_playerAnimationManager.PlayJumpUpAnimation();
                }
            }

            if (m_playerAnimationManager.GetIsJumpUpEnd())
            {
                m_rb.AddForce(Vector3.up * m_jumpPower, ForceMode.VelocityChange);
                if (m_isGroundTmp != isGrounded(true))
                {
                    m_playerAnimationManager.SetIsJumpUpEnd(false);
                    m_playerAnimationManager.PlayJumpDownAnimation();
                }
            }
        }
        else
        {
            m_moveSpeedRate = 0.25f;
        }

        m_isGroundTmp = isGrounded();
    }

    /// <summary>
    /// ローリング処理
    /// </summary>
    private void Rolling()
    {
        //スティック入力取得
        Vector2 inputValue = playerInput.Player.Move.ReadValue<Vector2>();
        if (playerInput.Player.Jump.triggered)
        {
            if (Mathf.Abs(inputValue.x) >= 0.2f && !m_isRolling)
            {
                m_isRolling = true;
                m_rollingVelocity = new Vector3(inputValue.x, 0.0f, inputValue.y);
                m_playerAnimationManager.PlayRollingAnimation();

            }
        }
    }

    private void SetWeaponTransform(int m_index)
    {
        m_weapon[m_index].gameObject.transform.localScale = m_weaponScale;

        //回転軸取得
        Vector3 rotateAxis = Vector3.Cross(m_weapon[m_index].gameObject.transform.forward, this.transform.forward);

        //回転角度取得
        float rotateAngle = Vector3.Dot(m_weapon[m_index].gameObject.transform.forward, this.transform.forward);

        //出した回転軸に回転角度を与える
        m_weapon[m_index].gameObject.transform.rotation = Quaternion.AngleAxis(rotateAngle, rotateAxis);
    }

    /// <summary>
    /// 地面着地判定
    /// </summary>
    /// <returns></returns>
    private bool isGrounded(bool isAnimation = false)
    {
        float lengthRate = 1.0f;
        if (isAnimation)
        {
            lengthRate = 2.0f;
        }

        Vector3 rayPos = transform.position + new Vector3(0.0f, +0.1f, 0.0f);
        Ray ray = new Ray(rayPos, Vector3.down);
        return Physics.Raycast(ray, m_rayLength * lengthRate);
    }

    /// <summary>
    /// リジッドボディ取得
    /// </summary>
    /// <returns></returns>
    public Rigidbody GetRigidbody()
    {
        return m_rb;
    }

    /// <summary>
    /// ローリング方向取得
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRollingVelocity()
    {
        return m_rollingVelocity;
    }

    /// <summary>
    /// ローリングフラグ設定
    /// </summary>
    /// <param name="flag"></param>
    public void SetIsRolling(bool flag)
    {
        m_isRolling = flag;
    }

    public WeaponBase GetWeapon(int index) {
        return m_weapon[index];
    }

    public int GetWeaponIndex() {
        return m_weaponIndex;
    }

    public void SetWeapon(string weaponName, int index) {
        m_weapon[index] = WeaponManager.m_instance.AttachWeapon(m_weaponAttachParent, weaponName);
        SetWeaponTransform(index);
    }
}
