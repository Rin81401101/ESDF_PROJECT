using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    List<Node.NodePos> firstNodePosList = new List<Node.NodePos>();    //����̌o�R�n�_���X�g
    List<Node> shortNodeObjList = new List<Node>(); //�ŒZ�o�R�n�_���[�g���X�g
    List<Node> tempNodeObjList = new List<Node>();  //�ꎞ�o�R�n�_���[�g���X�g

    [Header("�ڕW�n�_"), SerializeField] Transform playerTransform;
    [Header("�e�̌o�R�n�_���"), SerializeField] GameObject masterNodeObj;
    [Header("�ړ����x"), SerializeField] float moveSpeed = 3.0f;
    [Header("�����̌덷�͈�"), SerializeField] float errorRange = 0.1f;
    [Header("�����Ԋu����"), SerializeField] public float processIntervalTime = 1.0f;

    [Header("�ŒZ�o�R�n�_�̍��W�����v"), HideInInspector] float shortNodePosDis;
    [Header("�ꎞ�o�R�n�_�̍��W�����v"), HideInInspector] float tempNodePosDis;
    [Header("�ړ��\��̌o�R�n�_�ԍ�"), HideInInspector] int currentTempNodeIndex;
    [Header("�ړ����̌o�R�n�_"), HideInInspector] Node currentNode;
    [Header("�ړ����̌o�R�n�_���W"), HideInInspector] Vector3 currentDis;
    [Header("�o�R�n�_���B�t���O"), HideInInspector] bool arriveNode = false;


    void Start()
    {
        StartCoroutine(ShortestRouteSearch());     //�ŒZ�o�H�T������
    }

    #region �ŒZ�o�H�T������
    IEnumerator ShortestRouteSearch()
    {
        while (true)
        {
            ClearShortestRoute();             //�O��ŒZ�o�H������
            Node nodeObject = GetFirstPos();  //����n�_�擾
            GetShortestRoute(nodeObject);     //�ŒZ�o�H���[�g�擾

            Debug.Log("�ŒZ�o�H�F" + string.Join(", ", shortNodeObjList) + shortNodePosDis);

            yield return new WaitForSeconds(processIntervalTime);
        }
    }
    #endregion

    #region �O��ŒZ�o�H������
    void ClearShortestRoute()
    {
        shortNodeObjList.Clear();
        shortNodePosDis = 0f;
        currentTempNodeIndex = 0;
    }
    #endregion

    #region ����n�_�擾
    Node GetFirstPos()
    {
        Node firstNodeObj = null;    //�o�R�n�_���

        float firstNodePosDisMin = 0;   //�o�R�n�_�̍ŒZ���W�ԋ���
        int firstNodeNumMin = 0;        //�o�R�n�_�̍ŒZ�����̗v�f�ԍ�

        //�q�̌o�R�n�_�����A�J��Ԃ�
        for (int i = 0; i < masterNodeObj.gameObject.transform.childCount; i++)
        {
            Node.NodePos tempNodePos = new Node.NodePos();

            //�e�o�R�n�_���ƍ��W�ԋ������擾
            tempNodePos.nodeObj = masterNodeObj.gameObject.transform.GetChild(i).gameObject;

            Vector3 tempNodePosDis = tempNodePos.nodeObj.gameObject.transform.position;
            tempNodePos.nodePosDis = (Vector3.Distance(transform.position, tempNodePosDis));

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
        firstNodeObj = firstNodePosList[firstNodeNumMin].nodeObj.GetComponent<Node>();
        //Debug.Log("�G�l�~�[�Ŋ�o�R�n�_�F" + firstNodeObj);

        return firstNodeObj;
    }
    #endregion

    #region �ŒZ�o�H���[�g�擾
    void GetShortestRoute(Node nextNodeObj)
    {
        //���̍s��o�R�n�_�����A�������J��Ԃ�
        for (int i = 0; i < nextNodeObj.nextNodeObjList.Count; i++)
        {
            //�ꎞ���X�g�ɍ��W�ƍ��W�����i�[����
            tempNodeObjList.Add(nextNodeObj);

            Vector3 nextNodePosDis = nextNodeObj.nextNodeObjList[i].transform.position;
            if (nextNodeObj.nextNodeObjList[i] != null)
            {
                tempNodePosDis += (Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis));
            }

            //�v���C���[�̍Ŋ�o�R�n�_�܂Ŋi�[�����ꍇ�A
            if (nextNodeObj.isPlayer)
            {
                //����̏ꍇ�A�ŒZ�o�H�Ƃ��Ċi�[����
                if (shortNodeObjList.Count == 0)
                {
                    shortNodeObjList.AddRange(tempNodeObjList);
                    shortNodePosDis = tempNodePosDis;
                }
                //�ȍ~�̏ꍇ�A�ŒZ�o�H�ƈꎞ���X�g���r�A�X�V���s��
                else if (shortNodePosDis > tempNodePosDis)
                {
                    shortNodeObjList.Clear();
                    shortNodeObjList.AddRange(tempNodeObjList);
                    shortNodePosDis = tempNodePosDis;
                }

                //Debug.Log("�o�H�ꗗ�F" + string.Join(", ", tempNodeObjList) + tempNodePosDis);

                //�v���C���[�̍Ŋ�o�R�n�_�f�[�^���폜����
                tempNodeObjList.Remove(nextNodeObj);
                tempNodePosDis -= (Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis));

                return;
            }

            //���̍s��o�R�n�_�����擾����
            Node tempNodeObj = nextNodeObj.nextNodeObjList[i];

            //�擾�f�[�^���ꎞ���X�g���ɖ����ꍇ�A�ċA�֐��Ŏ��̌o�R�n�_���̊K�w�Ɉڂ�
            if (!tempNodeObjList.Contains(tempNodeObj))
            {
                GetShortestRoute(tempNodeObj);
            }

            //�T���ς݁A�܂��́A���̍s��o�R�n�_�̍ēǍ��̂��߁A�폜����
            tempNodeObjList.Remove(nextNodeObj);
            tempNodePosDis -= Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis);
        }
    }
    #endregion


    void Update()
    {
        MoveShortestRoute();    //�ŒZ�o�H�ړ�����
    }

    #region �ŒZ�o�H�ړ�����
    void MoveShortestRoute()
    {
        if (currentTempNodeIndex < shortNodeObjList.Count)
        {
            //���݂̌o�R�n�_�ɓ��B����܂ŁA�o�R�n�_���X�V���Ȃ�
            if (!arriveNode)
            {
                //���݂̈ړ���o�R�n�_�ƈړ��������擾����
                currentNode = shortNodeObjList[currentTempNodeIndex];
                currentDis = (currentNode.transform.position - transform.position).normalized;
                arriveNode = true;
            }

            //���݂̈ړ���o�R�n�_�Ɉړ�����
            transform.Translate(currentDis * moveSpeed * Time.deltaTime);

            //���݂̌o�R�n�_�ɓ��B�����ꍇ�A���̈ړ���o�R�n�_��ݒ�
            if (Vector3.Distance(transform.position, currentNode.transform.position) < errorRange)
            {
                arriveNode = false;
                currentTempNodeIndex++;
            }
        }
    }
    #endregion
}
