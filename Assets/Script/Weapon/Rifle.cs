using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Rifle : WeaponBase {
    [SerializeField]
    protected Bullet m_bullet;

    [SerializeField]
    Transform m_muzzleTransform;

    [SerializeField]
    private Image m_reticleImage;

    [SerializeField]
    private float m_reloadTime = 1;

    [SerializeField]
    private float m_shotInterval = 0.0f;

    [SerializeField]
    private int m_bulletMax = 100;

    [SerializeField]
    private float m_scopeRatio = 1;

    [SerializeField]
    private bool m_canUsedScope = false;

    private Image m_reticleUI;
    private int m_bulletNum = 100;
    private float m_reloadTimer = 0.05f;
    private bool m_canShot = true;
    private bool m_isReload = false;

    private void Start() {
        m_bulletNum = m_bulletMax;
        m_reloadTimer= m_reloadTime;

        WeaponUI weaponUI = WeaponManager.m_instance.m_weaponUI.GetComponent<WeaponUI>();
        m_reticleUI = Instantiate(m_reticleImage, weaponUI.m_reticleUIParent.transform);
    }

    // Update is called once per frame
    void Update() {
        //����������Ă�����Return
        if (!m_isVaild) return;

        //�����[�h����
        if (m_isReload) {
            m_reloadTimer += Time.deltaTime;

            //�����[�h����
            if (GetReloadRatio() >= 1) {
                m_reloadTimer = m_reloadTime;
                m_isReload = false;
                m_canShot=true;
                m_bulletNum = m_bulletMax;
            }
        }

    }

    public override void Shot() {
        if (m_canShot) {
            //�e�̐���
            Instantiate(m_bullet, m_muzzleTransform.position, m_muzzleTransform.rotation);


            m_bulletNum--;//�e�����炷
            m_canShot = false;//���ĂȂ�����

            if (m_bulletNum > 0) {
                //�e���c���Ă����m_shotInterval�b�ҋ@
                StartCoroutine(ShoInterval());

            } else {
                //�����[�h�J�n
                m_isReload=true;
                m_reloadTimer=0;
            }
        }

    }

    //���������[�h
    public override void Reload() {
        if (m_isReload) return;

        m_canShot=false;
        m_isReload = true;
        m_reloadTimer = 0;
        m_bulletNum = 0;
    }

    //�X�R�[�v
    public override void ViewScope(bool isView) {
        if (!m_canUsedScope) return;

        float fieldOfViewBase = 60;
        if (isView) {
            Camera.main.fieldOfView = fieldOfViewBase/ m_scopeRatio;
        } else {
            Camera.main.fieldOfView = fieldOfViewBase;
        }
    }

    //�����[�h����������true
    public override bool GetIsReload() {
        return m_isReload;
    }

    //�����[�h�܂ł̎��Ԃ�0�`1�ɕϊ����Ď擾�ł���
    //1�������烊���[�h����
    public override float GetReloadRatio() {
        return m_reloadTimer/ m_reloadTime;
    }

    //�L��������
    public override void SetVaild(bool isVaild) {
        ViewScope(false);
        m_reticleUI.enabled = isVaild;
        m_isVaild = isVaild;
        m_visualObject.SetActive(isVaild);
    }


    //���ˊԊu(m_shotInterval�b�҂�)
    IEnumerator ShoInterval() {
        yield return new WaitForSeconds(m_shotInterval);
        m_canShot = true;
    }
}
