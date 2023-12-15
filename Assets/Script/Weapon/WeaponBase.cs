using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public abstract void Shot();
    public abstract void Reload();
    public abstract bool GetIsReload();
    public abstract float GetReloadRatio();
}
