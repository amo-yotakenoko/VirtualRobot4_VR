using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.Json;

public class Settings : MonoBehaviour
{
    public static string path = Path.Combine(Directory.GetCurrentDirectory(), "settings.yaml");

    public static string load(string key, string defaultValue = null)
    {
        if (!File.Exists(path))
        {
            return defaultValue;
        }

        try
        {
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                // コメントと空行をスキップ
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                    continue;

                var split = line.Split(new[] { ':' }, 2);
                if (split.Length == 2)
                {
                    string k = split[0].Trim();
                    string v = split[1].Trim();
                    if (k == key)
                        return v;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"設定の読み込みに失敗しました: {ex.Message}");
        }

        return defaultValue;
    }


    public static void save(string key, string value)
    {
        var dict = new Dictionary<string, string>();

        // 既存の設定を読み込む
        if (File.Exists(path))
        {
            try
            {
                var lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                        continue;

                    var split = line.Split(new[] { ':' }, 2);
                    if (split.Length == 2)
                    {
                        string k = split[0].Trim();
                        string v = split[1].Trim();
                        dict[k] = v;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"設定の読み込みに失敗しました: {ex.Message}");
            }
        }

        // 値を更新
        dict[key] = value;

        // ファイルに書き出し
        try
        {
            using (var writer = new StreamWriter(path))
            {
                foreach (var kvp in dict)
                {
                    writer.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"設定の保存に失敗しました: {ex.Message}");
        }
    }


}
