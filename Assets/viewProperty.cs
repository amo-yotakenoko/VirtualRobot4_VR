using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
public class viewProperty : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public static bool enable;
    private bool previousEnableState = false;
    void Update()
    {
        if (enable != previousEnableState)
        {
            print("enable変更: " + enable);
            previousEnableState = enable;

            if (enable)
            {

                EnableView(GetAllPlayerDevices());
            }
            else
            {
                DisableView();
            }
        }
    }

    public void enableChange()
    {
        enable = !enable;
    }


    /* ─────────────── ② TMP 生成/削除ロジック ─────────────── */

    public GameObject TMPObject;     // TextMeshPro のプレハブ
    private readonly List<GameObject> _spawned = new();

    public void EnableView(List<robotController.Device> deviceList)
    {
        foreach (var device in deviceList)
        {
            // if (device == null || device.transform == null) continue;

            // 表示用TMPを複製
            var go = Instantiate(TMPObject, device.transform.position, Quaternion.identity);
            // go.transform.SetParent(device.transform);               // デバイスに追従

            propertyObj propertyObj = go.GetComponent<propertyObj>();
            propertyObj.device = device; // デバイス情報を設定



            _spawned.Add(go);
        }
    }

    public void DisableView()
    {
        foreach (var go in _spawned)
            if (go) Destroy(go);
        _spawned.Clear();
    }

    public static List<robotController.Device> GetAllPlayerDevices()
    {
        var robots = Object.FindObjectsOfType<robotController>(true);
        var results = robots
          .Where(r => r != null && r.deviceList != null)
          .Where(x => x.IsOwner)
          .SelectMany(r => r.deviceList)
          .GroupBy(d => new { d.name, d.type })
          .Select(g => g.First())
          .ToList();
        print(results.Count + " devices found.");
        return results;
    }
}
