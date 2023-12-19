using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugGameWeapon : DebugGameMenuBase {
    int[] m_selectIndex = new int[2] { 0, 0 };
    bool m_isOpen = true;
    bool m_isError = false;

    PlayerManager m_player = null;



    public override void UpdateGUI() {
        //プレイヤーを探して取得する
        if (m_player == null) {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null) {
                m_player = playerObj.GetComponent<PlayerManager>();
            }
        }

        m_isOpen = EditorGUILayout.BeginFoldoutHeaderGroup(m_isOpen, "WeaponDebug");
        if (m_isOpen) {
            if (!ErrorLog()) {
                Dictionary<string, List<GameObject>> weaponList = WeaponManager.m_instance.GetWeaponList();
                List<string> categoryNameList = new List<string>();

                foreach (var key in weaponList.Keys) {
                    foreach (var weapon in weaponList[key]) {
                        categoryNameList.Add(key + "/" + weapon.name);
                    }
                }

                EditorGUILayout.LabelField("プレイヤーの装備を変える");

                //Popupで武器一覧を表示する
                for (int i = 0; i < m_selectIndex.Length; i++) {
                    using (new EditorGUILayout.HorizontalScope()) {
                        m_selectIndex[i] = UnityEditor.EditorGUILayout.Popup(
                                            label: new UnityEngine.GUIContent("装備枠[" + i + "]"),
                                            selectedIndex: m_selectIndex[i],
                                            displayedOptions: categoryNameList.ToArray()
                                            );

                        if (GUILayout.Button("Change!")) {
                            string[] name = categoryNameList[m_selectIndex[i]].Split('/');
                            ChangeWeapon(name[1], i);
                        }
                    }
                }
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private bool ErrorLog() {
        if (WeaponManager.m_instance == null) {
            EditorGUILayout.HelpBox("WeaponManager.m_instanceが見つかりません", MessageType.Error);
            m_isError = true;
        }
        if (m_player == null) {
            EditorGUILayout.HelpBox("Playerが見つかりません", MessageType.Error);
            m_isError = true;
        }

        return m_isError;
    }

    void ChangeWeapon(string changeWeaponName, int index) {
        m_player.GetWeapon(index).FinalizeDestroy();
        m_player.SetWeapon(changeWeaponName, index);

        //使っている武器じゃなかったら非表示
        if(m_player.GetWeaponIndex()!=index) {
            m_player.GetWeapon(index).SetVaild(false);
        }
    }
}
