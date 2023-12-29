using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    List<Node.NodePos> m_firstNodePosList = new List<Node.NodePos>();    //����̌o�R�n�_���X�g
    List<Node> m_shortNodeObjList = new List<Node>(); //�ŒZ�o�R�n�_���[�g���X�g
    List<Node> m_tempNodeObjList = new List<Node>();  //�ꎞ�o�R�n�_���[�g���X�g

    [Header("�ڕW�n�_"), SerializeField] Transform m_playerTransform;
    [Header("�e�̌o�R�n�_���"), SerializeField] GameObject m_masterNodeObj;
    [Header("�ړ����x"), SerializeField] float m_moveSpeed = 3.0f;
    [Header("�T���͈�"), SerializeField] float m_detectionRange = 50.0f;
    [Header("�T�����x"), SerializeField] float m_detectionHeight = 3.0f;
    [Header("���B�덷�͈�"), SerializeField] float m_errorRange = 0.1f;
    [Header("�����Ԋu����"), SerializeField] public float m_processIntervalTime = 1.0f;
    [Header("�ړ��̐�������"), SerializeField] float m_timeoutDuration = 10.0f;

    [Header("�ŒZ�o�R�n�_�̍��W�����v"), HideInInspector] float m_shortNodePosDis;
    [Header("�ꎞ�o�R�n�_�̍��W�����v"), HideInInspector] float m_tempNodePosDis;
    [Header("�ړ��\��̌o�R�n�_�ԍ�"), HideInInspector] int m_currentTempNodeIndex;
    [Header("�ړ����̌o�R�n�_"), HideInInspector] Node m_currentNode;
    [Header("�ړ����̌o�R�n�_���W"), HideInInspector] Vector3 m_currentDis;
    [Header("�o�R�n�_���B�t���O"), HideInInspector] bool m_arriveNode = false;
    [Header("�������ԗp�^�C�}�["), HideInInspector] float m_timeoutTimer = 0f;
    [Header("�o�R�n�_�̒T�m�p�x"), HideInInspector] float m_enemyVisio = 180f;


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

            Debug.Log("�ŒZ�o�H�F" + string.Join(", ", m_shortNodeObjList) + m_shortNodePosDis);

            yield return new WaitForSeconds(m_processIntervalTime);
        }
    }
    #endregion

    #region �O��ŒZ�o�H������
    void ClearShortestRoute()
    {
        m_shortNodeObjList.Clear();
        m_shortNodePosDis = 0f;
        m_currentTempNodeIndex = 0;
    }
    #endregion

    #region ����n�_�擾
    Node GetFirstPos()
    {
        Node firstNodeObj = null;       //�o�R�n�_���
        float firstNodePosDisMin = 0;   //�o�R�n�_�̍ŒZ���W�ԋ���
        int firstNodeNumMin = 0;        //�o�R�n�_�̍ŒZ�����̗v�f�ԍ�

        Vector3 rayDirection = m_playerTransform.position - transform.position;

        if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, m_detectionRange))
        {
            //�v���C���[�����F�A���A���̍��x(Y��)���Ŏ��F�����ꍇ�A
            float heightDis = Mathf.Abs(m_playerTransform.position.y - transform.position.y);

            if (hit.collider.CompareTag("Player") && heightDis <= m_detectionHeight)
            {
                //�v���C���[����ł��߂��o�R�n�_���擾
                GameObject playerNodeObj = GameObject.FindWithTag("Player");
                firstNodeObj = playerNodeObj.GetComponent<Player>().m_playerNodeObj;

                //Debug.Log("�v���C���[����");
            }
            //�v���C���[�����F�ł��Ȃ������ꍇ�A
            else
            {
                //�q�̌o�R�n�_�����A�J��Ԃ�
                for (int i = 0; i < m_masterNodeObj.gameObject.transform.childCount; i++)
                {
                    Node.NodePos tempNodePos = new Node.NodePos();

                    //�e�o�R�n�_���ƍ��W�ԋ������擾
                    tempNodePos.nodeObj = m_masterNodeObj.gameObject.transform.GetChild(i).gameObject;

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
                    m_firstNodePosList.Add(tempNodePos);
                }

                //���g����ł��߂��o�R�n�_���擾
                firstNodeObj = m_firstNodePosList[firstNodeNumMin].nodeObj.GetComponent<Node>();

                //Debug.Log("��Q������");
            }
        }

        //Debug.Log("�G�l�~�[�Ŋ�o�R�n�_�F" + firstNodeObj);

        return firstNodeObj;
    }
    #endregion

    #region �ŒZ�o�H���[�g�擾
    void GetShortestRoute(Node nextNodeObj)
    {
        //���̍s��o�R�n�_�����A�������J��Ԃ�
        for (int i = 0; i < nextNodeObj.m_nextNodeObjList.Count; i++)
        {
            //�ꎞ���X�g�ɍ��W�ƍ��W�����i�[����
            m_tempNodeObjList.Add(nextNodeObj);

            Vector3 nextNodePosDis = nextNodeObj.m_nextNodeObjList[i].transform.position;
            if (nextNodeObj.m_nextNodeObjList[i] != null)
            {
                m_tempNodePosDis += (Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis));
            }

            //����ڍs�̏ꍇ(�œK������)�A
            if (m_shortNodeObjList.Count != 0)
            {
                //�G�l�~�[���猩���ڕW�n�_�ƌo�R�n�_�̃x�N�g�����擾���A��_�̓��Ϙa���擾����(Y���W�͖���)
                Vector3 toPlayer = (m_playerTransform.position - transform.position).normalized; toPlayer.y = 0;
                Vector3 toNode = (nextNodeObj.transform.position - transform.position).normalized; toNode.y = 0;
                float dot = Vector3.Dot(toNode, toPlayer);

                //���ɍŒZ�o�H�̍��W�����v�ȏ�A�܂��A�Ώۂ̌o�R�n�_���G�l�~�[�̔��Ε����������ꍇ�A
                if (m_shortNodePosDis < m_tempNodePosDis || dot < Mathf.Cos(Mathf.Deg2Rad * (m_enemyVisio / 2f)))
                //if (m_shortNodePosDis < m_tempNodePosDis)
                {
                    //�ꎞ���X�g�Ɋi�[�������W�����폜�A���̌o�R�n�_�����ɑJ�ڂ���
                    m_tempNodeObjList.Remove(nextNodeObj);
                    m_tempNodePosDis -= Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis);

                    continue;
                }
            }

            //�v���C���[�̍Ŋ�o�R�n�_�܂Ŋi�[�����ꍇ�A
            if (nextNodeObj.m_isPlayer)
            {
                //����̏ꍇ�A�ŒZ�o�H�Ƃ��Ċi�[����
                if (m_shortNodeObjList.Count == 0)
                {
                    m_shortNodeObjList.AddRange(m_tempNodeObjList);
                    m_shortNodePosDis = m_tempNodePosDis;
                }
                //�ȍ~�̏ꍇ�A�ŒZ�o�H�ƈꎞ���X�g���r�A�X�V���s��
                else if (m_shortNodePosDis > m_tempNodePosDis)
                {
                    m_shortNodeObjList.Clear();
                    m_shortNodeObjList.AddRange(m_tempNodeObjList);
                    m_shortNodePosDis = m_tempNodePosDis;
                }

                Debug.Log("�o�H�ꗗ�F" + string.Join(", ", m_tempNodeObjList) + m_tempNodePosDis);

                //�v���C���[�̍Ŋ�o�R�n�_�f�[�^���폜����
                m_tempNodeObjList.Remove(nextNodeObj);
                m_tempNodePosDis -= (Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis));

                return;
            }

            //���̍s��o�R�n�_�����擾����
            Node tempNodeObj = nextNodeObj.m_nextNodeObjList[i];

            //�擾�f�[�^���ꎞ���X�g���ɖ����ꍇ�A�ċA�֐��Ŏ��̌o�R�n�_���̊K�w�Ɉڂ�
            if (!m_tempNodeObjList.Contains(tempNodeObj))
            {
                GetShortestRoute(tempNodeObj);
            }

            //�T���ς݁A���̍s��o�R�n�_�̍ēǍ��̂��߁A�폜����
            m_tempNodeObjList.Remove(nextNodeObj);
            m_tempNodePosDis -= Vector3.Distance(nextNodeObj.transform.position, nextNodePosDis);
        }
    }
    #endregion


    void Update()
    {
        //MoveShortestRoute();    //�ŒZ�o�H�ړ�����
    }

    #region �ŒZ�o�H�ړ�����
    void MoveShortestRoute()
    {
        if (m_currentTempNodeIndex < m_shortNodeObjList.Count)
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            m_timeoutTimer += Time.deltaTime;

            //���݂̌o�R�n�_�ɓ��B����܂ŁA�o�R�n�_���X�V���Ȃ�
            if (!m_arriveNode)
            {
                //���݂̈ړ���o�R�n�_�ƈړ��������擾����
                m_currentNode = m_shortNodeObjList[m_currentTempNodeIndex];
                Vector3 targetDirection = (m_currentNode.transform.position - transform.position).normalized;
                rb.velocity = new Vector3(targetDirection.x * m_moveSpeed, rb.velocity.y, targetDirection.z * m_moveSpeed);

                m_arriveNode = true;
                m_timeoutTimer = 0f;
            }

            // ���݂̌o�R�n�_�ɓ��B�����A���A�������Ԃ𒴉߂����ꍇ�A���̈ړ���o�R�n�_��ݒ�
            if (Vector3.Distance(transform.position, m_currentNode.transform.position) < m_errorRange || m_timeoutTimer > m_timeoutDuration)
            {
                //�ړ����~
                rb.velocity = Vector3.zero;
                m_currentTempNodeIndex++;

                m_arriveNode = false;
                m_timeoutTimer = 0f;
            }
        }
    }
    #endregion


    #region �f�o�b�O�@�\
    void OnDrawGizmos()
    {
        //�v���C���[�i���C�L���X�g�j�Ɍ������Đ���`�� 
        Gizmos.color = Color.blue;

        Vector3 rayStart = transform.position;
        Vector3 rayDirection = (m_playerTransform.position - transform.position).normalized;
        Gizmos.DrawRay(rayStart, rayDirection * m_detectionRange);
    }
    #endregion
}
