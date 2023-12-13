using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTestShot : MonoBehaviour
{

    WeaponBase m_weapon;

    // Start is called before the first frame update
    void Start()
    {
        m_weapon = WeaponManager.m_instance.AttachWeapon(this.gameObject, "AssaultRifle");
    }

    // Update is called once per frame
    void Update()
    {
        if (m_weapon != null) {
            if (Input.GetKey(KeyCode.Space)) {
                m_weapon.Shot();
            }

            if (Input.GetKey(KeyCode.R)) {
                m_weapon.Reload();
            }
        }
    }

    public WeaponBase GetWeapon() {
        return m_weapon;
    }
}
