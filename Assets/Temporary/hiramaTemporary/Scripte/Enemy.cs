using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    List<Node.NodePos> firstNodePosList = new List<Node.NodePos>();    //初回の経由地点リスト
    List<Node> shortNodeObjectList = new List<Node>(); //最短経由地点ルートリスト
    List<Node> tempNodeObjectList = new List<Node>();  //一時経由地点ルートリスト

    [Header("目標地点"), SerializeField] Transform playerTransform;
    [Header("親の経由地点情報"), SerializeField] GameObject masterNodeObject;
    [Header("探知範囲"), SerializeField] float detectionRange = Mathf.Infinity;
    //[Header("移動スピード"), SerializeField] float speed = 2f;
    //[Header("座標の許容誤差値"), SerializeField] float tolLevPos = 0.01f;

    [Header("初回の経由地点"), HideInInspector] public Node firstNodeObject;
    [Header("最短経由地点の座標差合計"), HideInInspector] float shortNodePosDis;
    [Header("一時経由地点の座標差合計"), HideInInspector] float tempNodePosDis;
    //[Header("次の移動座標"), HideInInspector] Transform nextNodeTransform;


    void Start()
    {
        firstNodeObject = GetFirstNode(); //初回経由地点探索処理
        GetNextNode(firstNodeObject);     //経由地点ルート探索処理

        Debug.Log("最短経路：" + string.Join(", ", shortNodeObjectList) + shortNodePosDis);
    }


    void Update()
    {
        #region
        ////目的地点に移動
        //transform.position = Vector3.MoveTowards(
        //        transform.position,
        //        new Vector3(nextNodeTransform.position.x,
        //            nextNodeTransform.position.y,
        //            nextNodeTransform.position.z),
        //            speed * Time.deltaTime);

        ////現在地点の座標差を計算
        //float disNextPos = Vector3.Distance(transform.position, nextNodeTransform.position);

        //// 次移動先ノードを更新
        //if (disNextPos < tolLevPos)
        //{
        //    Node node = nextNodeTransform.GetComponent<Node>();
        //    //nextNodeTransform = node.nextNodeTransform[0];
        //}
        #endregion
    }


    #region 初回経由地点探索処理
    Node GetFirstNode()
    {
        Node fIrstNoseObject = null;  //経由地点情報
        float firstNodePosDisMin = 0; //経由地点の最短座標間距離
        int firstNodeNumMin = 0;      //経由地点の最短距離の要素番号

        //目標地点にレイを飛ばし、
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

        return fIrstNoseObject;
    }
    #endregion


    #region 経由地点ルート探索処理
    void GetNextNode(Node nextNodeObject)
    {
        //次の座標地点に格納されている要素数分、繰り返す
        for (int i = 0; i < nextNodeObject.nextNodeObjList.Count; i++)
        {

            //一時リストに座標情報と座標間距離を格納
            tempNodeObjectList.Add(nextNodeObject);
            if (nextNodeObject.nextNodeObjList[i] != null)
            {
                tempNodePosDis += Vector3.Distance(nextNodeObject.transform.position,
                                                    nextNodeObject.nextNodeObjList[i].transform.position);
            }

            //最終地点の場合、最短経路リストとの比較、更新を行う
            if (nextNodeObject.isPlayer)
            {
                //最短経路リストが空の場合、初回格納を行う
                if (shortNodeObjectList.Count == 0)
                {
                    shortNodeObjectList.AddRange(tempNodeObjectList);
                    shortNodePosDis = tempNodePosDis;
                }
                //初回格納以降の場合、最短経路リストの更新を行う
                else
                {
                    ////最短経路リストと一時リストの要素数が一致しない場合、要素数を合わせる
                    //if (shortNodeObjectList.Count > tempNodeObjectList.Count)
                    //{
                    //    //不足している要素数を算出、不足箇所に要素番号を最短経路リストから挿入
                    //    int elementsToAdd = shortNodeObjectList.Count - tempNodeObjectList.Count;

                    //    for (int ii = 0; ii < elementsToAdd; ii++)
                    //    {
                    //        tempNodeObjectList.Insert(ii, shortNodeObjectList[ii]);
                    //    }
                    //}
                    //現在の座標間距離合計の方が距離が短い場合、最短座標間距離合計を更新する
                    if (shortNodePosDis > tempNodePosDis)
                    {
                        shortNodeObjectList.Clear();
                        shortNodeObjectList.AddRange(tempNodeObjectList);
                        shortNodePosDis = tempNodePosDis;
                    }
                }

                Debug.Log("経路一覧：" + string.Join(", ", tempNodeObjectList) + tempNodePosDis);

                //プレイヤー最寄経由地点のみを削除する
                tempNodeObjectList.Remove(nextNodeObject);
                tempNodePosDis -= Vector3.Distance(nextNodeObject.transform.position,
                                    nextNodeObject.nextNodeObjList[i].transform.position);
                ////一時リスト、座標間距離をクリア
                //tempNodeObjectList.Clear();
                //tempNodePosDis = 0;

                return;
            }

            //次の移動座標を取得する
            Node temp = nextNodeObject.nextNodeObjList[i];

            //次の経由地点を取得できなかった場合、一時リスト、座標間距離を初期化して終了する
            if (temp == null)
            {
                tempNodeObjectList.Clear();
                tempNodePosDis = 0;

                return;
            }
            //
            else if(!tempNodeObjectList.Contains(temp))
            {
                GetNextNode(temp);
            }

            //
            tempNodeObjectList.Remove(nextNodeObject);
            tempNodePosDis -= Vector3.Distance(nextNodeObject.transform.position,
                    nextNodeObject.nextNodeObjList[i].transform.position);
        }
    }
    #endregion
}
