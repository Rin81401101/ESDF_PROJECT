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
    WeaponTestShot m_player;
    


    // Update is called once per frame
    void Update()
    {
        m_image.fillAmount= m_player.GetWeapon().GetReloadRatio();

        if (m_player.GetWeapon().GetIsReload()) {
            m_imageParent.SetActive(true);
        } else {
            m_imageParent.SetActive(false);
        }
    }
}
