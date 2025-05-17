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