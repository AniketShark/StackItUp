using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ResultView : View
{
	public GameObject  reward;
	public TextMeshProUGUI gems;
	public GameObject  backButton;
	public TextMeshProUGUI emotion;
	public TextMeshProUGUI message;
	public Image smiley;
	public SpriteAtlas atlas;
	public GameObject multiplyButton;
	public TextMeshProUGUI tapToContinue;


	private Action<MessageBoxStatus> _callback;
	public override void Init(Hashtable data)
	{
		//TODO: show result screen
		emotion.text = data.ContainsKey("emotion") ? data["emotion"].ToString() : "";
		message.text = data.ContainsKey("message") ? data["message"].ToString() : "";
		//smiley.sprite = atlas.GetSprite(data.ContainsKey("smiley") ? data["smiley"].ToString() : "");
		_callback = (data.ContainsKey("callback") ? (Action<MessageBoxStatus>)data["callback"] : null);
	}

	public override void Hide()
	{
		if (_callback != null)
			_callback(MessageBoxStatus.OK);

		gameObject.SetActive(false);
		
	}

	public override void Show()
	{
		gameObject.SetActive(true);
	}
}
