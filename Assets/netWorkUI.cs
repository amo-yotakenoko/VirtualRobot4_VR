using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
public class netWorkUI : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI log;
    public TextMeshProUGUI joinCode;
    void Start()
    {
        pause = true;
        // string currentDirectory = Directory.GetCurrentDirectory();

        // // #if UNITY_EDITOR
        // currentDirectory = @"C:\Users\taken\Downloads\vr3build";
        // // #endif

        // string[] files = Directory.GetFiles(currentDirectory)
        //                          .OrderBy(f => new FileInfo(f).LastWriteTime)
        //                          .ToArray();

        // foreach (string file in files)
        // {
        //     print(file);
        //     print(Path.GetExtension(file));
        //     log.text += $"{file}\n";
        //     if (Path.GetExtension(file) == ".glb")
        //     {
        //         glbPassInput.text = file;
        //     }
        //     if (Path.GetExtension(file) == ".py")
        //     {
        //         FileNameInput.text = "python";
        //         ArgumentsInput.text = file;


        //     }

        // }


    }
    public TMP_InputField glbPassInput;
    public TMP_InputField FileNameInput;//pythonのこと
    public TMP_InputField ArgumentsInput;
    // Update is called once per frame
    public static bool pause;
    public Transform networkPanel;
    void Update()
    {
        // print(Cursor.lockState);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause = !pause;
        }
        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsConnectedClient)
        {
            print("接続町");
        }
        Cursor.lockState = !pause ? CursorLockMode.Locked : CursorLockMode.None;
        networkPanel.gameObject.SetActive(pause);

    }

    public void startHost()
    {
        print("host");
        NetworkManager.Singleton.StartHost();
        // pause = false;
        // NetworkManager.Singleton.OnServerConnectedCallback += connected;
    }


    public TMP_InputField ipAddress;
    public TMP_InputField port;
    public void startClient()
    {
        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        if (transport is Unity.Netcode.Transports.UTP.UnityTransport unityTransport)
        {
            // 接続先のIPアドレスとポートを指定
            unityTransport.SetConnectionData(ipAddress.text, ushort.Parse(port.text));
        }
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.OnClientConnectedCallback += connected;
        // pause = false;
        // NetworkManager.Singleton.OnClientDisconnectedCallback += disconnected;
        // NetworkManager.Singleton.OnServerConnectedCallback += connected;
        // NetworkManager.Singleton.OnServerDisconnectedCallback += disconnected;
    }
    void connected(ulong clientId)
    {
        pause = false;
    }
    void disconnected(ulong clientId)
    {
        pause = false;
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("menu");
    }
}
