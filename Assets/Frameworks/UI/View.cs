using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class View : MonoBehaviour
{
	public abstract void Init(Hashtable data);
	public abstract void Show();

	public abstract void Hide();
	
}
