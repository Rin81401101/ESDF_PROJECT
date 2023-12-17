using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Ver2 : MonoBehaviour
{
    List<Node.NodePos> firstNodePosList = new List<Node.NodePos>();    //����̌o�R�n�_���X�g
    List<Node.NodePos> enemyNodePosList = new List<Node.NodePos>();    //�Ŋ�̌o�R�n�_���X�g

    [Header("�ڕW�n�_"), SerializeField] Transform playerTransform;
    [Header("�e�̌o�R�n�_���"), SerializeField] GameObject masterNodeObject;
    [Header("�G�l�~�[�̒T�m�͈�"), SerializeField] float detectionRange = Mathf.Infinity;
    [Header("�����Ԋu����"), SerializeField] float processIntervalTime = 1.0f;

    [Header("�ڕW�̌o�R�n�_"), HideInInspector] public Node enemyNodeObject;
    [Header("�O��̃G�l�~�[�Ŋ�o�R�n�_"), HideInInspector] public Node lastEnemyNode = null;


    void Awake()
    {
        StartCoroutine(GetEnemyNode());     //����o�R�n�_�T������
    }


    void Start()
    {
        //�ŒZ�o�H�T������
    }


    void Update()
    {
        //�ړ�����
    }


    #region ����o�R�n�_�T������
    IEnumerator GetEnemyNode()
    {
        while (true)
        {
            Node fIrstNoseObject = null;  //�o�R�n�_���
            float firstNodePosDisMin = 0; //�o�R�n�_�̍ŒZ���W�ԋ���
            int firstNodeNumMin = 0;      //�o�R�n�_�̍ŒZ�����̗v�f�ԍ�
            RaycastHit hit;

            if (Physics.Raycast(transform.position, (playerTransform.position - transform.position), out hit, detectionRange))
            {
                //�q�̌o�R�n�_�����A�J��Ԃ�
                for (int i = 0; i < masterNodeObject.gameObject.transform.childCount; i++)
                {
                    //�e�o�R�n�_���ƍ��W�ԋ������擾
                    Node.NodePos tempNodePos = new Node.NodePos();

                    //�q�̌o�R�n�_�ƌo�R�n�_���i�G�l�~�[�ԁj���擾
                    tempNodePos.nodeObject = masterNodeObject.gameObject.transform.GetChild(i).gameObject;
                    tempNodePos.nodePosDis = (Vector3.Distance(transform.position,
                                                    tempNodePos.nodeObject.gameObject.transform.position));

                    //����͕K���ێ�
                    if (i == 0)
                    {
                        firstNodePosDisMin = tempNodePos.nodePosDis;
                        firstNodeNumMin = i;
                    }
                    //�ȍ~�͍��W�ԋ������r�A�������Z�����ɍX�V
                    else if (firstNodePosDisMin > tempNodePos.nodePosDis)
                    {
                        firstNodePosDisMin = tempNodePos.nodePosDis;
                        firstNodeNumMin = i;
                    }

                    //���W���A���W�ԋ����̑g���������X�g�Ɋi�[
                    firstNodePosList.Add(tempNodePos);
                }

                //�ڕW�n�_����ł��߂��o�R�n�_���擾
                fIrstNoseObject = firstNodePosList[firstNodeNumMin].nodeObject.GetComponent<Node>();
                Debug.Log("�G�l�~�[�Ŋ�o�R�n�_�F" + fIrstNoseObject);
            }

            yield return new WaitForSeconds(processIntervalTime);
        }
    }
    #endregion
}
