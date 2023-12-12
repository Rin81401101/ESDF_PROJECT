using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponList : MonoBehaviour
{
    public enum WeaponType {
        ASSAULT_RIFLE,
        WEAPON_MAX
    }

    [SerializeField]
    public WeaponArray[] weapons=new WeaponArray[(int)WeaponType.WEAPON_MAX];
    public static WeaponList m_instance = null;


    [System.Serializable]
    public class WeaponArray {
        [SerializeField]
        public WeaponBase[] weapon;
    }

    private void Awake() {



        if(m_instance == null)
            m_instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR

#endif
    }
}
