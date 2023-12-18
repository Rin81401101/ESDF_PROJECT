using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Missile : WeaponBase {
    [SerializeField]
    private GameObject m_bullet;
    [SerializeField]
    private GameObject m_muzzle;
    [SerializeField]
    private Image m_lockOnImage;
    [SerializeField]
    private Image m_lockOnRangeImage;
    [SerializeField]
    private float m_reloadTime = 1;
    [SerializeField]
    private int m_bulletMax = 5;
    [SerializeField]
    private float m_lockOnRangeRatio = 0.7f;
    [SerializeField]
    private float m_rotationSpeedRatio = 0.01f;
    [SerializeField]
    private float m_moveSpeed = 10;
    [SerializeField]
    private float m_lockOnInterval = 0.5f;

    List<GameObject> m_rangeInEnemyList = new List<GameObject>();//ロックオン範囲に入っている敵
    List<GameObject> m_lockOnEnemyList = new List<GameObject>();//ロックオンが終わった敵
    List<Image> m_lockOnImageList = new List<Image>();
    Vector2 m_lockOnRangeDefaultSize = new Vector2();
    Image m_lockOnRangeUI = null;
    private GameObject m_enemyParentObj = null;
    private GameObject m_lockOnEnemy = null;//ロックオン中の敵
    private float m_reloadTimer = 0.05f;
    private float m_lockOnTimer = 0;
    private bool m_isReload = false;
    private bool m_isLockOn = false;


    private void Start() {
        m_reloadTimer = m_reloadTime;
        m_enemyParentObj = GameObject.Find("Enemys");
        m_reloadTimer = m_reloadTime;

        WeaponUI weaponUI = WeaponManager.m_instance.m_weaponUI.GetComponent<WeaponUI>();

        //ロックオン範囲UIを生成する
        Image rangeImage = Instantiate(m_lockOnRangeImage, weaponUI.m_lockOnUIParent.transform);
        m_lockOnRangeDefaultSize = rangeImage.rectTransform.sizeDelta;
        rangeImage.rectTransform.sizeDelta = m_lockOnRangeDefaultSize * m_lockOnRangeRatio;
        m_lockOnRangeUI = rangeImage;

        //敵のロックオンUIを生成する
        for (int i = 0; i < m_bulletMax; i++) {
            Image img = Instantiate(m_lockOnImage, weaponUI.m_lockOnUIParent.transform);
            img.enabled = false;
            m_lockOnImageList.Add(img);
        }
    }

    void Update() {
        if (!m_isVaild) {
            DisableLockOn();
            return;
        }

        if (m_isReload) {
            m_reloadTimer += Time.deltaTime;
            DisableLockOn();

            //リロード完了
            if (GetReloadRatio() >= 1) {
                m_reloadTimer = m_reloadTime;
                m_isReload = false;
            }
        } else {
            if (m_isLockOn) {
                LockOn();
            } else {
                //ロックオンが終了したタイミングでミサイルを発射する
                MissileShot();
                DisableLockOn();
            }
        }
        m_isLockOn = false;
    }

    private void LockOnRangeCheck() {
        //全体のエネミーからロックオン範囲内か探す
        for (int i = 0; i < m_enemyParentObj.transform.childCount; i++) {
            bool canLockOn = GetCanRockOn(m_enemyParentObj.transform.GetChild(i).gameObject.transform.position);
            if (canLockOn) {
                //ロックオン範囲内
                if (m_rangeInEnemyList.Contains(m_enemyParentObj.transform.GetChild(i).gameObject) == false) {
                    m_rangeInEnemyList.Add(m_enemyParentObj.transform.GetChild(i).gameObject);
                }
            } else {
                //ロックオン範囲外のエネミーをリストから削除
                if (m_rangeInEnemyList.Contains(m_enemyParentObj.transform.GetChild(i).gameObject) == true) {
                    m_rangeInEnemyList.Remove(m_enemyParentObj.transform.GetChild(i).gameObject);
                }

                //ロックオンしている敵がロックオン範囲外に出たのでリストから消す
                if (m_lockOnEnemyList.Contains(m_enemyParentObj.transform.GetChild(i).gameObject) == true) {
                    m_lockOnEnemyList.Remove(m_enemyParentObj.transform.GetChild(i).gameObject);
                }
            }
        }

        //ロックオン中のエネミーの範囲内チェック
        if (m_lockOnEnemy != null) {
            bool canLockOn = GetCanRockOn(m_lockOnEnemy.transform.position);
            if (!canLockOn) {
                //ロックオン範囲外のエネミーをリストから削除
                if (m_rangeInEnemyList.Contains(m_lockOnEnemy) == true) {
                    m_rangeInEnemyList.Remove(m_lockOnEnemy);
                }

                //ロックオンしている敵がロックオン範囲外に出たのでリストから消す
                if (m_lockOnEnemyList.Contains(m_lockOnEnemy) == true) {
                    m_lockOnEnemyList.Remove(m_lockOnEnemy);
                }
                m_lockOnEnemy = null;
            }

        }
    }

    private void LockOn() {
        //ロックオン範囲内の敵をm_enemyListに入れる
        LockOnRangeCheck();


        ///////////////////////////////////////////
        //---------ロックオン処理本体--------------//
        ///////////////////////////////////////////
        int lockOnCount = m_lockOnEnemyList.Count;
        if (lockOnCount < m_bulletMax) {
            //ロックオン対象の敵を取得
            if (lockOnCount < m_rangeInEnemyList.Count) {
                if (m_lockOnEnemy != m_rangeInEnemyList[lockOnCount]) {
                    m_lockOnEnemy = m_rangeInEnemyList[lockOnCount];
                    m_lockOnTimer = 0;
                    m_lockOnImageList[lockOnCount].GetComponent<EnemyLockOnUI>().SetLockOnState(EnemyLockOnUI.LockOnState.SEARCH);
                }
            }

            //ロックオン対象の敵を取得出来ていればタイマーを起動
            if (m_lockOnEnemy != null) {
                m_lockOnTimer += Time.deltaTime;

                //interval中もUIをOnにする
                m_lockOnImageList[lockOnCount].enabled = true;
                m_lockOnImageList[lockOnCount].rectTransform.position = GetScreenPos(m_lockOnEnemy.transform.position);

                if (m_lockOnTimer > m_lockOnInterval && m_bulletMax > m_lockOnEnemyList.Count) {
                    m_lockOnEnemyList.Add(m_lockOnEnemy);
                    m_lockOnEnemy = null;
                    m_lockOnImageList[lockOnCount].GetComponent<EnemyLockOnUI>().SetLockOnState(EnemyLockOnUI.LockOnState.LOCKON);
                }
            }
        }

        //UIを更新する
        UpdateUI(lockOnCount);
    }


    private void UpdateUI(int lockOnCount) {
        //ロックオンしている敵のUIをONにする
        for (int i = 0; i < m_bulletMax; i++) {
            if (i < m_lockOnEnemyList.Count) {
                m_lockOnImageList[i].enabled = true;
                m_lockOnImageList[i].rectTransform.position = GetScreenPos(m_lockOnEnemyList[i].transform.position);
            } else {
                if ((m_lockOnEnemy != null && lockOnCount == i) == false)
                    m_lockOnImageList[i].enabled = false;
            }
        }

        //ロックオン範囲の四角いUIを表示させる
        m_lockOnRangeUI.enabled = true;
    }

    //ミサイルを生成して発射する
    void MissileShot() {
        //ロックオンしている敵がいるか
        bool canShot = false;
        for (int i = 0; i < m_lockOnImageList.Count; i++) {
            if (m_lockOnImageList[i].enabled) {
                canShot = true;
                break;
            }
        }

        //ミサイルの発射
        if (canShot) {
            for (int i = 0; i < m_lockOnEnemyList.Count; i++) {
                GameObject bullet = Instantiate(m_bullet.gameObject, m_muzzle.transform.position, m_muzzle.transform.rotation);
                MissileBullet missileBullet = bullet.GetComponent<MissileBullet>();
                if (missileBullet != null) {
                    missileBullet.SetTarget(m_lockOnEnemyList[i]);
                    missileBullet.SetRotationSpeedRatio(m_rotationSpeedRatio);
                    missileBullet.SetMoveSpeed(m_moveSpeed);
                }
                m_isReload = true;
                m_reloadTimer = 0;
                
            }

        }
    }

    //ロックオン終了
    private void DisableLockOn() {
        if (m_lockOnImageList.Count > 0) {
            for (int i = 0; i < m_bulletMax; i++) {
                m_lockOnImageList[i].enabled = false;
            }
        }

        if (m_lockOnRangeUI != null) {
            m_lockOnRangeUI.enabled = false;
        }

        m_lockOnEnemy = null;
        m_lockOnTimer = 0;
        m_lockOnEnemyList.Clear();
    }

    public override void Shot() {
        m_isLockOn = true;

    }

    //強制リロード
    public override void Reload() {
        if (m_isReload) return;
        m_isReload = true;
        m_reloadTimer = 0;


    }

    //スコープ
    public override void ViewScope(bool isView) {
        //NONE
    }

    //リロード中だったらtrue
    public override bool GetIsReload() {
        return m_isReload;
    }

    //リロードまでの時間を0～1に変換して取得できる
    //1だったらリロード完了
    public override float GetReloadRatio() {
        return m_reloadTimer / m_reloadTime;
    }


    //有効化する
    public override void SetVaild(bool isVaild) {
        if (isVaild) {

        } else {
            DisableLockOn();
        }
        m_isVaild = isVaild;
        m_visualObject.SetActive(isVaild);
    }

    private bool GetCanRockOn(Vector3 pos) {
        Vector2 screenPosRatio = GetScreenPosRatio(pos);

        if (screenPosRatio.x < 0.5f + m_lockOnRangeRatio / 2 && screenPosRatio.x > 0.5f - m_lockOnRangeRatio / 2 &&
            screenPosRatio.y < 0.5f + m_lockOnRangeRatio / 2 && screenPosRatio.y > 0.5f - m_lockOnRangeRatio / 2) {
            return true;
        }
        return false;
    }
    private Vector3 GetScreenPosRatio(Vector3 pos) {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        Vector3 screenPosRatio = new Vector3();

        //ロックオン範囲内に入っているか
        screenPosRatio.x = screenPos.x / Screen.width;
        screenPosRatio.y = screenPos.y / Screen.height;
        screenPosRatio.z = 0;

        return screenPosRatio;
    }
    private Vector3 GetScreenPos(Vector3 pos) {
        return Camera.main.WorldToScreenPoint(pos);
    }
}
