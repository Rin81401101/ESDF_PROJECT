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

    [SerializeField]
    private GameObject m_lineParent = null;


    private void Start() {

    }

    public void SetLockOnState(LockOnState state) {
        switch(state) {
            case LockOnState.SEARCH:
                m_lineParent.SetActive(true);
                m_lockOnImage.rectTransform.sizeDelta = Vector2.one*50;
                break;
            case LockOnState.LOCKON:
                m_lineParent.SetActive(false);
                m_lockOnImage.rectTransform.sizeDelta = Vector2.one*100;
                break;
        }
    }
}
