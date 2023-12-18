using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    List<Node.NodePos> playerNodePosList = new List<Node.NodePos>();     //目標の経由地点リスト

    [Header("親の経由地点情報"), SerializeField] GameObject masterNodeObj;
    [Header("処理間隔時間"), SerializeField] float processIntervalTime = 1.0f;

    [Header("目標の経由地点"), HideInInspector] public Node playerNodeObj;
    [Header("前回のプレイヤー最寄経由地点"), HideInInspector] public Node lastPlayerNode = null;


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
            if (lastPlayerNode != null)
            {
                lastPlayerNode.isPlayer = false;
            }

            playerNodeObj = null;           //経由地点情報
            float playerNodePosDisMin = 0;  //経由地点の最短座標間距離
            int playerNodeNumMin = 0;       //経由地点の最短距離の要素番号

            //子の経由地点数分、繰り返す
            for (int i = 0; i < masterNodeObj.gameObject.transform.childCount; i++)
            {
                Node.NodePos tempNodePos = new Node.NodePos();

                //各経由地点情報と座標間距離取得
                tempNodePos.nodeObj = masterNodeObj.gameObject.transform.GetChild(i).gameObject;

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
                playerNodePosList.Add(tempNodePos);
            }

            //目標地点から最も近い経由地点を取得
            playerNodeObj = playerNodePosList[playerNodeNumMin].nodeObj.GetComponent<Node>();

            //プレイヤーの最寄経由地点を立てる
            if (playerNodeObj != null)
            {
                playerNodeObj.isPlayer = true;
                lastPlayerNode = playerNodeObj;
                //Debug.Log("プレイヤー最寄経由地点：" + playerNodeObj);
            }

            yield return new WaitForSeconds(processIntervalTime);
        }
    }
    #endregion
}
