using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    List<Node.NodePos> m_playerNodePosList = new List<Node.NodePos>();     //�ڕW�̌o�R�n�_���X�g

    [Header("�e�̌o�R�n�_���"), SerializeField] GameObject m_masterNodeObj;
    [Header("�v���C���[�̈ړ����x"), SerializeField] float m_moveSpeed = 5.0f;
    [Header("�����Ԋu����"), SerializeField] float m_processIntervalTime = 1.0f;

    [Header("�ڕW�̌o�R�n�_"), HideInInspector] public Node m_playerNodeObj;
    [Header("�O��̃v���C���[�Ŋ�o�R�n�_"), HideInInspector] public Node m_lastPlayerNode = null;


    void Awake()
    {
        StartCoroutine(GetPlayerNode());    //�ڕW�o�R�n�_�T������
    }

    #region �ڕW�o�R�n�_�T������
    IEnumerator GetPlayerNode()
    {
        while (true)
        {
            //�O��̃v���C���[�̍Ŋ�o�R�n�_��߂�
            if (m_lastPlayerNode != null)
            {
                m_lastPlayerNode.m_isPlayer = false;
            }

            m_playerNodeObj = null;           //�o�R�n�_���
            float playerNodePosDisMin = 0;  //�o�R�n�_�̍ŒZ���W�ԋ���
            int playerNodeNumMin = 0;       //�o�R�n�_�̍ŒZ�����̗v�f�ԍ�

            //�q�̌o�R�n�_�����A�J��Ԃ�
            for (int i = 0; i < m_masterNodeObj.gameObject.transform.childCount; i++)
            {
                Node.NodePos tempNodePos = new Node.NodePos();

                //�e�o�R�n�_���ƍ��W�ԋ����擾
                tempNodePos.nodeObj = m_masterNodeObj.gameObject.transform.GetChild(i).gameObject;

                Vector3 tempNodePosDis = tempNodePos.nodeObj.gameObject.transform.position;
                tempNodePos.nodePosDis = (Vector3.Distance(transform.position, tempNodePosDis));

                //����͕K���ێ�
                if (i == 0)
                {
                    playerNodePosDisMin = tempNodePos.nodePosDis;
                    playerNodeNumMin = i;
                }
                //�ȍ~�͍��W�ԋ������r�A�������Z�����ɍX�V
                else
                {
                    if (playerNodePosDisMin > tempNodePos.nodePosDis)
                    {
                        playerNodePosDisMin = tempNodePos.nodePosDis;
                        playerNodeNumMin = i;
                    }
                }

                //���W���A���W�ԋ����̑g���������X�g�Ɋi�[
                m_playerNodePosList.Add(tempNodePos);
            }

            //�ڕW�n�_����ł��߂��o�R�n�_���擾
            m_playerNodeObj = m_playerNodePosList[playerNodeNumMin].nodeObj.GetComponent<Node>();

            //�v���C���[�̍Ŋ�o�R�n�_�𗧂Ă�
            if (m_playerNodeObj != null)
            {
                m_playerNodeObj.m_isPlayer = true;
                m_lastPlayerNode = m_playerNodeObj;
                //Debug.Log("�v���C���[�Ŋ�o�R�n�_�F" + playerNodeObj);
            }

            yield return new WaitForSeconds(m_processIntervalTime);
        }
    }
    #endregion


    void Update()
    {
        // "WASD"�̓��͂��擾����
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // ���͒l����ړ��x�N�g�����쐬���A�ړ�����
        Vector3 move = new Vector3(horizontalInput, 0f, verticalInput) * m_moveSpeed * Time.deltaTime;
        transform.Translate(move);
    }
}
