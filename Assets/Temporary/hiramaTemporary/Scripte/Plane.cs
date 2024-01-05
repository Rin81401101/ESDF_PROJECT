using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Plane : MonoBehaviour
{
    [Header("�G���A��"), HideInInspector] private int m_divisions = 2;      //4�G���A�ɕ���
    [Header("�e�G���A�̌o�R�n�_"), HideInInspector] public List<Node>[] m_areaNodeList;


    void Start()
    {
        //�X�e�[�W�̒��S���W�ƃT�C�Y���擾
        Vector3 stageCenter = this.transform.position;
        Vector3 stageSize = this.GetComponent<Renderer>().bounds.size;

        //�X�e�[�W���e�G���A�ɕ���
        float areaSize_X = stageSize.x / m_divisions;
        float areaSize_Z = stageSize.z / m_divisions;

        //�e�G���A�̌o�R�n�_���X�g��������
        m_areaNodeList = new List<Node>[m_divisions * m_divisions];

        for (int i = 0; i < m_areaNodeList.Length; i++)
        {
            m_areaNodeList[i] = new List<Node>();
        }

        for (int i = 0; i < m_divisions; i++)
        {
            for (int ii = 0; ii < m_divisions; ii++)
            {
                //�G���A�̒��S���W���v�Z
                float areaCenter_X = stageCenter.x - (stageSize.x / 2) + (areaSize_X / 2) + i * areaSize_X;
                float areaCenter_Z = stageCenter.z - (stageSize.z / 2) + (areaSize_Z / 2) + ii * areaSize_Z;

                //�G���A���̃m�[�h���擾
                GetAreaNodes(areaCenter_X, areaCenter_Z, areaSize_X, areaSize_Z, i, ii);
            }
        }

    }

    void GetAreaNodes(float areaCenter_X, float areaCenter_Z, float areaSize_X, float areaSize_Z, int areaIndex_X, int areaIndex_Z)
    {
        GameObject[] tempNodeObjs = GameObject.FindGameObjectsWithTag("Node");

        for (int i = 0; i < tempNodeObjs.Length; i++)
        {
            //�G���A���̌o�R�n�_�𔻒�
            Vector3 nodePos = tempNodeObjs[i].transform.position;

            if (nodePos.x >= areaCenter_X - areaSize_X / 2 && nodePos.x < areaCenter_X + areaSize_X / 2 &&
                nodePos.z >= areaCenter_Z - areaSize_Z / 2 && nodePos.z < areaCenter_Z + areaSize_Z / 2)
            {
                //�Ή�����o�R�n�_���X�g��ݒ�
                int areaIndex = areaIndex_X * m_divisions + areaIndex_Z;

                //�o�R�n�_���X�g�Ɋi�[
                Node tempNode = tempNodeObjs[i].gameObject.GetComponent<Node>();
                m_areaNodeList[areaIndex].Add(tempNode);
            }
        }
    }
}