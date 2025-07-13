using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class pointCounter : NetworkBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public static List<stageObject> stageObjects;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        stageObjects = new List<stageObject>();
    }

    // Update is called once per frame
    void Update()
    {
        print(IsServer);
        if (IsServer)
        {
            // print(stageObjects);
            Dictionary<string, int> points = new Dictionary<string, int>();
            if (stageObjects != null)
            {
                foreach (stageObject stageObject in stageObjects)
                {
                    if (!points.ContainsKey(stageObject.getTeamName()))
                        points[stageObject.getTeamName()] = 0;
                    points[stageObject.getTeamName()] += stageObject.pointCount();

                }
            }
            string text = "";


            foreach (KeyValuePair<string, int> point in points)
            {
                text += $"{point.Key}:{point.Value}\n";

            }
            // print("ポイントを送信" + text);
            UpdatePointsClientRpc(text);
        }


    }


    [ClientRpc]
    private void UpdatePointsClientRpc(string scoreText)
    {
        if (text != null)
            text.text = scoreText;
    }
}
public interface stageObject
{
    public int pointCount();
    public string getTeamName();

}
