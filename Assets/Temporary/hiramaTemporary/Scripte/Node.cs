using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node : MonoBehaviour
{
    public class NodePos    //経由地点クラス
    {
        public GameObject m_nodeObj;    //各経由地点情報
        public float m_nodePosDis;      //各経由地点の座標間距離
    }

    [Header("次の行先経由地点"), SerializeField] public List<Node> m_nextNodeObjList = new List<Node>();

    [Header("プレイヤーの最寄経由地点"), HideInInspector] public bool m_isPlayer;


    void OnValidate()
    {
        //次の行先経由地点が入力された場合、相互設定を行う
        for (int i = 0; i < m_nextNodeObjList.Count; i++)
        {
            Node nextNode = m_nextNodeObjList[i];
            if (nextNode != null && !nextNode.m_nextNodeObjList.Contains(this))
            {
                nextNode.m_nextNodeObjList.Add(this);
            }
        }
    }


    #region デバッグ機能
    void OnDrawGizmos()
    {
        // 各行先経由地点間に赤線を描画
        Gizmos.color = Color.red;

        foreach (Node nextNode in m_nextNodeObjList)
        {
            if (nextNode != null)
            {
                Gizmos.DrawLine(transform.position, nextNode.transform.position);
            }
        }
    }
    #endregion
}
