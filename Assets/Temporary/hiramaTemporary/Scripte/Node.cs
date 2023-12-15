using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node : MonoBehaviour
{
    public class NodePos    //経由地点クラス
    {
        public GameObject nodeObject; //各経由地点情報
        public float nodePosDis;      //各経由地点の座標間距離
    }

    [Header("次の行先経由地点"), SerializeField] public List<Node> nextNodeObjList = new List<Node>();

    [Header("プレイヤーの最寄経由地点"), SerializeField] public bool isPlayer;


    void Reset()
    {
        //経由地点情報リスト生成時、空の要素を生成する
        nextNodeObjList.Add(null);
    }


    #region デバッグ機能
    void OnDrawGizmos()
    {
        // 各、次の行先経由地点との赤線を描画
        Gizmos.color = Color.red;

        foreach (Node nextNode in nextNodeObjList)
        {
            if (nextNode != null)
            {
                Gizmos.DrawLine(transform.position, nextNode.transform.position);
            }
        }
    }
    #endregion
}
