using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
using FuriousPlay;
public class SkinsView : View
{
	[SerializeField]
	RectTransform rect;
	[SerializeField]
	List<ShapeButton> allShapes;
	[SerializeField]
	List<ShapeButton> allPatterns;
	[SerializeField]
	GridLayoutGroup shapeGrid;
	[SerializeField]
	GridLayoutGroup patternGrid;

	private Vector3 startPosition;
	private ShapeButton currentShape;
	private ShapeButton currentPattern;


	private void Awake()
	{
	}

	private void Start()
	{
		startPosition = transform.localPosition;

		int    currentStack   = SaveManager.SaveData.currentStack;
		string currentPattern = SaveManager.SaveData.currentPattern;

		foreach (ShapeButton shape in allShapes)
		{
			shape.Unlocked();
			if(shape.name.Equals(currentStack.ToString()))
			{
				currentShape = shape;
				shape.Active();
			}
		}

		foreach (ShapeButton shape in allPatterns)
		{
			shape.Unlocked();
			if (shape.name.Equals(currentPattern))
			{
				shape.Active();
			}
		}
	}

	public override void Hide()
	{
		transform.DOLocalMoveX(startPosition.x, 0.3f);
	}

	public override void Init(Hashtable data)
	{
	}

	public override void Show()
	{
		transform.DOLocalMoveX(0, 0.3f);
		OnSkinsClicked();
	}

	public void ShapeSelected(GameObject go)
	{
		if (currentShape != null)
		{
			currentShape.Unlocked();
		}

		currentShape = go.GetComponent<ShapeButton>();
		currentShape.Active();
		ActionManager.TriggerEvent(GameEvents.STACK_CHANGE, new Hashtable() {
			{"stack", int.Parse(currentShape.name)}
		});
	}

	public void OnPatternSelected(GameObject go)
	{
		if (currentPattern != null)
		{
			currentPattern.Unlocked();
		}
		currentPattern = go.GetComponent<ShapeButton>();
		currentPattern.Active();
		ActionManager.TriggerEvent(GameEvents.PATTERN_CHANGE, new Hashtable() {
			{"pattern", currentPattern.name}
		});
	}

	public void OnSkinsClicked()
	{
		shapeGrid.gameObject.SetActive(true);
		patternGrid.gameObject.SetActive(false);
	}

	public void OnPatternClicked()
	{
		shapeGrid.gameObject.SetActive(false);
		patternGrid.gameObject.SetActive(true);
	}

}
