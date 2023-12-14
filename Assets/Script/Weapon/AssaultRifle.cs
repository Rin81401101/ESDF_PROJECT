using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AssaultRifle : WeaponBase {
    [SerializeField]
    Transform m_muzzleTransform;

    [SerializeField]
    private float m_reloadTime = 1;

    [SerializeField]
    private float m_shotInterval = 0.0f;

    [SerializeField]
    private int m_bulletMax = 100;


    private int m_bulletNum = 100;
    private float m_reloadTimer = 0.05f;

    private bool m_canShot = true;
    private bool m_isReload = false;
    private void Awake() {
        m_bulletNum = m_bulletMax;
        m_reloadTimer= m_reloadTime;
    }

    // Update is called once per frame
    void Update() {
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

    //�����[�h����������true
    public override bool GetIsReload() {
        return m_isReload;
    }

    //�����[�h�܂ł̎��Ԃ�0�`1�ɕϊ����Ď擾�ł���
    //1�������烊���[�h����
    public override float GetReloadRatio() {
        return m_reloadTimer/ m_reloadTime;
    }



    //���ˊԊu(m_shotInterval�b�҂�)
    IEnumerator ShoInterval() {
        yield return new WaitForSeconds(m_shotInterval);
        m_canShot = true;
    }
}
