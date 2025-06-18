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


public class NetCodeConnection : MonoBehaviour
{
    public TMP_InputField IpPortInput;
    public static string ipAddress; // IPアドレスを保存する static 変数
    public static ushort port; // ポート番号を保存する static 変数
    void Start()
    {
        IpPortInput.text = Settings.load("server", "127.0.0.1:7777");
    }

    // Update is called once per frame
    void Update()
    {

    }

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
