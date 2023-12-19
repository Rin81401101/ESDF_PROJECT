using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour {

    public static WeaponManager m_instance = null;

    private Dictionary<string, List<GameObject>> m_weaponList = new Dictionary<string, List<GameObject>>();

    [SerializeField]
    private GameObject m_weaponUIPrefab;

    [HideInInspector]
    public GameObject m_weaponUI;

    private void Awake() {
        m_instance = this;

        m_weaponUI=Instantiate(m_weaponUIPrefab);

        //Weapon以下の全ファイルパス取得
        string path = Application.dataPath + "/Prefab/Weapon";
        string[] directoryNames = System.IO.Directory.GetFiles(@path, "*", System.IO.SearchOption.AllDirectories);
        List<GameObject> tempWeaponList = new List<GameObject>();
        string folderNameTemp = "";
        bool isFirst = true;
        for (int i = 0; i < directoryNames.Length; i++) {
            //拡張子取得
            string extension = System.IO.Path.GetExtension(directoryNames[i]);

            //prefabだったらオブジェクトとして取得したい
            if (extension == ".prefab") {
                //フォルダ名取得とAssetより上のパスを消す処理のために一旦分割
                string[] pathArray = directoryNames[i].Split('\\');
                directoryNames[i] = "";
                for (int j = 0; j < pathArray.Length; j++) {
                    directoryNames[i] += pathArray[j];
                    if (j != pathArray.Length - 1) {
                        directoryNames[i] += "/";
                    }
                }

                //AssetsのPathに変換する
                pathArray = directoryNames[i].Split('/');
                string assetPath = "";
                bool isAssetFolder = false;
                for (int j = 0; j < pathArray.Length; j++) {
                    //Assetsフォルダみっけ
                    if (pathArray[j] == "Assets") {
                        isAssetFolder = true;
                    }

                    if (isAssetFolder) {
                        assetPath += pathArray[j];
                        if (j != pathArray.Length - 1) {
                            assetPath += "/";
                        }
                    }
                }
                
                //フォルダ名をキーとしてDictionaryにGameObjectを追加
                string folderName = pathArray[pathArray.Length - 2];
                GameObject weaponAssets = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) as GameObject;
                if (folderName == folderNameTemp || isFirst) {
                    if (isFirst) {
                        folderNameTemp = folderName;
                    }
                    tempWeaponList.Add(weaponAssets);
                    isFirst = false;
                } else {

                    m_weaponList.Add(folderNameTemp, new List<GameObject>(tempWeaponList));
                    tempWeaponList = new List<GameObject>();
                    tempWeaponList.Add(weaponAssets);
                    folderNameTemp = folderName;
                }

            }
        }
        m_weaponList.Add(folderNameTemp, new List<GameObject>(tempWeaponList));

        //ちゃんとweaponListに入っているかDebug.Logで出す
        //List<string> keys=new List<string>();
        //foreach(string key in weaponList.Keys) {
        //    keys.Add(key);
        //}
        //int k = 0;
        //foreach (List<GameObject> weapon in weaponList.Values) {
        //    foreach (GameObject weaponObj in weapon) {
        //        Debug.Log(keys[k]+":"+weaponObj.name);
        //    }
        //    k++;
        //}
    }


    public WeaponBase AttachWeapon(GameObject parent, string weaponName) {
        foreach (List<GameObject> weapon in m_weaponList.Values) {
            foreach (GameObject weaponObj in weapon) {
                if (weaponObj.name == weaponName) {
                    GameObject obj = Instantiate(weaponObj, parent.transform);
                    obj.transform.parent = parent.transform;

                    return obj.GetComponent<WeaponBase>();
                }
            }
        }

        return null;
    }

    public Dictionary<string, List<GameObject>> GetWeaponList() {
        return m_weaponList;
    }

}
