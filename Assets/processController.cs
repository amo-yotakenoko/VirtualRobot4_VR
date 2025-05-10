using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
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
public class processController : Unity.Netcode.NetworkBehaviour
{
    // Start is called before the first frame update
    public Process proc;
    void Start()
    {
        if (this.IsOwner)
        {
            // C:\Users\taken\projects\VirtualRobot4\Assets\robots\test.py
            // C:\Users\taken\projects\VirtualRobot4\Assets\robots\test.py
            // C:\Users\taken\projects\VirtualRobot4\Assets\robots\test.py
            // processname = GameObject.Find("FileNameInput").GetComponent<TMP_InputField>().text;
            // argument = GameObject.Find("ArgumentsInput").GetComponent<TMP_InputField>().text;
            if (processname == "" || argument == "")
            {
                print("プロセスできない");

            }
            commandQueue = new Queue<(string name, string property, float value)>();
            startprocess(processname, argument);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (robotController == null)
        // {
        //     robotController[] controllers = FindObjectsOfType<robotController>();
        //     foreach (robotController controller in controllers)
        //     {
        //         if (controller.IsOwner)
        //             robotController = controller;
        //     }
        // }
        // else
        // {
        if (this.IsOwner)
        {
            for (int i = 0; i < 10; i++)
            {
                while (commandQueue.Count > 0)
                {
                    var command = commandQueue.Dequeue();
                    print($"Name: {command.name}, Property: {command.property}, Value: {command.value}");
                    // robotController.setvalue(name, property, value);
                    robotController.setvalue(command.name, command.property, command.value);
                }

            }
        }
        // }


    }


    public static string processname;
    public static string argument;
    public void startprocess(string filename, string argument = "", string inputcommand = "")
    {
        // try
        // {
        print("filename:" + filename);
        print("inputcommand\n" + inputcommand);

        proc = new Process();
        proc.StartInfo.FileName = filename; // 起動させる別アプリ名をここに入れて下さい(フルパス指定でも可)
        proc.StartInfo.Arguments = argument; //引数
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardInput = true;
        proc.StartInfo.RedirectStandardError = true;
        proc.StartInfo.RedirectStandardOutput = true;

        // proc.StartInfo.WorkingDirectory = Application.persistentDataPath;
        proc.StartInfo.CreateNoWindow = true;

        // Application.OpenURL(Application.persistentDataPath);
        proc.Start();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();
        proc.OutputDataReceived += OnStandardOut;
        proc.ErrorDataReceived += OnErrorOut;

        // send("pwd");
        // send("ls");
        // send("cd virtualRobotPrograms");
        // send("gcc test.c");
        // if (inputcommand != "")
        //     StartCoroutine(sendcommands(inputcommand));
        // }
        // catch (Exception e)
        // {
        //     print(e);

        //     // execution.log(e.ToString(), 1);
        // }
    }
    public IEnumerator sendcommands(string inputcommand)
    {
        yield return new WaitForSeconds(1f);
        foreach (string command in inputcommand.Split("\n"))
        {
            print("command:{" + command + "}");
            send(command);

            // yield return new WaitForSeconds(0.1f);
            // yield return null;
        }
        yield return null;
    }
    Queue<string> inputed = new Queue<string>();

    // Queue<string> errorinputed = new Queue<string>();
    void OnErrorOut(object sender, DataReceivedEventArgs e)
    {
        print($"<color=red>{e.Data}</color>");
        // inputed.Enqueue(e.Data);
    }
    public robotController robotController;
    public Queue<(string name, string property, float value)> commandQueue;
    void OnStandardOut(object sender, DataReceivedEventArgs e)
    {
        print($"<color=yellow>{e.Data}</color>");
        Match setmatch = Regex.Match(e.Data, @"set:\s*(\w+)\s*\.\s*(\w+)\s*=\s*(-?\d+)");
        if (setmatch.Success)
        {
            // print(setmatch.Success + "," + setmatch);
            string identifier = setmatch.Groups[1].Value;  // identifier
            string command = setmatch.Groups[2].Value;     // command
            float value = float.Parse(setmatch.Groups[3].Value); // value

            // print("Identifier: " + identifier);
            // print("Command: " + command);
            // print("Value: " + value);
            // robotController.setvalue(identifier, command, value);
            commandQueue.Enqueue((identifier, command, value));
            print(commandQueue.Count);
            // robotController.test();
        }

        // inputed.Enqueue(e.Data);
        // execution.log(e.ToString(), 1);
        // if (int.TryParse(e.Data, out int number))
        // {
        //     send((number + 1).ToString());
        // }
        // if (e.Data == "値を入力\n") proc.StandardInput.WriteLine("100\n");
    }

    void send(string command)
    {
        print($"<color=cyan>{command}</color>");
        proc.StandardInput.Write(command.Replace("\r", "") + "\n");
    }
    void OnDestroy()
    {
        print("processclose");
        proc.Kill();
    }

}
