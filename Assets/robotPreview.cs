using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class robotPreview : MonoBehaviour
{
    // Start is called before the first frame update
    // public GLTFast.GltfAsset gltf;
    public Transform gltfLoaded;
    public Camera c;
    public string directoryPath;
    public string glbpath;
    RenderTexture renderTexture;
    public IEnumerator generate(string path)
    {
        cameraSet();

        directoryPath = path;
        glbpath = Directory.GetFiles(path)
                                   .Where(file => Path.GetExtension(file) == ".glb")
                                   .OrderByDescending(file => new FileInfo(file).LastWriteTime)
                                   .FirstOrDefault();
        if (glbpath == null)
        {
            Debug.LogError("No .glb file found in directory: " + path);
            yield break;
        }
        this.gameObject.name = glbpath;


        byte[] data = File.ReadAllBytes(glbpath);
        var gltf = new GltfImport();
        Task<bool> loadTask = gltf.LoadGltfBinary(data, new Uri(path));
        yield return new WaitUntil(() => loadTask.IsCompleted);
        bool success = loadTask.Result;

        if (success)
        {
            Task<bool> instantiateTask = gltf.InstantiateMainSceneAsync(gltfLoaded);
            yield return new WaitUntil(() => instantiateTask.IsCompleted);
            success = instantiateTask.Result;
        }

        if (!success)
        {
            Debug.LogError("Failed to load or instantiate glTF file.");
        }
        // yield return new WaitForSeconds(1);
        print("読み込みok");
        calculatSize(gltfLoaded);
        gltfLoaded.localScale = new Vector3(1, 1, 1) * 5 / size;



        objsProcess(this.transform);


    }

    void cameraSet()
    {
        // UI要素にアタッチするRawImageを作成し、レンダーテクスチャを設定
        // rawImage = Instantiate(rawImagePrefab, transform);
        // rawImage.texture = renderTexture;

        // RenderTexture renderTexture = new RenderTexture(256, 256, 0);
        // カメラにレンダーテクスチャを指定
        renderTexture = new RenderTexture(1024, 1024, 24);
        c.targetTexture = renderTexture;

        // レンダーテクスチャをRawImageに設定する
        image.texture = renderTexture;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        image.gameObject.name = this.gameObject.name;
        gltfLoaded.transform.Rotate(-25, 0, 0);
        // GameObject robotContentUI = Instantiate(robotContentUIPrefab, scrollViewContent);
        // robotContentUI.transform.SetParent(robotContentUI.transform);
    }

    void objsProcess(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // print(child.gameObject.name);
            Light LightComponent = child.GetComponent<Light>();
            if (LightComponent != null)
            {
                print("light");
                // LightComponent.enabled = true;
                LightComponent.intensity /= 250f;
            }

            objsProcess(child);

        }
    }
    public void Update()
    {
        gltfLoaded.transform.Rotate(0, Time.deltaTime * 5, 0);
        if (robotSelect == null) Destroy(this.gameObject);
        // print(toggle.transform.parent.GetComponent<RectTransform>().sizeDelta);
        // toggle.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1) * toggle.transform.parent.GetComponent<RectTransform>().sizeDelta.y;
    }

    public RawImage image;
    // public Transform scrollViewContent;
    public GameObject robotContentUI;
    public float size;
    void calculatSize(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // 子オブジェクトの MeshFilter コンポーネントを取得
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();

            if (meshFilter != null && meshFilter.mesh != null)
            {
                // メッシュの頂点を取得し、一番原点から遠い点を見つける
                Vector3[] vertices = meshFilter.mesh.vertices;
                foreach (Vector3 vertex in vertices)
                {
                    float distance = Vector3.Distance(child.TransformPoint(vertex), gltfLoaded.position);
                    Debug.DrawLine(child.TransformPoint(vertex), gltfLoaded.position, Color.red, 1.0f);
                    if (distance > size)
                    {
                        // farthestPoint = vertex;
                        size = distance;
                    }
                }
            }

            // 子オブジェクトの子を再帰的に検索
            calculatSize(child);
        }
    }
    public robotSelect robotSelect;
    public Toggle toggle;

    public GameObject robotChangeButton;
    public void toggleChanged()
    {
        if (toggle.isOn)
        {
            robotSelect.selectRobot(directoryPath, renderTexture);
        }
        robotChangeButton.SetActive(toggle.isOn && SceneManager.GetActiveScene().name == "multi");
    }


    public void robotChange()
    {
        robotSelect.glbFullPath = glbpath;
        // print("交代");
        player.ownerPlayer.robotChange();
    }


}
