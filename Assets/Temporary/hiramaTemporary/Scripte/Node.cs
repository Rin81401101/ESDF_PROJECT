using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node : MonoBehaviour
{
    public class NodePos    //�o�R�n�_�N���X
    {
        public GameObject m_nodeObj;    //�e�o�R�n�_���
        public float m_nodePosDis;      //�e�o�R�n�_�̍��W�ԋ���
    }

    [Header("���̍s��o�R�n�_"), SerializeField] public List<Node> m_nextNodeObjList = new List<Node>();

    [Header("�v���C���[�̍Ŋ�o�R�n�_"), HideInInspector] public bool m_isPlayer;


    void OnValidate()
    {
        //���̍s��o�R�n�_�����͂��ꂽ�ꍇ�A���ݐݒ���s��
        for (int i = 0; i < m_nextNodeObjList.Count; i++)
        {
            Node nextNode = m_nextNodeObjList[i];
            if (nextNode != null && !nextNode.m_nextNodeObjList.Contains(this))
            {
                nextNode.m_nextNodeObjList.Add(this);
            }
        }
    }


    #region �f�o�b�O�@�\
    void OnDrawGizmos()
    {
        // �e�s��o�R�n�_�ԂɐԐ���`��
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
