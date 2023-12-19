using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField]
    protected GameObject m_visualObject = null;
    protected bool m_isVaild = true;

    public abstract void Shot();
    public abstract void Reload();
    public abstract void ViewScope(bool isView);
    public abstract void FinalizeDestroy();
    public abstract bool GetIsReload();
    public abstract float GetReloadRatio();

    //この武器を有効化する
    public abstract void SetVaild(bool isVaild);
}
