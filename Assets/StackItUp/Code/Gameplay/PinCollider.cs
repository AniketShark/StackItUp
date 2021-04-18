using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinCollider : MonoBehaviour
{
	[SerializeField]
	StackPin pin;

	public void OnMouseUp()
	{
		if (StackPin.SelectedTile != null)
		{
			pin.PushTile(StackPin.SelectedTile);
		}
		else
		{
			pin.PopTile();
		}
	}
}
