using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSharp.Net;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
public class WebSocketController : Unity.Netcode.NetworkBehaviour
{

    // public static WebSocket activeWebSocket;
    public robotController robotController;

    void Start()
    {


        // DontDestroyOnLoad(this.gameObject);



        // wssv.Stop();
    }



    // void OnDestroy()
    // {
    //     ws?.Stop();
    // }
    void Awake()
    {
        // print("消えないように");
        // DontDestroyOnLoad(this.gameObject);
    }

    // void OnSceneChanged(Scene oldScene, Scene newScene)
    // {
    //     Debug.Log($"シーン遷移: {oldScene.name} → {newScene.name}");

    //     robotController = GameObject.FindObjectsOfType<robotController>().FirstOrDefault(robot => robot.IsOwner);
    // }

    // Update is called once per frame
    void Update()
    {
        if (this.IsOwner)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Server.commandQueue.Count > 0)
                {
                    Debug.Log($"commandQueue.Count: {Server.commandQueue.Count}");
                    var commandText = Server.commandQueue.Dequeue();
                    print($"CommandText: {commandText}");
                    if (robotController != null)
                        commandParse(commandText);
                    // print($"Name: {command.name}, Property: {command.property}, Value: {command.value}");
                    // // robotController.setvalue(name, property, value);
                    // robotController.setvalue(command.name, command.property, command.value);
                }

            }
        }
    }



    private void commandParse(string commandText)
    {
        CommandData command = JsonUtility.FromJson<CommandData>(commandText);

        if (robotController != null)
        {

            if (command.type == "set")
            {
                var parts = command.key.Split('.');
                if (parts.Length == 2)
                {

                    string name = parts[0];
                    string property = parts[1];
                    // Debug.Log($"First: {first}, Second: {second}");
                    float value = float.Parse(command.value); // value
                    robotController.setvalue(name, property, value);
                }
            }
            else if (command.type == "get")
            {
                string result = response(command.key);
                string respoonse = JsonUtility.ToJson(new ResponseData(result, command.id));
                Server.currentConnection.sendToClient(respoonse);
            }
        }
    }

    string response(string key)
    {
        var parts = key.Split('.');
        if (parts.Length == 2)
        {
            if (parts[0] == "key")
            {
                print(parts[1]);
                return $"{Input.GetKey(parts[1])}";
            }

        }
        return "";
    }
}


[System.Serializable]
public class CommandData
{
    public string type;
    public string key;
    public string value;
    public int id;
}



[System.Serializable]
public class ResponseData
{
    public ResponseData(string result, int id)
    {
        this.result = result;
        this.id = id;
    }
    public string result;
    public int id;
}

