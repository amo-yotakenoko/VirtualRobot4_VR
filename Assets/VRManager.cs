using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRManager : MonoBehaviour
{
    public GameObject XROrigin;

    void Start()
    {
        // 10秒ごとにチェックするコルーチン開始
        StartCoroutine(CheckHandTracking());
    }

    IEnumerator CheckHandTracking()
    {
        while (true)
        {
            bool available = IsHandTrackingAvailable();
            Debug.Log("Hand Tracking Available: " + available);

            if (XROrigin != null)
            {
                XROrigin.SetActive(available);
            }

            yield return new WaitForSeconds(3f);
        }
    }

    public static bool IsHandTrackingAvailable()
    {
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
}
