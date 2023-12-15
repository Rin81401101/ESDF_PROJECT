using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Missile : WeaponBase {

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

    List<GameObject> m_enemyList = new List<GameObject>();
    List<Image> m_lockOnImageList = new List<Image>();
    Vector2 m_lockOnRangeDefaultSize = new Vector2();
    Image m_lockOnRange;
    private GameObject m_enemyParentObj;
    private int m_bulletNum = 5;
    private float m_reloadTimer = 0.05f;
    private bool m_isReload = false;
    private bool m_isLockOn = false;

    private bool isGenerateOnece = false;

    private void Start() {
        m_bulletNum = m_bulletMax;
        m_reloadTimer = m_reloadTime;
        m_enemyParentObj = GameObject.Find("Enemys");

        WeaponUI weaponUI = WeaponManager.m_instance.m_weaponUI.GetComponent<WeaponUI>();

        //ロックオン範囲UIを生成する
        Image rangeImage = Instantiate(m_lockOnRangeImage, weaponUI.m_lockOnUIParent.transform);
        m_lockOnRangeDefaultSize = rangeImage.rectTransform.sizeDelta;
        rangeImage.rectTransform.sizeDelta = m_lockOnRangeDefaultSize * m_lockOnRangeRatio;
        m_lockOnRange = rangeImage;

        //敵のロックオンUIを生成する
        for (int i = 0; i < m_bulletMax; i++) {
            Image img = Instantiate(m_lockOnImage, weaponUI.m_lockOnUIParent.transform);
            img.enabled = false;
            m_lockOnImageList.Add(img);
        }
    }
  
    void Update() {
        if (m_isLockOn)
        {
            LockOn();
        } else {
            //ロックオンしている敵がいるか
            bool canShot = false;
            for(int i=0; i< m_lockOnImageList.Count; i++) {
                if(m_lockOnImageList[i].enabled) {
                    canShot = true;
                    break;
                }
            }

            //ミサイルの発射
            if(canShot) {

            }

            DisableLockOn();
        }
        m_isLockOn = false;
    }

    private void LockOn() {
        //ロックオン範囲内の敵をm_enemyListに入れる
        for (int i = 0; i < m_enemyParentObj.transform.childCount; i++) {
            bool canLockOn = GetCanRockOn(m_enemyParentObj.transform.GetChild(i).gameObject.transform.position);
            if (canLockOn) {
                if (m_enemyList.Contains(m_enemyParentObj.transform.GetChild(i).gameObject)==false) {
                    m_enemyList.Add(m_enemyParentObj.transform.GetChild(i).gameObject);
                }
            } else {
                if (m_enemyList.Contains(m_enemyParentObj.transform.GetChild(i).gameObject) == true) { 
                    m_enemyList.Remove(m_enemyParentObj.transform.GetChild(i).gameObject);
                }
            }
        }

        //ここでロックオン
        for (int i=0; i< m_bulletMax; i++) {
            if (i < m_enemyList.Count) {
                m_lockOnImageList[i].enabled = true;
                m_lockOnImageList[i].rectTransform.position = GetScreenPos(m_enemyList[i].transform.position);
            } else {
                m_lockOnImageList[i].enabled = false;
            }
        }

        m_lockOnRange.enabled = true;
    }

    //ロックオン終了
    private void DisableLockOn() {
        for (int i = 0; i < m_bulletMax; i++) {
            m_lockOnImageList[i].enabled = false;
        }
        m_lockOnRange.enabled = false;
    }

    public override void Shot() {
        m_isLockOn = true;

    }

    //強制リロード
    public override void Reload() {

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
