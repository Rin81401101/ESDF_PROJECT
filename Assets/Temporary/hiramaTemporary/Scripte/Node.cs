using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node : MonoBehaviour
{
    public class NodePos    //�o�R�n�_�N���X
    {
        public GameObject nodeObj;    //�e�o�R�n�_���
        public float nodePosDis;      //�e�o�R�n�_�̍��W�ԋ���
    }

    [Header("���̍s��o�R�n�_"), SerializeField] public List<Node> m_nextNodeObjList = new List<Node>();

    [Header("�v���C���[�̍Ŋ�o�R�n�_"),HideInInspector] public bool m_isPlayer;


    void Reset()
    {
        //�o�R�n�_��񃊃X�g�������A��̗v�f�𐶐�����
        m_nextNodeObjList.Add(null);
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
