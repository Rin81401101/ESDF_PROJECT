using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Plane : MonoBehaviour
{
    [Header("エリア数"), HideInInspector] private int m_divisions = 2;      //4エリアに分割
    [Header("各エリアの経由地点"), HideInInspector] public List<Node>[] m_areaNodeList;


    void Start()
    {
        //ステージの中心座標とサイズを取得
        Vector3 stageCenter = this.transform.position;
        Vector3 stageSize = this.GetComponent<Renderer>().bounds.size;

        //ステージを各エリアに分割
        float areaSize_X = stageSize.x / m_divisions;
        float areaSize_Z = stageSize.z / m_divisions;

        //各エリアの経由地点リストを初期化
        m_areaNodeList = new List<Node>[m_divisions * m_divisions];

        for (int i = 0; i < m_areaNodeList.Length; i++)
        {
            m_areaNodeList[i] = new List<Node>();
        }

        for (int i = 0; i < m_divisions; i++)
        {
            for (int ii = 0; ii < m_divisions; ii++)
            {
                //エリアの中心座標を計算
                float areaCenter_X = stageCenter.x - (stageSize.x / 2) + (areaSize_X / 2) + i * areaSize_X;
                float areaCenter_Z = stageCenter.z - (stageSize.z / 2) + (areaSize_Z / 2) + ii * areaSize_Z;

                //エリア内のノードを取得
                GetAreaNodes(areaCenter_X, areaCenter_Z, areaSize_X, areaSize_Z, i, ii);
            }
        }

    }

    void GetAreaNodes(float areaCenter_X, float areaCenter_Z, float areaSize_X, float areaSize_Z, int areaIndex_X, int areaIndex_Z)
    {
        GameObject[] tempNodeObjs = GameObject.FindGameObjectsWithTag("Node");

        for (int i = 0; i < tempNodeObjs.Length; i++)
        {
            //エリア内の経由地点を判定
            Vector3 nodePos = tempNodeObjs[i].transform.position;

            if (nodePos.x >= areaCenter_X - areaSize_X / 2 && nodePos.x < areaCenter_X + areaSize_X / 2 &&
                nodePos.z >= areaCenter_Z - areaSize_Z / 2 && nodePos.z < areaCenter_Z + areaSize_Z / 2)
            {
                //対応する経由地点リストを設定
                int areaIndex = areaIndex_X * m_divisions + areaIndex_Z;

                //経由地点リストに格納
                Node tempNode = tempNodeObjs[i].gameObject.GetComponent<Node>();
                m_areaNodeList[areaIndex].Add(tempNode);
            }
        }
    }
}