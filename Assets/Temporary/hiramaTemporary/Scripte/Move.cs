using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("�ړ����x"), SerializeField] float moveSpeed = 3.0f;


    void Update()
    {
        // "WASD"�̓��͂��擾����
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // ���͒l����ړ��x�N�g�����쐬���A�ړ�����
        Vector3 move = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;
        transform.Translate(move);
    }
}
