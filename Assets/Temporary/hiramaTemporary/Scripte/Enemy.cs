using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    List<Node.NodePos> m_firstNodePosList = new List<Node.NodePos>();    //初回の経由地点リスト
    List<Node> m_shortNodeObjList = new List<Node>(); //最短経由地点ルートリスト
    List<Node> m_tempNodeObjList = new List<Node>();  //一時経由地点ルートリスト

    [Header("目標地点"), SerializeField] Transform m_playerTransform;
    [Header("親の経由地点情報"), SerializeField] GameObject m_masterNodeObj;
    [Header("移動速度"), SerializeField] float m_moveSpeed = 3.0f;
    [Header("距離の誤差範囲"), SerializeField] float m_errorRange = 0.1f;
    [Header("処理間隔時間"), SerializeField] public float m_processIntervalTime = 1.0f;

    [Header("最短経由地点の座標差合計"), HideInInspector] float m_shortNodePosDis;
    [Header("一時経由地点の座標差合計"), HideInInspector] float m_tempNodePosDis;
    [Header("移動予定の経由地点番号"), HideInInspector] int m_currentTempNodeIndex;
    [Header("移動中の経由地点"), HideInInspector] Node m_currentNode;
    [Header("移動中の経由地点座標"), HideInInspector] Vector3 m_currentDis;
    [Header("経由地点到達フラグ"), HideInInspector] bool m_arriveNode = false;


    void Start()
    {
        StartCoroutine(ShortestRouteSearch());     //最短経路探索処理
    }

    #region 最短経路探索処理
    IEnumerator ShortestRouteSearch()
    {
        while (true)
        {
            ClearShortestRoute();             //前回最短経路初期化
            Node nodeObject = GetFirstPos();  //初回地点取得
            GetShortestRoute(nodeObject);     //最短経路ルート取得

            Debug.Log("最短経路：" + string.Join(", ", m_shortNodeObjList) + m_shortNodePosDis);

            yield return new WaitForSeconds(m_processIntervalTime);
        }
    }
    #endregion

    #region 前回最短経路初期化
    void ClearShortestRoute()
    {
        m_shortNodeObjList.Clear();
        m_shortNodePosDis = 0f;
        m_currentTempNodeIndex = 0;
    }
    #endregion

    #region 初回地点取得
    Node GetFirstPos()
    {
        Node firstNodeObj = null;    //経由地点情報

        float firstNodePosDisMin = 0;   //経由地点の最短座標間距離
        int firstNodeNumMin = 0;        //経由地点の最短距離の要素番号

        //子の経由地点数分、繰り返す
        for (int i = 0; i < m_masterNodeObj.gameObject.transform.childCount; i++)
        {
            Node.NodePos tempNodePos = new Node.NodePos();

            //各経由地点情報と座標間距離を取得
            tempNodePos.nodeObj = m_masterNodeObj.gameObject.transform.GetChild(i).gameObject;

            Vector3 tempNodePosDis = tempNodePos.nodeObj.gameObject.transform.position;
            tempNodePos.nodePosDis = (Vector3.Distance(transform.position, tempNodePosDis));

            //初回は必ず保持
            if (i == 0)
            {
                firstNodePosDisMin = tempNodePos.nodePosDis;
                firstNodeNumMin = i;
            }
            //以降は座標間距離を比較、距離が短い方に更新
            else if (firstNodePosDisMin > tempNodePos.nodePosDis)
            {
                firstNodePosDisMin = tempNodePos.nodePosDis;
                firstNodeNumMin = i;
            }

            //座標情報、座標間距離の組合せをリストに格納
            m_firstNodePosList.Add(tempNodePos);
        }

        //目標地点から最も近い経由地点を取得
        firstNodeObj = m_firstNodePosList[firstNodeNumMin].nodeObj.GetComponent<Node>();
        //Debug.Log("エネミー最寄経由地点：" + firstNodeObj);

        return firstNodeObj;
    }
    #endregion

    #region 最短経路ルート取得
    void GetShortestRoute(Node nextNodeObj)
    {
        //次の行先経由地点数分、処理を繰り返す
        for (int i = 0; i < nextNodeObj.m_nextNodeObjList.Count; i++)
        {
            //一時リストに座標と座標差を格納する
            m_tempNodeObjList.Add(nextNodeObj);

            Vector3 nextNodePosDis = nextNodeObj.m_nextNodeObjList[i].transform.position;
            if (nextNodeObj.m_nextNodeObjList[i] != null)
            {
                m_tempNodePosDis += (Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis));
            }

            //プレイヤーの最寄経由地点まで格納した場合、
            if (nextNodeObj.m_isPlayer)
            {
                //初回の場合、最短経路として格納する
                if (m_shortNodeObjList.Count == 0)
                {
                    m_shortNodeObjList.AddRange(m_tempNodeObjList);
                    m_shortNodePosDis = m_tempNodePosDis;
                }
                //以降の場合、最短経路と一時リストを比較、更新を行う
                else if (m_shortNodePosDis > m_tempNodePosDis)
                {
                    m_shortNodeObjList.Clear();
                    m_shortNodeObjList.AddRange(m_tempNodeObjList);
                    m_shortNodePosDis = m_tempNodePosDis;
                }

                //Debug.Log("経路一覧：" + string.Join(", ", tempNodeObjList) + tempNodePosDis);

                //プレイヤーの最寄経由地点データを削除する
                m_tempNodeObjList.Remove(nextNodeObj);
                m_tempNodePosDis -= (Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis));

                return;
            }

            //次の行先経由地点を仮取得する
            Node tempNodeObj = nextNodeObj.m_nextNodeObjList[i];

            //取得データが一時リスト内に無い場合、再帰関数で次の経由地点情報の階層に移る
            if (!m_tempNodeObjList.Contains(tempNodeObj))
            {
                GetShortestRoute(tempNodeObj);
            }

            //探索済み、または、他の行先経由地点の再読込のため、削除する
            m_tempNodeObjList.Remove(nextNodeObj);
            m_tempNodePosDis -= Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis);
        }
    }
    #endregion


    void Update()
    {
        MoveShortestRoute();    //最短経路移動処理
    }

    #region 最短経路移動処理
    void MoveShortestRoute()
    {
        if (m_currentTempNodeIndex < m_shortNodeObjList.Count)
        {
            //現在の経由地点に到達するまで、経由地点を更新しない
            if (!m_arriveNode)
            {
                //現在の移動先経由地点と移動方向を取得する
                m_currentNode = m_shortNodeObjList[m_currentTempNodeIndex];
                m_currentDis = (m_currentNode.transform.position - transform.position).normalized;
                m_arriveNode = true;
            }

            //現在の移動先経由地点に移動する
            transform.Translate(m_currentDis * m_moveSpeed * Time.deltaTime);

            //現在の経由地点に到達した場合、次の移動先経由地点を設定
            if (Vector3.Distance(transform.position, m_currentNode.transform.position) < m_errorRange)
            {
                m_arriveNode = false;
                m_currentTempNodeIndex++;
            }
        }
    }
    #endregion
}
