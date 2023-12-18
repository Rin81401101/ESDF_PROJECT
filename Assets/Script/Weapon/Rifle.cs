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
        //無効化されていたらReturn
        if (!m_isVaild) return;

        //リロード処理
        if (m_isReload) {
            m_reloadTimer += Time.deltaTime;

            //リロード完了
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
            //弾の生成
            Instantiate(m_bullet, m_muzzleTransform.position, m_muzzleTransform.rotation);


            m_bulletNum--;//弾を減らす
            m_canShot = false;//撃てなくする

            if (m_bulletNum > 0) {
                //弾が残っていればm_shotInterval秒待機
                StartCoroutine(ShoInterval());

            } else {
                //リロード開始
                m_isReload=true;
                m_reloadTimer=0;
            }
        }

    }

    //強制リロード
    public override void Reload() {
        if (m_isReload) return;

        m_canShot=false;
        m_isReload = true;
        m_reloadTimer = 0;
        m_bulletNum = 0;
    }

    //スコープ
    public override void ViewScope(bool isView) {
        if (!m_canUsedScope) return;

        float fieldOfViewBase = 60;
        if (isView) {
            Camera.main.fieldOfView = fieldOfViewBase/ m_scopeRatio;
        } else {
            Camera.main.fieldOfView = fieldOfViewBase;
        }
    }

    //リロード中だったらtrue
    public override bool GetIsReload() {
        return m_isReload;
    }

    //リロードまでの時間を0～1に変換して取得できる
    //1だったらリロード完了
    public override float GetReloadRatio() {
        return m_reloadTimer/ m_reloadTime;
    }

    //有効化する
    public override void SetVaild(bool isVaild) {
        ViewScope(false);
        m_reticleUI.enabled = isVaild;
        m_isVaild = isVaild;
        m_visualObject.SetActive(isVaild);
    }


    //発射間隔(m_shotInterval秒待つ)
    IEnumerator ShoInterval() {
        yield return new WaitForSeconds(m_shotInterval);
        m_canShot = true;
    }
}
