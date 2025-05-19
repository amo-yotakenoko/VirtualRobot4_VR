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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.IO;
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
using Unity.Netcode;
using Unity.Mathematics;


public class robotSelect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        image.enabled = false;
        string currentDirectory = Directory.GetCurrentDirectory();

        // // #if UNITY_EDITOR
        // currentDirectory = @"C:\Users\taken\Downloads\vr3build";
        // // #endif
        print(Directory.GetCurrentDirectory());
        StartCoroutine(GetAllFilesCoroutine(currentDirectory));
        IpPortInput.text = Settings.load("server", "127.0.0.1:7777");
    }

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

            // 1フレーム待機して処理を中断
            yield return null;

        }
    }


    float robotpos = 0;
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
        rp.toggle.isOn = true;
        rp.toggle.onValueChanged.Invoke(true);
        robotpreview.transform.position = new Vector3(robotpos, 0, 0);
        robotpos += 10;



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
        image.enabled = true;
        image.texture = renderTexture;
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

        propertyView.text = $"{path}\nmodel:{glbPath}\nsoft:{programPath}\nprogram:{processPath}";


    }

    public Transform scrollViewContent;



    // Update is called once per frame
    void Update()
    {
        // print(glbFullPath);
        // image.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
    }
    public TMP_InputField IpPortInput;
    public static string ipAddress; // IPアドレスを保存する static 変数
    public static ushort port; // ポート番号を保存する static 変数
    public void StartHost()
    {
        SceneManager.sceneLoaded += HostStart;
        SceneManager.LoadScene("multi");

        // NetworkManager.Singleton.StartHost();
    }
    private void HostStart(Scene next, LoadSceneMode mode)
    {
        // 接続先のIPアドレスとポートを設定
        // var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        // if (transport is Unity.Netcode.Transports.UTP.UnityTransport unityTransport)
        // {
        //     unityTransport.SetConnectionData(ipAddress, port);
        // }

        // クライアントとしてゲームを開始
        NetworkManager.Singleton.StartHost();
        LoadField();
        // NetworkManager.Singleton.StartHost();

        // イベントリスナーを削除
        SceneManager.sceneLoaded -= HostStart;
    }


    public void StartClient()
    {
        Settings.save("server", IpPortInput.text);
        print(IpPortInput.text);
        if (new Regex(@"^[A-Z0-9]{6}$").IsMatch(IpPortInput.text))
        {
            print("参加コード");
            SceneManager.sceneLoaded += JoinRelay;
            // "multi" シーンを読み込む
            SceneManager.LoadScene("multi");
        }
        else if (new Regex(@"^(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):(\d{1,5})$").IsMatch(IpPortInput.text))
        {
            print("IPアドレス");
            string[] parts = IpPortInput.text.Split(':');
            ipAddress = parts[0];
            port = ushort.Parse(parts[1]);
            // SceneManager.sceneLoaded イベントに GameSceneLoaded メソッドを登録
            SceneManager.sceneLoaded += ClientStart;
            // "multi" シーンを読み込む
            SceneManager.LoadScene("multi");
        }


    }

    private void ClientStart(Scene next, LoadSceneMode mode)
    {
        // 接続先のIPアドレスとポートを設定
        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        if (transport is Unity.Netcode.Transports.UTP.UnityTransport unityTransport)
        {
            unityTransport.SetConnectionData(ipAddress, port);
        }

        // クライアントとしてゲームを開始
        NetworkManager.Singleton.StartClient();
        // NetworkManager.Singleton.StartHost();


        // イベントリスナーを削除
        SceneManager.sceneLoaded -= ClientStart;
    }

    public async void JoinRelay(Scene next, LoadSceneMode mode)
    {
        print(IpPortInput.text);
        string joinCode = IpPortInput.text;
        // Auth認証
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Debug.Log("JoinRelay code = " + joinCode);
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
            joinAllocation.RelayServer.IpV4,
            (ushort)joinAllocation.RelayServer.Port,
            joinAllocation.AllocationIdBytes,
            joinAllocation.Key,
            joinAllocation.ConnectionData,
            joinAllocation.HostConnectionData
            );

        NetworkManager.Singleton.StartClient();

        SceneManager.sceneLoaded -= JoinRelay;
    }


    public void CreateRelay()
    {
        SceneManager.sceneLoaded += CreateRelay;

        // "multi" シーンを読み込む
        SceneManager.LoadScene("multi");
    }

    public string lobbyName;
    public int maxPlayers;
    public CreateLobbyOptions createOptions;
    public int maxConnections;
    public string hostAllocationId;
    private async void CreateRelay(Scene next, LoadSceneMode mode)
    {
        // print("AuthenticationService.Instance.IsSignedIn)" + AuthenticationService.Instance.IsSignedIn);
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        // Relay鯖にAllocation
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        // Relay鯖へのJoinCode取得
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        print("joincode" + joinCode);
        FindObjectOfType<netWorkUI>().joinCode.text = joinCode;
        // Lobby作成
        Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createOptions);

        // allocationからリレー鯖情報を取得してNetworkManagerに設定
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData);

        // Hostとしてゲーム開始
        NetworkManager.Singleton.StartHost();
        LoadField();

        SceneManager.sceneLoaded -= CreateRelay;

        // 一定時間ごとにLobbyにHeartBeat
        // await Task.Run(async () =>
        // {
        //     while (true)
        //     {
        //         await Lobbies.Instance.SendHeartbeatPingAsync(lobby.Id);
        //         await Task.Delay(TimeSpan.FromSeconds(10)); // 10秒ごとにHeartBeatを送信
        //     }
        // });
    }

    void LoadField()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("field", LoadSceneMode.Additive);

    }

}