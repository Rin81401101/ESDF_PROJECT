using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("移動速度"), SerializeField] float moveSpeed = 3.0f;


    void Update()
    {
        // "WASD"の入力を取得する
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 入力値から移動ベクトルを作成し、移動する
        Vector3 move = new Vector3(horizontalInput, 0f, verticalInput) * moveSpeed * Time.deltaTime;
        transform.Translate(move);
    }
}
