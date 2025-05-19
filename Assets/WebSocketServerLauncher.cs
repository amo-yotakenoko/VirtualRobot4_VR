using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using System.Collections;


public class WebSocketServerLauncher : MonoBehaviour
{
    public static WebSocketServer ws = null;
    public TMP_Text webSocketView;
    void Start()
    {
        // print("サーバ?");
        if (ws == null) serverStart();
    }
    int port = 12345;

    void serverStart()
    {
        // string certificatePath = Application.streamingAssetsPath + "/mycert.pfx";
        // string certificatePassword = "virtual"; // パスワードを指定

        // WebSocketServerをポート番号で初期化
        port = int.Parse(Settings.load("WebSocketport", "12345"));
        ws = new WebSocketServer($"ws://localhost:{port}");  // true = secure (wss)

        // // サーバー証明書を設定
        // ws.SslConfiguration.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificatePath, certificatePassword);

        // WebSocketのサービスを追加
        ws.AddWebSocketService<Server>("/");

        // サーバーを開始
        ws.Start();
        print("サーバー開始");
    }
    // Update is called once per frame
    void Update()
    {
        if (webSocketView)
        {
            // for (int i = 0; i < 10; i++)
            // {
            while (Server.commandQueue.Count > 0)
            {
                var commandText = Server.commandQueue.Dequeue();
                webSocketViewAppend(commandText);

                // print($"Name: {command.name}, Property: {command.property}, Value: {command.value}");
                // // robotController.setvalue(name, property, value);
                // robotController.setvalue(command.name, command.property, command.value);
            }

            // }
        }
    }
    void webSocketViewAppend(string text)
    {
        webSocketView.text += $"{text}\n";
        var lines = webSocketView.text.Split('\n');
        if (lines.Length > 10)
        {
            webSocketView.text = string.Join("\n", lines.Skip(lines.Length - 10));
        }
    }
}

public class Server : WebSocketBehavior
{
    public static Queue<string> commandQueue = new Queue<string>();
    public static Server currentConnection;
    protected override void OnMessage(WebSocketSharp.MessageEventArgs e)
    {
        commandQueue.Enqueue(e.Data);

        // if (commandQueue.Count > 10)
        // {
        // Send("{\"queueCount\": " + commandQueue.Count + "}");
        // }
    }

    protected override void OnOpen()
    {
        // isActive = true;
        currentConnection = this;
    }

    protected override void OnClose(CloseEventArgs e)
    {
        // isActive = false;
        currentConnection = null;
    }

    public void sendToClient(string message)
    {
        Send(message);
    }

}