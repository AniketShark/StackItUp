using FuriousPlay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBGPicker : MonoBehaviour
{
	public List<Sprite> randomBgs;

	void Awake()
	{
		//ActionManager.SubscribeToEvent(GameEvents.STACK_LOAD_COMPLETE, SetRandomBG);
		//ActionManager.SubscribeToEvent(GameEvents.SET_RANDOM_BG, SetRandomBG);
	}
	private void Start()
	{
		
	}
	void OnDestroy()
	{
		//ActionManager.UnsubscribeToEvent(GameEvents.STACK_LOAD_COMPLETE, SetRandomBG);
		//ActionManager.UnsubscribeToEvent(GameEvents.SET_RANDOM_BG, SetRandomBG);
	}

	private void SetRandomBG(Hashtable parameters)
	{
		int random = UnityEngine.Random.Range(0, randomBgs.Count);

		if(randomBgs.Count > 0)
		{
			GetComponent<Image>().sprite = randomBgs[random];
		}
	}
}
