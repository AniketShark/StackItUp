using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ShapeButton : MonoBehaviour
{
	public Image current;
	public Image locked;

    public void Active()
	{
		current.gameObject.SetActive(true);
		locked.gameObject.SetActive(false);
	}

	public void Unlocked()	
	{
		current.gameObject.SetActive(false);
		locked.gameObject.SetActive(false);
	}

	public void Locked()
	{
		current.gameObject.SetActive(false);
		locked.gameObject.SetActive(true);
	}
}
