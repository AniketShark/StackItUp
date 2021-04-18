using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DLC
{


	public static T LoadAsset<T>(string path)
	{
		Object asset = Resources.Load(path);

		if(asset is T)
		{
			Debug.Log("Found Asset");
			//return asset as T;
		}

		return default(T);
	}

}
