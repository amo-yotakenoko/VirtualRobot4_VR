using System.IO;
using UnityEngine;
using VRM;

public class LoadVRM : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
    //     Load(Settings.load("VRMpath", "256human.vrm"));
    // }

    // async void Load(string path)
    // {
    //     if (!File.Exists(path))
    //     {
    //         Debug.LogError("VRM file not found: " + path);
    //         return;
    //     }

    //     // バイト配列として読み込み
    //     byte[] vrmBytes = File.ReadAllBytes(path);

    //     // VRMのGLB形式のパース
    //     var context = new VRMImporterContext();
    //     context.ParseGlb(vrmBytes);

    //     // 読み込み（非同期）
    //     await context.LoadAsyncTask();

    //     // GameObjectの取得
    //     GameObject vrmModel = context.Root;

    //     // シーンに配置
    //     vrmModel.transform.SetParent(transform, false);
    // }
}
