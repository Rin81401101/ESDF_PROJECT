using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTestShot : MonoBehaviour
{
    [SerializeField]
    GameObject weaponObj;
    WeaponBase weapon;

    // Start is called before the first frame update
    void Start()
    {
        weapon = weaponObj.GetComponent<AssaultRifle>();    
    }

    // Update is called once per frame
    void Update()
    {
        if (weapon != null) {
            if (Input.GetKey(KeyCode.Space)) {
                weapon.Shot();
            }

            if (Input.GetKey(KeyCode.R)) {
                weapon.OnReload();
            }
        }
    }
}
