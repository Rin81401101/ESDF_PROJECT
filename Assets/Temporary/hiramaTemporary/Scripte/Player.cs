using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    List<Node.NodePos> playerNodePosList = new List<Node.NodePos>();     //�ڕW�̌o�R�n�_���X�g

    [Header("�e�̌o�R�n�_���"), SerializeField] GameObject masterNodeObj;
    [Header("�����Ԋu����"), SerializeField] float processIntervalTime = 1.0f;

    [Header("�ڕW�̌o�R�n�_"), HideInInspector] public Node playerNodeObj;
    [Header("�O��̃v���C���[�Ŋ�o�R�n�_"), HideInInspector] public Node lastPlayerNode = null;


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
            if (lastPlayerNode != null)
            {
                lastPlayerNode.isPlayer = false;
            }

            playerNodeObj = null;           //�o�R�n�_���
            float playerNodePosDisMin = 0;  //�o�R�n�_�̍ŒZ���W�ԋ���
            int playerNodeNumMin = 0;       //�o�R�n�_�̍ŒZ�����̗v�f�ԍ�

            //�q�̌o�R�n�_�����A�J��Ԃ�
            for (int i = 0; i < masterNodeObj.gameObject.transform.childCount; i++)
            {
                Node.NodePos tempNodePos = new Node.NodePos();

                //�e�o�R�n�_���ƍ��W�ԋ����擾
                tempNodePos.nodeObj = masterNodeObj.gameObject.transform.GetChild(i).gameObject;

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
                playerNodePosList.Add(tempNodePos);
            }

            //�ڕW�n�_����ł��߂��o�R�n�_���擾
            playerNodeObj = playerNodePosList[playerNodeNumMin].nodeObj.GetComponent<Node>();

            //�v���C���[�̍Ŋ�o�R�n�_�𗧂Ă�
            if (playerNodeObj != null)
            {
                playerNodeObj.isPlayer = true;
                lastPlayerNode = playerNodeObj;
                //Debug.Log("�v���C���[�Ŋ�o�R�n�_�F" + playerNodeObj);
            }

            yield return new WaitForSeconds(processIntervalTime);
        }
    }
    #endregion
}
