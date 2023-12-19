using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    List<Node.NodePos> m_playerNodePosList = new List<Node.NodePos>();     //目標の経由地点リスト

    [Header("親の経由地点情報"), SerializeField] GameObject m_masterNodeObj;
    [Header("プレイヤーの移動速度"), SerializeField] float m_moveSpeed = 5.0f;
    [Header("処理間隔時間"), SerializeField] float m_processIntervalTime = 1.0f;

    [Header("目標の経由地点"), HideInInspector] public Node m_playerNodeObj;
    [Header("前回のプレイヤー最寄経由地点"), HideInInspector] public Node m_lastPlayerNode = null;


    void Awake()
    {
        StartCoroutine(GetPlayerNode());    //目標経由地点探索処理
    }

    #region 目標経由地点探索処理
    IEnumerator GetPlayerNode()
    {
        while (true)
        {
            //前回のプレイヤーの最寄経由地点を戻す
            if (m_lastPlayerNode != null)
            {
                m_lastPlayerNode.m_isPlayer = false;
            }

            m_playerNodeObj = null;           //経由地点情報
            float playerNodePosDisMin = 0;  //経由地点の最短座標間距離
            int playerNodeNumMin = 0;       //経由地点の最短距離の要素番号

            //子の経由地点数分、繰り返す
            for (int i = 0; i < m_masterNodeObj.gameObject.transform.childCount; i++)
            {
                Node.NodePos tempNodePos = new Node.NodePos();

                //各経由地点情報と座標間距離取得
                tempNodePos.nodeObj = m_masterNodeObj.gameObject.transform.GetChild(i).gameObject;

                Vector3 tempNodePosDis = tempNodePos.nodeObj.gameObject.transform.position;
                tempNodePos.nodePosDis = (Vector3.Distance(transform.position, tempNodePosDis));

                //初回は必ず保持
                if (i == 0)
                {
                    playerNodePosDisMin = tempNodePos.nodePosDis;
                    playerNodeNumMin = i;
                }
                //以降は座標間距離を比較、距離が短い方に更新
                else
                {
                    if (playerNodePosDisMin > tempNodePos.nodePosDis)
                    {
                        playerNodePosDisMin = tempNodePos.nodePosDis;
                        playerNodeNumMin = i;
                    }
                }

                //座標情報、座標間距離の組合せをリストに格納
                m_playerNodePosList.Add(tempNodePos);
            }

            //目標地点から最も近い経由地点を取得
            m_playerNodeObj = m_playerNodePosList[playerNodeNumMin].nodeObj.GetComponent<Node>();

            //プレイヤーの最寄経由地点を立てる
            if (m_playerNodeObj != null)
            {
                m_playerNodeObj.m_isPlayer = true;
                m_lastPlayerNode = m_playerNodeObj;
                //Debug.Log("プレイヤー最寄経由地点：" + playerNodeObj);
            }

            yield return new WaitForSeconds(m_processIntervalTime);
        }
    }
    #endregion


    void Update()
    {
        // "WASD"の入力を取得する
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 入力値から移動ベクトルを作成し、移動する
        Vector3 move = new Vector3(horizontalInput, 0f, verticalInput) * m_moveSpeed * Time.deltaTime;
        transform.Translate(move);
    }
}
