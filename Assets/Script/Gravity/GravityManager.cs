using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("重力加速度"), SerializeField]
    private float gravity;

    public static GravityManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void gravityUpdate(Rigidbody rigidbody)
    {
        rigidbody.velocity += Vector3.down * gravity * Time.deltaTime;
    }
}
