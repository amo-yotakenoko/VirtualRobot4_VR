using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;

public class WebGLRobotUpdate
{
	public static Dictionary<string, byte[]> robotDictionary { get; } = new Dictionary<string, byte[]>();

	// base64文字列を受け取って辞書に格納する関数
	public static void AddUpdateRobot(string key, string base64Data)
	{
		if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(base64Data))
		{
			throw new ArgumentException("key と base64Data は null または空であってはいけません。");
		}

		byte[] bytes;
		try
		{
			bytes = Convert.FromBase64String(base64Data);
		}
		catch (FormatException)
		{
			throw new ArgumentException("base64Data が正しいBase64形式ではありません。");
		}

		robotDictionary[key] = bytes;
	}
}
