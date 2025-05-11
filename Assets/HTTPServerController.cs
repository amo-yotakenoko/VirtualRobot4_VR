using UnityEngine;
using System.Net;
using System.Text;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;

public class HTTPServerController : MonoBehaviour
{
    public robotController robotController;
    private HttpListener listener;
    private Thread serverThread;

    private ConcurrentQueue<Request> requestQueue = new ConcurrentQueue<Request>();
    private ConcurrentQueue<Response> responseQueue = new ConcurrentQueue<Response>();



    void Start()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://*:8080/");
        listener.Start();

        serverThread = new Thread(ServerLoop);
        serverThread.Start();
        Debug.Log("HTTP Server started at http://localhost:8080/");
    }


    public class Request
    {
        public int id;
        public string requestText;


        public override string ToString()
        {
            return $"ResponseEntry {{ id = {id}, Request = \"{requestText}\"";
        }
    }
    [System.Serializable]
    public class Response
    {
        public string value = "0";
        public int id;
    }



    public int requestId;
    void ServerLoop()
    {
        while (listener.IsListening)
        {
            try
            {
                var context = listener.GetContext();
                var request = context.Request;
                var response = context.Response;
                print("response: " + response + " request: " + request.HttpMethod + "request.RawUrl: " + request.RawUrl);
                // CORSヘッダー
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "*");
                if (request.HttpMethod == "OPTIONS")
                {
                    // プリフライトリクエストへの応答（内容なしでOKを返す）
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Close();

                }
                else if (request.HttpMethod == "POST")
                {
                    string requestBody = string.Empty;
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        requestBody = reader.ReadToEnd();
                    }

                    // 受け取ったリクエストの表示
                    print("HTTP Server: " + request.HttpMethod + " " + request.RawUrl + " " + requestBody);

                    requestQueue.Enqueue(new Request
                    {
                        id = requestId++,
                        requestText = requestBody,

                    });


                    Response responseData = null;
                    while (true)
                    {
                        if (responseQueue.TryDequeue(out responseData))
                        {
                            break; // 取れたら抜ける
                        }
                        Thread.Sleep(10); // 少し待ってから再試行（CPU負荷軽減）
                    }
                    // レスポンスを設定
                    string responseText = JsonUtility.ToJson(responseData);
                    print("responseData: " + responseText + "を返した");
                    byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Close(); // ← これが重要
                }




            }
            catch (System.Exception e)
            {
                Debug.LogError("HTTP Server error: " + e.Message);
            }
        }
    }

    void Update()
    {
        // キューから順にエントリを取り出して処理
        while (requestQueue.TryDequeue(out var entry))
        {
            // エントリの情報を表示（ここではidとレスポンスの内容を表示）
            print($"ID: {entry.id}, Request: {entry.requestText}");

            Response responseData = commandExecute(entry.requestText);
            responseData.id = entry.id;
            responseQueue.Enqueue(responseData);
        }
    }

    private Response commandExecute(string commandText)
    {
        Response responseData = new Response();
        print("commandText: " + commandText);
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
                    responseData.value = "1";
                }
            }
            else if (command.type == "get")
            {
                string result = response(command.key);
                responseData.value = result;

            }


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

        }
        return "";
    }

    void OnDestroy()
    {
        if (listener != null && listener.IsListening)
        {
            listener.Stop();
            serverThread.Abort();
        }
    }





}


