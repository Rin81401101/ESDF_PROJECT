using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Ver2 : MonoBehaviour
{
    List<Node.NodePos> firstNodePosList = new List<Node.NodePos>();    //初回の経由地点リスト
    List<Node> shortNodeObjList = new List<Node>(); //最短経由地点ルートリスト
    List<Node> tempNodeObjList = new List<Node>();  //一時経由地点ルートリスト

    [Header("目標地点"), SerializeField] Transform playerTransform;
    [Header("親の経由地点情報"), SerializeField] GameObject masterNodeObj;
    [Header("処理間隔時間"), SerializeField] float processIntervalTime = 1.0f;

    [Header("最短経由地点の座標差合計"), HideInInspector] float shortNodePosDis;
    [Header("一時経由地点の座標差合計"), HideInInspector] float tempNodePosDis;


    void Start()
    {
        StartCoroutine(GetShortestRouteSearch());     //最短経路探索処理
    }


    //void Update()
    //{
    //    //移動処理
    //    if (routeSearchFLG)
    //    {
    //        //経路探索移動の場合
    //    }
    //    else
    //    {
    //        //直線移動の場合
    //    }
    //}


    #region 最短経路探索処理
    IEnumerator GetShortestRouteSearch()
    {
        while (true)
        {
            ClearShortestRoute();             //前回最短経路初期化
            Node nodeObject = GetFirstPos();  //初回地点取得
            GetShortestRoute(nodeObject);     //最短経路ルート取得

            Debug.Log("最短経路：" + string.Join(", ", shortNodeObjList) + shortNodePosDis);

            yield return new WaitForSeconds(processIntervalTime);
        }
    }
    #endregion

    #region 前回最短経路初期化
    void ClearShortestRoute()
    {
        shortNodeObjList.Clear();
        shortNodePosDis = 0f;
    }
    #endregion

    #region 初回地点取得
    Node GetFirstPos()
    {
        Node firstNodeObj = null;    //経由地点情報

        float firstNodePosDisMin = 0;   //経由地点の最短座標間距離
        int firstNodeNumMin = 0;        //経由地点の最短距離の要素番号

        //子の経由地点数分、繰り返す
        for (int i = 0; i < masterNodeObj.gameObject.transform.childCount; i++)
        {
            Node.NodePos tempNodePos = new Node.NodePos();

            //各経由地点情報と座標間距離を取得
            tempNodePos.nodeObj = masterNodeObj.gameObject.transform.GetChild(i).gameObject;

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
            firstNodePosList.Add(tempNodePos);
        }

        //目標地点から最も近い経由地点を取得
        firstNodeObj = firstNodePosList[firstNodeNumMin].nodeObj.GetComponent<Node>();
        //Debug.Log("エネミー最寄経由地点：" + firstNodeObj);

        return firstNodeObj;
    }
    #endregion

    #region 最短経路ルート取得
    void GetShortestRoute(Node nextNodeObj)
    {
        //次の行先経由地点数分、処理を繰り返す
        for (int i = 0; i < nextNodeObj.nextNodeObjList.Count; i++)
        {
            //一時リストに座標と座標差を格納する
            tempNodeObjList.Add(nextNodeObj);

            Vector3 nextNodePosDis = nextNodeObj.nextNodeObjList[i].transform.position;
            if (nextNodeObj.nextNodeObjList[i] != null)
            {
                tempNodePosDis += (Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis));
            }

            //プレイヤーの最寄経由地点まで格納した場合、
            if (nextNodeObj.isPlayer)
            {
                //初回の場合、最短経路として格納する
                if (shortNodeObjList.Count == 0)
                {
                    shortNodeObjList.AddRange(tempNodeObjList);
                    shortNodePosDis = tempNodePosDis;
                }
                //以降の場合、最短経路と一時リストを比較、更新を行う
                else if (shortNodePosDis > tempNodePosDis)
                {
                    shortNodeObjList.Clear();
                    shortNodeObjList.AddRange(tempNodeObjList);
                    shortNodePosDis = tempNodePosDis;
                }

                //Debug.Log("経路一覧：" + string.Join(", ", tempNodeObjList) + tempNodePosDis);

                //プレイヤーの最寄経由地点データを削除する
                tempNodeObjList.Remove(nextNodeObj);
                tempNodePosDis -= (Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis));

                return;
            }

            //次の行先経由地点を仮取得する
            Node tempNodeObj = nextNodeObj.nextNodeObjList[i];

            //取得データが一時リスト内に無い場合、再帰関数で次の経由地点情報の階層に移る
            if (!tempNodeObjList.Contains(tempNodeObj))
            {
                GetShortestRoute(tempNodeObj);
            }

            //探索済み、または、他の行先経由地点の再読込のため、削除する
            tempNodeObjList.Remove(nextNodeObj);
            tempNodePosDis -= Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis);
        }
    }
    #endregion
}
