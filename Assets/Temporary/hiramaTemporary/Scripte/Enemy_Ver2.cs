using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Ver2 : MonoBehaviour
{
    List<Node.NodePos> firstNodePosList = new List<Node.NodePos>();    //初回の経由地点リスト
    List<Node.NodePos> enemyNodePosList = new List<Node.NodePos>();    //最寄の経由地点リスト

    [Header("目標地点"), SerializeField] Transform playerTransform;
    [Header("親の経由地点情報"), SerializeField] GameObject masterNodeObject;
    [Header("エネミーの探知範囲"), SerializeField] float detectionRange = Mathf.Infinity;
    [Header("処理間隔時間"), SerializeField] float processIntervalTime = 1.0f;

    [Header("目標の経由地点"), HideInInspector] public Node enemyNodeObject;
    [Header("前回のエネミー最寄経由地点"), HideInInspector] public Node lastEnemyNode = null;


    void Awake()
    {
        StartCoroutine(GetEnemyNode());     //初回経由地点探索処理
    }


    void Start()
    {
        //最短経路探索処理
    }


    void Update()
    {
        //移動処理
    }


    #region 初回経由地点探索処理
    IEnumerator GetEnemyNode()
    {
        while (true)
        {
            Node fIrstNoseObject = null;  //経由地点情報
            float firstNodePosDisMin = 0; //経由地点の最短座標間距離
            int firstNodeNumMin = 0;      //経由地点の最短距離の要素番号
            RaycastHit hit;

            if (Physics.Raycast(transform.position, (playerTransform.position - transform.position), out hit, detectionRange))
            {
                //子の経由地点数分、繰り返す
                for (int i = 0; i < masterNodeObject.gameObject.transform.childCount; i++)
                {
                    //各経由地点情報と座標間距離を取得
                    Node.NodePos tempNodePos = new Node.NodePos();

                    //子の経由地点と経由地点差（エネミー間）を取得
                    tempNodePos.nodeObject = masterNodeObject.gameObject.transform.GetChild(i).gameObject;
                    tempNodePos.nodePosDis = (Vector3.Distance(transform.position,
                                                    tempNodePos.nodeObject.gameObject.transform.position));

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
                    firstNodePosList.Add(tempNodePos);
                }

                //目標地点から最も近い経由地点を取得
                fIrstNoseObject = firstNodePosList[firstNodeNumMin].nodeObject.GetComponent<Node>();
                Debug.Log("エネミー最寄経由地点：" + fIrstNoseObject);
            }

            yield return new WaitForSeconds(processIntervalTime);
        }
    }
    #endregion
}
