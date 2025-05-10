using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
//没
public class fieldManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // // print("ooo" + name);
        // // 初期状態では何もロードしない
        // if (NetworkManager.Singleton.IsServer)
        // {

        //     NetworkManager.Singleton.SceneManager.LoadScene("field", LoadSceneMode.Additive);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        // Pキーが押されたときにシーンを追加でロードする
        if (Input.GetKeyDown(KeyCode.P))
        {
        }
    }

    // フィールドシーンを追加でロードするメソッド

}