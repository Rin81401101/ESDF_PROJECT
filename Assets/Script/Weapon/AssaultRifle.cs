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

    //リロード中だったらtrue
    public override bool GetIsReload() {
        return m_isReload;
    }

    //リロードまでの時間を0～1に変換して取得できる
    //1だったらリロード完了
    public override float GetReloadRatio() {
        return m_reloadTimer/ m_reloadTime;
    }



    //発射間隔(m_shotInterval秒待つ)
    IEnumerator ShoInterval() {
        yield return new WaitForSeconds(m_shotInterval);
        m_canShot = true;
    }
}
