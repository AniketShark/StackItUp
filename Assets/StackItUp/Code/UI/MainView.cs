using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FuriousPlay;
using System;

public class MainView : View
{
	public TextMeshProUGUI moves;
	public TextMeshProUGUI levelText;
	public GameObject settings;
	public GameObject removeAds;
	public GameObject replay;
	public GameObject skipLevel;

	private int movesCount;
	private void Awake()
	{
		ActionManager.SubscribeToEvent(GameEvents.STACK_LOAD_COMPLETE, OnLevelLoadComplete);
		ActionManager.SubscribeToEvent(GameEvents.CHECK_COMPLETE, UpdateMoves);
	}

	private void UpdateMoves()
	{
		movesCount++;
		moves.text = movesCount.ToString();
	}

	private void OnDestroy()
	{
		ActionManager.UnsubscribeToEvent(GameEvents.STACK_LOAD_COMPLETE, OnLevelLoadComplete);
		ActionManager.UnsubscribeToEvent(GameEvents.CHECK_COMPLETE, UpdateMoves);
	}

	public void OnLevelLoadComplete(Hashtable parameters)
	{
		movesCount = 0;
		moves.text = movesCount.ToString();
		levelText.text = "LEVEL " + (parameters.ContainsKey("level") ? parameters["level"].ToString() : "1");
	}

	public override void Hide()
	{
	}

	public override void Init(Hashtable data)
	{

	}

	public override void Show()
	{

	}


	public void SettingsClicked()
	{
		ActionManager.TriggerEvent(UIEvents.SETTINGS, new Hashtable() {
			{ "event", UIEvents.SETTINGS},
			{ "heptic", SaveManager.SaveData.heptic},
			{ "sound", SaveManager.SaveData.sound}
		});
	}
	public void RemoveAds()
	{

	}

	public void Replay()
	{
		ActionManager.TriggerEvent(GameEvents.RELOAD_LEVEL);
	}

	public void Skins()
	{
		ActionManager.TriggerEvent(UIEvents.SKINS, new Hashtable() {
			{ "event", UIEvents.SKINS}
		});
	}

	public void SkipLevel()
	{
		ActionManager.TriggerEvent(GameEvents.SKIP_LEVEL);
	}
}
