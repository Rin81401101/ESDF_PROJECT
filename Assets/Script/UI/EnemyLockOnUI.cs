using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLockOnUI : MonoBehaviour
{
    public enum LockOnState {
        SEARCH,
        LOCKON
    }

    [SerializeField]
    private Image m_lockOnImage = null;


    private void Start() {

    }

    public void SetLockOnState(LockOnState state) {
        switch(state) {
            case LockOnState.SEARCH:
                m_lockOnImage.rectTransform.sizeDelta = Vector2.one*25;
                break;
            case LockOnState.LOCKON:
                m_lockOnImage.rectTransform.sizeDelta = Vector2.one*50;
                break;
        }
    }
}
