using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;
using System.Collections;

namespace FuriousPlay
{
	public class ActionManager //: MonoBehaviour
	{
		private static Dictionary<string, UnityEvent> eventsList = new Dictionary<string, UnityEvent>();
		private static Dictionary<string, GenericEvent> genericEventsList = new Dictionary<string, GenericEvent>();

		//public void Awake()
		//{
		//	Instance = this;
		//}

		//public static ActionManager Instance;
		//{
		//	get {
		//		if(_instance == null)
		//		{
		//			_instance = FindObjectOfType(typeof(ActionManager)) as ActionManager;

		//			if (_instance == null)
		//				Debug.LogError("There is no ActionManager in the scene. Please add ActionManager script to one of the gameObject");
		//		}
		//		return _instance;
		//	}
		//}

		public static GenericEvent genericPoupEvents = new GenericEvent();

		public static void SubscribeToEvent(string eventName, UnityAction listener)
		{

			if (eventsList.ContainsKey(eventName))
			{
				eventsList[eventName].AddListener(listener);
			}
			else
			{
				UnityEvent uEvent = new UnityEvent();
				uEvent.AddListener(listener);
				eventsList.Add(eventName, uEvent);
			}
		}

		public static void SubscribeToEvent(string eventName, UnityAction<Hashtable> listener)
		{
			if (genericEventsList.ContainsKey(eventName))
			{
				genericEventsList[eventName].AddListener(listener);
			}
			else
			{
				GenericEvent gEvent = new GenericEvent();
				gEvent.AddListener(listener);
				genericEventsList.Add(eventName, gEvent);
			}
		}

		public static void UnsubscribeToEvent(string eventName, UnityAction listener)
		{
			if (eventsList.ContainsKey(eventName))
			{
				eventsList[eventName].RemoveListener(listener);
			}
		}

		public static void UnsubscribeToEvent(string eventName, UnityAction<Hashtable> listener)
		{
			if (genericEventsList.ContainsKey(eventName))
			{
				genericEventsList[eventName].RemoveListener(listener);
			}
		}

		public static void UnscribeAllToEvent(string eventName)
		{
			UnityEvent uEvent = null;
			if (eventsList.TryGetValue(eventName, out uEvent))
			{
				uEvent.RemoveAllListeners();
			}

			GenericEvent evt = null;
			if (genericEventsList.TryGetValue(eventName, out evt))
			{
				evt.RemoveAllListeners();
			}
		}

		public static void TriggerEvent(string eventName, Hashtable parameters = null)
		{
			if (eventsList.ContainsKey(eventName))
			{
				UnityEvent uEvent = null;
				if (eventsList.TryGetValue(eventName, out uEvent))
				{
					uEvent.Invoke();
				}
			}
			else if (genericEventsList.ContainsKey(eventName))
			{
				GenericEvent gEvent = null;
				if (genericEventsList.TryGetValue(eventName, out gEvent))
				{
					gEvent.Invoke(parameters);
				}
			}
			else
			{
				Debug.LogError(string.Format("No one has subscribed to {0} event", eventName));
			}


		}

		/// <summary>
		/// This type of event takes
		/// title
		/// message
		/// button names [{positive button}, {negative button}]
		/// button colors [{positive button color}, {negative button color}]
		/// actions [{positive action}, {negative action}]
		/// </summary>

		public class GenericEvent : UnityEvent<Hashtable> { }

	}
}
