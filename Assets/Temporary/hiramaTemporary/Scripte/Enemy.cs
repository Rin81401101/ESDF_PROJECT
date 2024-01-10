using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    List<Node.NodePos> m_firstNodePosList = new List<Node.NodePos>();    //初回の経由地点リスト
    List<Node> m_shortNodeObjList = new List<Node>();   //最短経由地点ルートリスト
    List<Node> m_searchNodeObjList = new List<Node>();  //検索対象経由地点リスト
    List<Node> m_tempNodeObjList = new List<Node>();    //一時経由地点ルートリスト

    [Header("目標地点"), SerializeField] Transform m_playerTransform;
    [Header("親の経由地点情報"), SerializeField] GameObject m_masterNodeObj;
    [Header("ステージ情報"), SerializeField] GameObject m_stageObj;
    [Header("移動速度"), SerializeField] float m_moveSpeed = 3.0f;
    [Header("探索範囲"), SerializeField] float m_detectionRange = 50.0f;
    [Header("探索高度"), SerializeField] float m_detectionHeight = 3.0f;
    [Header("到達誤差範囲"), SerializeField] float m_errorRange = 0.1f;
    [Header("処理間隔時間"), SerializeField] public float m_processIntervalTime = 1.0f;
    [Header("移動の制限時間"), SerializeField] float m_timeoutDuration = 10.0f;

    [Header("前回のエネミー最寄経由地点"), HideInInspector] public Node m_lastEnemyNode = null;
    [Header("最短経由地点の座標差合計"), HideInInspector] float m_shortNodePosDis;
    [Header("一時経由地点の座標差合計"), HideInInspector] float m_tempNodePosDis;
    [Header("移動予定の経由地点番号"), HideInInspector] int m_currentTempNodeIndex;
    [Header("移動中の経由地点"), HideInInspector] Node m_currentNode;
    [Header("経由地点到達フラグ"), HideInInspector] bool m_arriveNode = false;
    [Header("制限時間用タイマー"), HideInInspector] float m_timeoutTimer = 0f;


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
            GetAreaNodeList();                //対象経由地点リスト取得
            GetShortestRoute(nodeObject);     //最短経路ルート取得

            Debug.Log("最短経路ルート：" + string.Join(", ", m_shortNodeObjList) + m_shortNodePosDis);

            yield return new WaitForSeconds(m_processIntervalTime);
        }
    }
    #endregion

    #region 前回最短経路初期化
    void ClearShortestRoute()
    {
        m_searchNodeObjList.Clear();
        m_shortNodeObjList.Clear();
        m_shortNodePosDis = 0f;
        m_currentTempNodeIndex = 0;
    }
    #endregion

    #region 初回地点取得
    Node GetFirstPos()
    {
        //前回の最寄経由地点を戻す
        if (m_lastEnemyNode != null)
        {
            m_lastEnemyNode.m_isEnemy = false;
        }

        Node firstNodeObj = null;       //経由地点情報
        float firstNodePosDisMin = 0;   //経由地点の最短座標間距離
        int firstNodeNumMin = 0;        //経由地点の最短距離の要素番号

        Vector3 rayDirection = m_playerTransform.position - transform.position;

        if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, m_detectionRange))
        {
            //プレイヤーを視認、且つ、一定の高度(Y軸)内で視認した場合、
            float heightDis = Mathf.Abs(m_playerTransform.position.y - transform.position.y);

            if (hit.collider.CompareTag("Player") && heightDis <= m_detectionHeight)
            {
                //プレイヤーから最も近い経由地点を取得
                GameObject playerNodeObj = GameObject.FindWithTag("Player");
                firstNodeObj = playerNodeObj.GetComponent<Player>().m_playerNodeObj;

                //Debug.Log("プレイヤー判定");
            }
            //プレイヤーを視認できなかった場合、
            else
            {
                //子の経由地点数分、繰り返す
                for (int i = 0; i < m_masterNodeObj.gameObject.transform.childCount; i++)
                {
                    Node.NodePos tempNodePos = new Node.NodePos();

                    //各経由地点情報と座標間距離を取得
                    tempNodePos.m_nodeObj = m_masterNodeObj.gameObject.transform.GetChild(i).gameObject;

                    Vector3 tempNodePosDis = tempNodePos.m_nodeObj.gameObject.transform.position;
                    tempNodePos.m_nodePosDis = (Vector3.Distance(transform.position, tempNodePosDis));

                    //初回は必ず保持
                    if (i == 0)
                    {
                        firstNodePosDisMin = tempNodePos.m_nodePosDis;
                        firstNodeNumMin = i;
                    }
                    //以降は座標間距離を比較、距離が短い方に更新
                    else if (firstNodePosDisMin > tempNodePos.m_nodePosDis)
                    {
                        firstNodePosDisMin = tempNodePos.m_nodePosDis;
                        firstNodeNumMin = i;
                    }

                    //座標情報、座標間距離の組合せをリストに格納
                    m_firstNodePosList.Add(tempNodePos);
                }

                //自身から最も近い経由地点を取得
                firstNodeObj = m_firstNodePosList[firstNodeNumMin].m_nodeObj.GetComponent<Node>();

                //Debug.Log("障害物判定");
            }
        }

        //Debug.Log("エネミー最寄経由地点：" + firstNodeObj);

        //エネミーの最寄経由地点を立てる
        if (firstNodeObj != null)
        {
            firstNodeObj.m_isEnemy = true;
            m_lastEnemyNode = firstNodeObj;
        }

        return firstNodeObj;
    }
    #endregion

    #region 対象経由地点リスト取得
    void GetAreaNodeList()
    {
        //分割した各エリアの経由地点リストを取得
        List<Node>[] tempAreaNodeList = m_stageObj.GetComponent<Plane>().m_areaNodeList;

        //各エリアの経由地点リスト数分、繰り返す
        for (int i = 0; i < tempAreaNodeList.Length; i++)
        {
            //経由地点数分、繰り返す
            for (int ii = 0; ii < tempAreaNodeList[i].Count; ii++)
            {
                Node tempNode = tempAreaNodeList[i][ii];

                //最寄経由地点を判定した場合、検索対象の経由地点リストを格納
                if (tempNode.m_isPlayer || tempNode.m_isEnemy)
                {
                    m_searchNodeObjList.AddRange(tempAreaNodeList[i]);
                    //Debug.Log($"対象検索エリア：エリア{i + 1}");
                    break;
                }
            }
        }

        //Debug.Log("検索経路地点一覧：" + string.Join(", ", m_searchNodeObjList));
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

            //初回格納以降、且つ、前回座標差合計以上、または、処理中の経由地点が検索対象リストに無い場合、
            if (m_shortNodeObjList.Count != 0 && (m_shortNodePosDis < m_tempNodePosDis ||
                !m_searchNodeObjList.Contains(nextNodeObj)))
            //if (m_shortNodeObjList.Count != 0 && m_shortNodePosDis < m_tempNodePosDis)
            {
                //一時リストに格納した座標情報を削除、次の経由地点処理に遷移する
                m_tempNodeObjList.Remove(nextNodeObj);
                m_tempNodePosDis -= Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis);

                continue;
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

                //Debug.Log("経路ルート一覧：" + string.Join(", ", m_tempNodeObjList) + m_tempNodePosDis);

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

            //探索済み、他の行先経由地点の再読込のため、削除する
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
            Rigidbody rb = GetComponent<Rigidbody>();

            m_timeoutTimer += Time.deltaTime;

            //現在の経由地点に到達するまで、経由地点を更新しない
            if (!m_arriveNode)
            {
                //現在の移動先経由地点と移動方向を取得する
                m_currentNode = m_shortNodeObjList[m_currentTempNodeIndex];
                Vector3 targetDirection = (m_currentNode.transform.position - transform.position).normalized;
                rb.velocity = new Vector3(targetDirection.x * m_moveSpeed, rb.velocity.y, targetDirection.z * m_moveSpeed);

                m_arriveNode = true;
                m_timeoutTimer = 0f;
            }

            // 現在の経由地点に到達した、且つ、制限時間を超過した場合、次の移動先経由地点を設定
            if (Vector3.Distance(transform.position, m_currentNode.transform.position) < m_errorRange || m_timeoutTimer > m_timeoutDuration)
            {
                //移動を停止
                rb.velocity = Vector3.zero;
                m_currentTempNodeIndex++;

                m_arriveNode = false;
                m_timeoutTimer = 0f;
            }
        }
    }
    #endregion


    #region デバッグ機能
    void OnDrawGizmos()
    {
        //プレイヤー（レイキャスト）に向かって青線を描画 
        Gizmos.color = Color.blue;

        Vector3 rayStart = transform.position;
        Vector3 rayDirection = (m_playerTransform.position - transform.position).normalized;
        Gizmos.DrawRay(rayStart, rayDirection * m_detectionRange);
    }
    #endregion
}
