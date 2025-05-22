using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRManager : MonoBehaviour
{
    public GameObject XROrigin;
    public bool simulatorMode;
    public GameObject XRdeviceSimulator;

    public GameObject VRBasePrefab;
    public GameObject VRHeadPrefab;
    public GameObject VRRightHandPrefab;
    public GameObject VRLeftHandPrefab;

    // 生成されたインスタンスを記録
    public GameObject vrBaseInstance;
    public GameObject vrHeadInstance;
    public GameObject vrRightHandInstance;
    public GameObject vrLeftHandInstance;

    void InstantiateVRPrefab()
    {
        // すでに生成済みなら何もしない
        if (vrBaseInstance != null || vrHeadInstance != null ||
            vrRightHandInstance != null || vrLeftHandInstance != null)
        {
            Debug.Log("VRプレハブはすでに生成されています。");
            return;
        }

        // プレハブのインスタンス化
        vrBaseInstance = Instantiate(VRBasePrefab);
        vrHeadInstance = Instantiate(VRHeadPrefab);
        vrRightHandInstance = Instantiate(VRRightHandPrefab);
        vrLeftHandInstance = Instantiate(VRLeftHandPrefab);

        // XR Origin 情報取得
        VRCharacterController vrCharacterController = XROrigin.GetComponent<VRCharacterController>();

        // 各オブジェクトの親を設定
        vrBaseInstance.transform.SetParent(vrCharacterController.xrOrigin.transform, false);
        vrHeadInstance.transform.SetParent(vrCharacterController.HeadTransform, false);  // 頭も XR Origin に直接付ける
        vrRightHandInstance.transform.SetParent(vrCharacterController.rightHandTransform, false);
        vrLeftHandInstance.transform.SetParent(vrCharacterController.leftHandTransform, false);
    }


    void Start()
    {
        // 10秒ごとにチェックするコルーチン開始
        StartCoroutine(CheckHandTracking());
        if (simulatorMode)
        {
            Instantiate(XRdeviceSimulator);
        }
    }

    void Update()
    {
        GetControllerInput(" ", " ");

        if (vrBaseInstance && vrHeadInstance && vrRightHandInstance && vrLeftHandInstance)
        {
            VRCharacterController vrCharacterController = GetComponent<VRCharacterController>();

            vrBaseInstance.transform.position = vrCharacterController.xrOrigin.transform.position;
            vrBaseInstance.transform.rotation = vrCharacterController.xrOrigin.transform.rotation;

            vrHeadInstance.transform.position = vrCharacterController.HeadTransform.position;
            vrHeadInstance.transform.rotation = vrCharacterController.HeadTransform.rotation;

            vrRightHandInstance.transform.position = vrCharacterController.rightHandTransform.position;
            vrRightHandInstance.transform.rotation = vrCharacterController.rightHandTransform.rotation;

            vrLeftHandInstance.transform.position = vrCharacterController.leftHandTransform.position;
            vrLeftHandInstance.transform.rotation = vrCharacterController.leftHandTransform.rotation;
        }
    }

    static bool isVRMode = false;

    IEnumerator CheckHandTracking()
    {

        while (true)
        {
            isVRMode = IsHandTrackingAvailable();

            if (XROrigin != null)
            {
                activeCameraSet(isVRMode);
            }

            yield return new WaitForSeconds(3f);
        }
    }


    void activeCameraSet(bool isVR)
    {
        XROrigin.SetActive(isVR);
        cameraUI ui = FindObjectOfType<cameraUI>();
        if (ui != null)
        {
            ui.enabled = !isVR;
            foreach (var camera in ui.allCameras)
            {
                camera.enabled = !isVR;
            }
        }
        if (isVR) InstantiateVRPrefab();
    }




    public bool IsHandTrackingAvailable()
    {
        if (simulatorMode) return true;
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);

        foreach (var device in devices)
        {
            // if (device.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
            // {
            return true;
            // }
        }

        return false;
    }

    public static String GetControllerInput(String hand, String key)
    {

        String result = "";
        InputDevice handDevice = handDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (hand == "VRright")
        {
            handDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }
        else if (hand == "VRleft")
        {
            handDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }
        else
        {
            return "";
        }

        List<InputFeatureUsage> features = new List<InputFeatureUsage>();
        if (handDevice.TryGetFeatureUsages(features))
        {
            foreach (var feature in features)
            {
                if (feature.name == key)
                {
                    if (feature.type == typeof(bool))
                    {
                        if (handDevice.TryGetFeatureValue(feature.As<bool>(), out bool boolValue))
                        {
                            Debug.Log($" {feature.name}: {boolValue}");
                            result = $"{boolValue}";
                        }
                    }
                    else if (feature.type == typeof(float))
                    {
                        if (handDevice.TryGetFeatureValue(feature.As<float>(), out float floatValue))
                        {
                            Debug.Log($" {feature.name}: {floatValue}");
                            result = $"{floatValue}";
                        }
                    }
                }


            }
        }
        return result;
    }
}



/*

 Grip: 0
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:101)
VRManager:Update () (at Assets/VRManager.cs:19)

 GripButton: False
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 MenuButton: False
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 PrimaryButton: False
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 PrimaryTouch: False
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 SecondaryButton: False
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 SecondaryTouch: False
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 Trigger: 0
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:101)
VRManager:Update () (at Assets/VRManager.cs:19)

 TriggerButton: False
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 TriggerTouch: False
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 Primary2DAxisClick: False
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 Primary2DAxisTouch: False
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 ThumbrestTouch: False
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 DeviceIsTracked: True
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 PointerIsTracked: True
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

 IsTracked: True
UnityEngine.Debug:Log (object)
VRManager:GetControllerInput (string,string) (at Assets/VRManager.cs:93)
VRManager:Update () (at Assets/VRManager.cs:19)

*/