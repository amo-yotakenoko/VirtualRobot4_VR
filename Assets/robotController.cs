using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

using Unity.Netcode;
public class robotController : Unity.Netcode.NetworkBehaviour
{
    // Start is called before the first frame update
    // public List<string> jsons;
    void Start()
    {
        // foreach (var json in jsons)
        // {


        //     addDevice(json);

        // }

    }
    void Update()
    {
        // foreach (var device in deviceList)
        // {
        //     print(device);
        //     if (device.GetType() == typeof(servo)){

        //     }
        // }
    }
    public void test()
    {
        print("test");
    }
    public void setvalue(string name, string property, float value)
    {
        setvalueServerRPC(name, property, value);
    }

    [ServerRpc]
    void setvalueServerRPC(string name, string property, float value)
    {
        foreach (var device in deviceList)
        {

            if (device.name == name)
            {
                // Device 型の power プロパティにアクセス
                PropertyInfo powerProperty = device.GetType().GetProperty(property);
                print(powerProperty);
                if (powerProperty != null)
                {
                    // motor クラスで定義された power プロパティにアクセスして値を設定
                    powerProperty.SetValue(device, value);
                }
            }

        }
    }


    public List<Device> deviceList = new List<Device>();

    public Device addDevice(string json)
    {
        string splitjson = "";
        int indent = 0;
        foreach (char c in json)
        {
            if (c == '{') indent++;
            if (indent > 0) splitjson += c;
            if (c == '}') indent--;
        }
        if (splitjson.Length > 0)
        {

            Device baseDevice = JsonUtility.FromJson<Device>(splitjson);
            if (baseDevice != null)
            {
                Type deviceType = Assembly.GetExecutingAssembly().GetType("robotController+" + baseDevice.type);
                // print(deviceType + "クラス"); // クラスの型情報を出力
                if (deviceType != null && deviceType.IsSubclassOf(typeof(Device)))
                {
                    // deviceTypeに基づいてインスタンスを作成する
                    Device device = Activator.CreateInstance(deviceType) as Device;
                    // device.type = "aa";
                    // JSON文字列からデータをデシリアライズしてインスタンスに適用する
                    JsonUtility.FromJsonOverwrite(splitjson, device);
                    deviceList.Add(device);
                    // print(device.name + deviceList.Count);
                    return device;
                }
                if (deviceType == null)
                {
                    return baseDevice;
                }

            }
        }

        return null;
    }



    // MotorDataクラスの定義
    [System.Serializable]
    public class Device
    {
        public string name;
        public string type;
    }
    public class motor : Device
    {
        private float _power;//無限ループ防止
        public HingeJoint HingeJoint;
        public float power
        {
            get { return _power; }
            set
            {
                _power = value;

                // Debug.Log("パワーを" + value + "に");
                JointMotor motor = HingeJoint.motor;
                motor.targetVelocity = value; // 回転速度を設定
                HingeJoint.motor = motor;
            }
        }

    }
    public class servo : Device
    {
        // public servo()
        // {
        //     StartCoroutine(moveServo());
        // }
        // IEnumerator moveServo()
        // {
        //     while (true)
        //     {
        //         yield return null;
        //         print("servo");
        //     }
        // }
        public servoControl servoControl;

        private float _angle;//無限ループ防止
        // public HingeJoint HingeJoint;
        public float angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                servoControl.angle = value;
                // // Debug.Log("パワーを" + value + "に");
                // JointMotor motor = HingeJoint.motor;


                // float speed = 1;
                // float maxspeed = 10;

                // motor.targetVelocity = Mathf.Clamp((value - HingeJoint.angle) * speed, -maxspeed, maxspeed);
                // print(motor.targetVelocity);
                // HingeJoint.motor = motor;

            }
        }

    }


    public class light : Device
    {

        public Light lightComponent;

        public float intensity
        {
            get { return lightComponent.intensity; }
            set
            {
                print(lightComponent);
                if (lightComponent != null)
                    lightComponent.intensity = value;
            }
        }

    }
    public class camera : Device
    {


    }


}


