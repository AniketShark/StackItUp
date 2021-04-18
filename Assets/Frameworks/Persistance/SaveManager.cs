using FuriousPlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Save
{
	public int currentLevel;
	public int currentStack;
	public string currentPattern;
	public int score;

	public bool heptic;
	public bool noads;
	public bool sound;
}

public class SaveKeys
{
	public const string GAME_STATE = "game_state";
	public const string PLAYER_STATE = "player_state";
}

public class SaveManager : MonoBehaviour
{
	public static Save _SaveData;
	public static Save SaveData {
		get {
			if (_SaveData == null)
			{
				string save = PlayerPrefs.GetString(SaveKeys.PLAYER_STATE);
				if (string.IsNullOrEmpty(save))
				{
					var freshSave = new Save();
					freshSave.currentLevel = 1;
					freshSave.currentStack = 1;
					freshSave.currentPattern = "none";
					freshSave.score = 0;
					freshSave.heptic = true;
					freshSave.noads = false;
					save = JsonUtility.ToJson(freshSave);
					PlayerPrefs.SetString(SaveKeys.PLAYER_STATE, save);
					PlayerPrefs.Save();
				}
				_SaveData = JsonUtility.FromJson<Save>(save);
			}
			return _SaveData;
		}
	}

	private void Start()
	{
		ActionManager.SubscribeToEvent(GameEvents.SAVE_GAME, UpdateState);
		ActionManager.SubscribeToEvent(GameEvents.SAVE_SETTINGS, UpdateState);
	}

	private void OnDestroy()
	{
		ActionManager.UnsubscribeToEvent(GameEvents.SAVE_GAME, UpdateState);
		ActionManager.UnsubscribeToEvent(GameEvents.SAVE_SETTINGS, UpdateState);
	}

	public void UpdateState(Hashtable data)
	{
		//Debug.LogError("updating game state");
		if(data.ContainsKey("level"))
		{
			SaveData.currentLevel = (int)data["level"];
		}

		if (data.ContainsKey("stack"))
		{
			SaveData.currentStack = (int)data["stack"];
		}

		if (data.ContainsKey("score"))
		{
			SaveData.score = (int)data["score"];
		}

		if(data.ContainsKey("heptic"))
		{
			SaveData.heptic = (bool)data["heptic"];
		}
		if(data.ContainsKey("noads"))
		{
			SaveData.noads = (bool)data["noads"];
		}

		if (data.ContainsKey("sound"))
		{
			SaveData.noads = (bool)data["sound"];
		}

		if (data.ContainsKey("pattern"))
		{
			SaveData.currentPattern = data["pattern"].ToString();
		}
		Save();
	}

	public void Save()
	{
		string save = JsonUtility.ToJson(SaveData);
		//Debug.LogError("Save : " + save);
		PlayerPrefs.SetString(SaveKeys.PLAYER_STATE, save);
		PlayerPrefs.Save();
		_SaveData = null;
	}
	
}
