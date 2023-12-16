using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("èdóÕâ¡ë¨ìx"), SerializeField]
    private float m_gravity;

    public static GravityManager m_instance;

    private void Awake()
    {
        m_instance = this;
    }

    public void gravityUpdate(Rigidbody rigidbody)
    {
        rigidbody.velocity += Vector3.down * m_gravity * Time.deltaTime;
    }
}
