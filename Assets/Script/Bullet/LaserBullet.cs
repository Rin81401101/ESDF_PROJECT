using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LaserBullet : MonoBehaviour
{
    [SerializeField]
    private LineRenderer m_lineRenderer;
    [SerializeField]
    float duration = 0.3f;


    // Start is called before the first frame update
    void Start()
    {


        float width = m_lineRenderer.startWidth;
        DOVirtual.Float(
            from: 0.25f,
            to:0,
            duration:0.3f,
            onVirtualUpdate: (m_width) => {
                m_lineRenderer.startWidth = m_width;
                m_lineRenderer.endWidth = m_width;
            }
            );
        Destroy(this.gameObject, 0.3f);
    }




    public void SetShotRange(Vector3 startPos, Vector3 endPos) {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);

    }
}
