using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public class DebugGameMenuMain : EditorWindow {

    List<DebugGameMenuBase> m_debugMenuList;

    //メニューからウィンドウを表示
    [MenuItem("Debug/DebugGameMenu Window")]
    public static void Open() {
        GetWindow(typeof(DebugGameMenuMain));
    }

    private void Awake() {
        m_debugMenuList = new List<DebugGameMenuBase>() {
            new DebugGameWeapon(),
        };
    }

    private void OnGUI() {
        foreach (var menu in m_debugMenuList) {
            menu.UpdateGUI();
        }
    }

}   