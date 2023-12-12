using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField]
    protected Bullet m_bullet;

    public abstract void Shot();
    public abstract void OnReload();
    public abstract bool GetIsReload();
    public abstract float GetReloadRatio();
}
