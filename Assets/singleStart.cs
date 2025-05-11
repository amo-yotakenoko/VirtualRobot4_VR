using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using TMPro;
public class singleStart : MonoBehaviour
{
    // Start is called before the first frame update
    public string filePath;
    public TextMeshProUGUI textMesh;
    public generateRobot generateRobot;
    void Start()
    {
        print("start");
        byte[] fileBytes = File.ReadAllBytes(filePath);
        print(fileBytes.Length);
        textMesh.text = fileBytes.Length.ToString();
        // StartCoroutine(LoadGltfBinaryFromMemory(fileBytes));
        StartCoroutine(generateRobot.generate(fileBytes));
        // StartCoroutine(GetComponent<generateRobot>().generate(File.ReadAllBytes(filePath)));
    }

    // Update is called once per frame
    void Update()
    {

    }
    public GLTFast.GltfAsset gltf;
    private IEnumerator LoadGltfBinaryFromMemory(byte[] data)
    {
        var gltf = new GltfImport();

        Task<bool> loadTask = gltf.LoadGltfBinary(data);

        // 非同期タスクの完了を待ちます
        yield return new WaitUntil(() => loadTask.IsCompleted);

        // タスクの結果を取得し、成功した場合はインスタンス化を試みます
        bool success = loadTask.Result;
        if (success)
        {
            Task<bool> instantiateTask = gltf.InstantiateMainSceneAsync(transform);

            // インスタンス化の非同期タスクの完了を待ちます
            yield return new WaitUntil(() => instantiateTask.IsCompleted);

            // インスタンス化が成功したかどうかを確認します
            bool instantiateSuccess = instantiateTask.Result;
            if (!instantiateSuccess)
            {
                print("Failed to instantiate main scene.");
            }
        }
        else
        {
            print("Failed to load glTF binary.");
        }
    }
}
