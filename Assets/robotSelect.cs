using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using GLTFast;
using UnityEngine.Networking;
using System;

using Unity.Mathematics;


public class robotSelect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (image != null) image.enabled = false;
#if UNITY_EDITOR || !UNITY_WEBGL
        string currentDirectory = Directory.GetCurrentDirectory();

        // // #if UNITY_EDITOR
        // currentDirectory = @"C:\Users\taken\Downloads\vr3build";
        // // #endif
        print(Directory.GetCurrentDirectory());
        StartCoroutine(GetAllFilesCoroutine(currentDirectory));
#else
        Application.ExternalCall("requestRobotList");
#endif
    }


#if UNITY_WEBGL
    [Serializable]
    public class RobotData
    {
        public string path;
        public string value;
    }

    public void requestRobotList(string json)
    {
        print(json);
        RobotData data = JsonUtility.FromJson<RobotData>(json);
        print(data.path + "が存在する");
        WebGLRobotUpdate.AddUpdateRobot(data.path, data.value);
        createRobotPreview(data.path);
    }
#endif



    IEnumerator GetAllFilesCoroutine(string rootDirectory)
    {
        Queue<string> directoriesToProcess = new Queue<string>();
        directoriesToProcess.Enqueue(rootDirectory);

        while (directoriesToProcess.Count > 0)
        {
            string currentDirectory = directoriesToProcess.Dequeue();

            var files = Directory.GetFiles(currentDirectory)
                                 .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                                 .ToList();

            foreach (string file in files)
            {
                if (Path.GetExtension(file) == ".glb")
                {
                    Debug.Log("Found: " + file);
                    createRobotPreview(currentDirectory);
                }
            }

            var subdirectories = Directory.GetDirectories(currentDirectory);
            foreach (var subdir in subdirectories)
            {

                if ((File.GetAttributes(subdir) & FileAttributes.Hidden) != 0 ||
                Path.GetFileName(subdir).StartsWith("."))

                    continue;

                directoriesToProcess.Enqueue(subdir);
                // yield return new WaitForSeconds(0.1f);
            }


            yield return null;

            // yield return new WaitForSeconds(0.1f);

        }
    }


    float robotpos = 100;
    public GameObject robotPreviewPrefab;
    void createRobotPreview(string path)
    {
        GameObject robotpreview = Instantiate(robotPreviewPrefab);
        // robotpreview.GetComponent<robotPreview>().scrollViewContent = scrollViewContent;
        StartCoroutine(robotpreview.GetComponent<robotPreview>().generate(path));
        robotPreview rp = robotpreview.GetComponent<robotPreview>();
        rp.robotContentUI.transform.SetParent(scrollViewContent);
        rp.robotContentUI.transform.SetAsLastSibling();
        rp.robotContentUI.transform.localScale = new Vector3(1, 1, 1);
        rp.robotSelect = this;
        rp.toggle.group = GetComponent<ToggleGroup>();
        // rp.toggle.isOn = true;
        rp.toggle.onValueChanged.Invoke(true);
        robotpreview.transform.position = new Vector3(robotpos, -100, 0);
        robotpos += 10;
        SceneManager.MoveGameObjectToScene(robotpreview, gameObject.scene);



    }




    public TMP_Text propertyView;
    public string glbPath;
    public string processPath;
    public string programPath;
    public static string glbFullPath;
    public RawImage image;



    public void selectRobot(string path, RenderTexture renderTexture)
    {
        print(path);
        if (image != null)
        {

            image.enabled = true;
            image.texture = renderTexture;
        }
#if UNITY_EDITOR || !UNITY_WEBGL
        var files = Directory.GetFiles(path)
                             .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                             // .Select(f => Path.GetFileName(f))
                             .ToList();
        glbPath = "";
        processPath = "";
        programPath = "";
        glbFullPath = "";
        processController.processname = "";
        processController.argument = "";
        foreach (string file in files)
        {
            if (Path.GetExtension(file) == ".glb")
            {
                glbFullPath = file;
                print("選択中のを" + glbFullPath + "に変更");
                glbPath = Path.GetFileName(file);
            }
            if (Path.GetExtension(file) == ".py" && Path.GetFileName(file)[0] != '_')
            {
                processPath = "python";
                processController.processname = "python";
                processController.argument = file;
                programPath = Path.GetFileName(file);
            }
        }

        if (propertyView != null) propertyView.text = $"{path}\nmodel:{glbPath}\nsoft:{programPath}\nprogram:{processPath}";
#else
 glbFullPath = path;
#endif

    }

    public Transform scrollViewContent;



    // Update is called once per frame
    void Update()
    {

        // print(glbFullPath);
        // image.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
    }


}