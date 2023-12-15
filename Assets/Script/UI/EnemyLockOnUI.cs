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
    Vector2 defaultScale = Vector2.one;
    Image img;
    private void Start() {
        img = GetComponent<Image>();
        defaultScale = img.rectTransform.sizeDelta;
    }

    public void SetLockOnState(LockOnState state) {
        switch(state) {
            case LockOnState.SEARCH:
                img.rectTransform.sizeDelta = defaultScale * 0.5f;
                break;
            case LockOnState.LOCKON:
                img.rectTransform.sizeDelta = defaultScale;
                break;
        }
    }
}
