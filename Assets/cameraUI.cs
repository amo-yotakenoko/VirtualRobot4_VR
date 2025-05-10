using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class cameraUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // registerCameras();
        // ActiceCameraUpdate();
        // print(Display.displays.Length);
        // for (int i = 1; i < Display.displays.Length; i++)
        // {
        //     Display.displays[i].Activate();
        // }


        //本体の解像度設定（タイトルバーを隠さない）
        // WindowController.windowReplace("MultiWindow", 100, 100, 640, 480, false);

        //Secondary Displayの解像度設定（タイトルバー隠す）
        // WindowController.windowReplace("Unity Secondary Display", 1000, 100, 640, 640, true);
    }

    // Update is called once per frame
    public int acticeCameraId;
    public Vector2 miniViewSize;

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.C))


        registerCameras();


        ActiceCameraSet();
        ActiceCameraUpdate();





    }
    public void registerCameras()
    {

        allCameras = new List<Camera>(FindObjectsOfType<Camera>());

        // enable なカメラだけをフィルタリングする
        allCameras = allCameras.Where(c => c.enabled).ToList();

    }
    public List<Camera> allCameras;

    void ActiceCameraSet()
    {
        for (int i = 1; i <= allCameras.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                // 押された数字を変数に代入
                acticeCameraId = i - 1;

            }
        }
    }
    public static Camera activeCamera;
    void ActiceCameraUpdate()
    {
        int i = 0;
        int menupos = 0;
        miniViewSize.x = 1.0f / (allCameras.Count);
        foreach (var camera in allCameras)
        {
            if (acticeCameraId == i)
            {
                camera.rect = new Rect(0f, 0f, 1, 1);
                camera.depth = 0;
                activeCamera = camera;
            }
            else
            {
                // print(i);
                camera.rect = new Rect(menupos * miniViewSize.x, 1 - miniViewSize.y, miniViewSize.x, miniViewSize.y);
                camera.depth = 5;
            }
            menupos += 1;
            i += 1;
            // if (camera.transform.parent != null)
            // {

            //     var fpsComponent = camera.transform.parent.GetComponent<fps>();
            //     if (fpsComponent != null)
            //         fpsComponent.enabled = acticeCamera == i;

            //     var orbitCameraInputComponent = camera.transform.parent.GetComponent<orbitCameraInput>();
            //     if (orbitCameraInputComponent != null)
            //         orbitCameraInputComponent.enabled = acticeCamera == i;
            // }

        }



    }
}



