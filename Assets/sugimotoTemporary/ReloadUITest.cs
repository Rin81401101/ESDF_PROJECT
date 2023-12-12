using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadUITest : MonoBehaviour
{
    [SerializeField]
    GameObject m_imageParent;

    [SerializeField]
    Image m_image;

    [SerializeField]
    WeaponBase m_weapon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_image.fillAmount=m_weapon.GetReloadRatio();

        if (m_weapon.GetIsReload()) {
            m_imageParent.SetActive(true);
        } else {
            m_imageParent.SetActive(false);
        }
    }
}
