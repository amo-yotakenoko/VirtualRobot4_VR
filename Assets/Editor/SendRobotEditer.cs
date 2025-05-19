using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(sendRobot))]
public class SendRobotEditer : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 通常のインスペクターを表示

        sendRobot sendRobotScript = (sendRobot)target;

        // ボタンを表示
        if (Application.isPlaying && GUILayout.Button("削除"))
        {
            sendRobotScript.destroyRobotServerRpc();
        }
        // if (Application.isPlaying && GUILayout.Button("リロード"))
        // {
        //     sendRobotScript.reloadRobotServerRpc();
        // }
    }
}
