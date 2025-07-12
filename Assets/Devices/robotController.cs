using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

using Unity.Netcode;
public class robotController : Unity.Netcode.NetworkBehaviour
{
    [System.Serializable]
    public class DeviceListWrapper
    {
        public List<robotController.Device> devices;
    }

    public Response commandExecute(string commandText)
    {
        Response responseData = new Response();
        print("commandText: " + commandText);
        CommandData command = JsonUtility.FromJson<CommandData>(commandText);
        responseData.id = command.id;


        if (command.type == "set")
        {
            var parts = command.key.Split('.');
            print($"{parts.Length},{parts[0]}");
            if (parts.Length == 1 && parts[0] == "activeViewproperty")
            {
                float value = float.Parse(command.value);
                print($"activeViewproperty{value}");
                viewProperty.enable = value == 1;
                responseData.value = viewProperty.enable.ToString();
            }
            else if (parts.Length == 2)
            {

                string name = parts[0];
                string property = parts[1];
                // Debug.Log($"First: {first}, Second: {second}");
                float value = float.Parse(command.value); // value
                setvalue(name, property, value);
                responseData.value = "1";
            }
        }
        else if (command.type == "get")
        {
            string result = response(command.key);
            responseData.value = result;

        }
        else if (command.type == "teleport")
        {
            Vector3 offset = new Vector3(command.x, command.y, command.z);
            print("teleport" + offset);
            player.ownerPlayer.rescueServerRpc(offset);
            // rescue.Instance.rescueStart(offset);
            responseData.value = "1";

        }



        return responseData;
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
            else if (parts[0] == "VRright" || parts[0] == "VRleft")
            {
                return VRManager.GetControllerInput(parts[0], parts[1]);
            }

            else
            {
                return getvalue(parts[0], parts[1]) ?? "";
            }


        }
        else
        {
            if (parts[0] == "info")
            {
                var wrapper = new DeviceListWrapper { devices = viewProperty.GetAllPlayerDevices() };


                string result = JsonUtility.ToJson(wrapper, true);
                print("プロパティ情報" + result);
                // result = "プロパティ情報";
                return result;
            }
        }
        return "";
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
        print(deviceList.Count + "個のデバイス");
        foreach (var device in deviceList)
        {
            print(device.name + "の" + property + "を" + value + "に");
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


    public string getvalue(string name, string property)
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
                    return powerProperty.GetValue(device).ToString();

                }
            }

        }
        return null;
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
        public Transform transform;
        public virtual string toString()
        {
            return $"{name} ({type})";
        }
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


    public class distanceSensor : Device
    {

        public DistanceSensor sensor;

        public float distance
        {
            get { return sensor.getDistance(); }
            set
            {
            }
        }
        public override string toString()
        {
            return base.toString() + $" ({distance}m)";
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
        public override string toString()
        {
            return "";
        }

    }


    public class goal : Device
    {


    }


}


/*

現在、アプリ開発を勉強しているのですが、アプリのアイディアを考えたり思いついたりする上で心掛けている事や何か重要だと思うことがあればアドバイスをいただきたいです！

質問ありがとうございます!

アプリ開発だと私は他であまり見ないタイプのものが多いので参考になるかわからないですが私が作ったアプリのアイデアと動機をざっと書いてみると
①編模様(イラスト手編み支援)→一応編み物用だが動機の半分はminecraftのドット絵を作るため
②pompompattern(ぽんぽん手芸支援)→母が読んでた手芸本の設計図がどうやって作られているかの検証
③VirtualRobot(Unity上にあるロボットをscratchから動かす)→コロナ禍でロボットを触っていたサークルがオンラインになった、学校のアイデア公募に出してVR機器が欲しかった(買ってもらえた)
④ColorSuggester(色の組み合わせを提案する)→配色デザインの本をいちいち開くのがめんどくさい、本で読んだ色彩調和論を試したい
⑤ペーパークラフト設計図作成プログラム(未公開)→趣味で触っていたペーパークラフト設計ソフトが使いにくい
など
・身近にあるちょっと不便なことを自動化して便利にしたい④⑤
・思いついたアルゴリズムを技術的に自分で作ることができるかのチャレンジ①②④⑤
・他にもSNS(twitter,zenn,quita等)で見たライブラリなどを使ってみたいというところから②(画像検出の巻き数カウンタ機能)③(マルチプレイ機能)
などから作り始めています。
個人的にはアプリのアイデアの元になるものは本人やそれに近い人の趣味、興味等から持ってきた方がモチベーションなど何かと開発はしやすいと思うのですが皆さんそれでTODOアプリを作りがちみたいな部分はあるのでなかなか難しいところではありますね...(私も毎回完成した後はこれ以上のアイデアは一生出ないだろうと思いながら公開しています)
また質問者さんの環境がどうかわかりませんが自分は授業中の余った時間、通学時間が長いので電車の中など何もすることがない中でアイデアが固まることが多かったと思うのですることが何もない時間も重要だったかもしれないです。

質問から多少話は逸れますが良いアイデアがあってもそのアプリに必要な機能の絞り込み、紹介、アピール等勿体ないなと思う人をよく見るのでちょっと宣伝っぽくはなってしまいますが未踏ジュニアのメンター陣がそれぞれどんな風に紹介したら伝わりやすいかみたいなブログを書いていたりするのでコンテスト等出すつもりがなくても一度読んでほしいです。
https://note.com/yoshifumiseki/n/n1e928281d7dc
https://note.com/teramotodaiki/n/n148d35899016
https://zenn.dev/reputeless/articles/idea-mitoujr

それなりに長文になってしまったのですが他人のアイデア、アプリなど見せてもらうの好きなので自慢のアプリができたら是非見せてください!
*/