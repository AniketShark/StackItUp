using FuriousPlay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
	public int currentLevel;
	public int currentStack;
	public string currentPattern;
	public Pattern currentPatternData;
	public LevelData currentLevelData;
	public StackData currentStackData;
	public PinSetup activeSetup;
	public List<Color> colors;
	public Globals data;

	public List<StackPin> stackPins;
	public List<GameObject> pooledGameObjects;
	public List<PinSetup> pinsetUps;
	public Transform poolParent;
	public int moves;
	public ConfettiSequence confettiSequence;
	private int stacksToWin;

	private void Awake()
	{
		ActionManager.SubscribeToEvent(GameEvents.CHECK_COMPLETE, CheckStackCompletion);
		ActionManager.SubscribeToEvent(GameEvents.RELOAD_LEVEL, Reload);
		ActionManager.SubscribeToEvent(GameEvents.SKIP_LEVEL, SkipLevel);
		ActionManager.SubscribeToEvent(GameEvents.STACK_CHANGE, StackChange);
		ActionManager.SubscribeToEvent(GameEvents.PATTERN_CHANGE, PatternChange);
	}

	private void OnDestroy()
	{
		ActionManager.UnsubscribeToEvent(GameEvents.CHECK_COMPLETE, CheckStackCompletion);
		ActionManager.UnsubscribeToEvent(GameEvents.RELOAD_LEVEL, Reload);
		ActionManager.UnsubscribeToEvent(GameEvents.SKIP_LEVEL, SkipLevel);
		ActionManager.UnsubscribeToEvent(GameEvents.STACK_CHANGE, StackChange);
		ActionManager.UnsubscribeToEvent(GameEvents.PATTERN_CHANGE, PatternChange);
	}

	private void StackChange(Hashtable parameters)
	{
		currentStack = (int)parameters["stack"];
		SaveManager.SaveData.currentStack = currentStack;
		ActionManager.TriggerEvent(GameEvents.SAVE_GAME, new Hashtable() { { "stack", currentStack } });
		currentStackData = Resources.Load<StackData>("Stacks/" + currentStack.ToString());
		foreach (StackPin stack in stackPins)
		{
			stack.stackData = currentStackData;
			stack.StackChanged();
		}
		stackPins[0].UpdateSelectedTile();
	}

	private void PatternChange(Hashtable parameters)
	{
		currentPattern = parameters["pattern"].ToString();
		SaveManager.SaveData.currentPattern = currentPattern;
		ActionManager.TriggerEvent(GameEvents.SAVE_GAME, new Hashtable() { { "pattern", currentPattern } });
		currentPatternData = data.allPatterns.Find(item => item.name.Equals(currentPattern));
		foreach (Material material in data.allMaterials)
		{
			material.mainTexture = currentPatternData.patternTexture;
		}
	}

	private void Start()
	{
		name = "Game";
		data = Resources.Load<Globals>("Globals");
		currentLevel = 1;//SaveManager.SaveData.currentLevel;

		currentStack = SaveManager.SaveData.currentStack;
        Debug.LogError("currentStack.ToString()"+ currentStack.ToString());
		currentStackData = Resources.Load<StackData>("Stacks/" + currentStack.ToString());
		LoadLevel(currentLevel);
	}

	private void Initialize()
	{
		int index = 1;
		foreach(StackPin stack in stackPins)
		{
			stack.levelData = currentLevelData;
			stack.stackData = currentStackData;
			stack.Init();
			stack.pinIndex = index;
			index++;
		}
	}

    private string getLevelName(int index)
    {
        var info = new DirectoryInfo("Assets/StackItUp/Resources/Levels");
        var fileInfo = info.GetFiles();

		
 //   #if UNITY_EDITOR
 //       if (isTesting)
 //       {
 //           for(int i = 0; i < fileInfo.Length; i++)
 //           {
 //               if (fileInfo[i].Name.Split('.')[0] == testLevelId)
 //                   index = i;
 //           }
 //       }
      
	//#endif

       
        return fileInfo[index % fileInfo.Length].Name.Split('.')[0];
    }

    private static int genIndex = 0;

//    public LevelData GetNextGeneratedLevel()
//    {
        //LevelData level = new LevelData();
        //if(LevelsFiles == null || LevelsFiles.Length == 0)
        //{
        //    var info = new DirectoryInfo("Assets/StackItUp/Resources/generated");
        //    LevelsFiles = new string[info.GetFiles().Length];
        //    var files = info.GetFiles();
        //    Debug.Log("files"+files.Length);
        //    for (int i = 0; i < files.Length; i++)
        //    {

        //        LevelsFiles[i] = files[i].Name.Split('.')[0];
        //    }
        //}
        //Debug.LogError("Assets/StackItUp/Resources/generated/" + LevelsFiles[0]);
        //LevelsData levelsInfo = Resources.Load<LevelsData>("Assets/StackItUp/Resources/generated/"+LevelsFiles[0]);
        //level = LevelData.TransformToLevelData(levelsInfo.AllLevels[0]);
        //return level;

        //return GetComponent<LevelsManager>().GetTestLevel();
//    }
    public void LoadLevel(int level)
	{
		if (activeSetup != null)
			activeSetup.gameObject.SetActive(false);

		currentLevel = level;

		// Debug.Log("file name"+ getLevelName(level));
//		Resources.Load<LevelData>("Levels/" + getLevelName(level));
//		if (isTesting)
//		{
//#if UNITY_EDITOR
//			currentLevelData = Resources.Load<LevelData>("Levels/" + getLevelName(level));
//#endif
//		}
//		else
			currentLevelData = Resources.Load<LevelData>("Levels/" + level.ToString());
		// currentLevelData = GetNextGeneratedLevel();

		if (currentLevelData == null)
		{
			//Debug.LogError("No More Levels Available " + level);
			currentLevel = 1;
			currentLevelData = Resources.Load<LevelData>("Levels/" + currentLevel.ToString());
			//return;
		}

		int pinSetupIndex = currentLevelData.pins;
		activeSetup = pinsetUps[pinSetupIndex];
		activeSetup.gameObject.SetActive(true);
		stackPins = activeSetup.stackPins;
		activeSetup.fov.CheckFov();
		Initialize();
		foreach(PinConfig config in currentLevelData.pinConfig)
		{
			LoadStack(currentStackData, config,currentLevelData.colors);
		}

		stacksToWin = currentLevelData.stackCount;
		ActionManager.TriggerEvent(GameEvents.STACK_LOAD_COMPLETE,new Hashtable() {
			{"level",currentLevel} 
		});
	}

	public void Reload()
	{
		foreach (StackPin stack in stackPins)
		{
			stack.RestoreToPool(pooledGameObjects,poolParent);
			stack.Reset();
		}
		LoadLevel(currentLevel);
	}

	public void SkipLevel()
	{
		int level = currentLevel + 1;

		ActionManager.TriggerEvent(GameEvents.SAVE_GAME, new Hashtable() {
			{"level",level}
		});
		foreach (StackPin stack in stackPins)
		{
			stack.RestoreToPool(pooledGameObjects, poolParent);
			stack.Reset();
		}
		LoadLevel(level);
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.tag.Equals("Ball"))
		{
			Reload();
		}
	}
	
	public void LoadStackRandom(StackData stack,int stacksToFill, int maxStackPins)
	{
		stacksToWin = stacksToFill;
		if (stacksToFill > maxStackPins)
		{
			Debug.LogError("Pins to fill is higher than max pins");
			return;
		}

		stackPins.Shuffle();
		colors.Shuffle();

		Color[] randomColors = new Color[stacksToFill];

		for (int i = 0;i < stacksToFill; i++)
		{
			randomColors[i] = colors[i];
			//randomStackpins[i] = stackPins[i];
		}

		for(int j = 0;j < stacksToFill;j++)
		{
			int colorCode = j;
			Color color = randomColors[j];
			stack.meshes.Shuffle();
			for (int i = 0; i < stack.meshes.Count;i++)
			{
				GameObject tile = pooledGameObjects[0];
				pooledGameObjects.RemoveAt(0);
				var stackTile = tile.GetComponent<StackTile>();
				stackTile.SetData(stack);
				stackTile.colorCode = colorCode;
				stackTile.index = stack.meshes[i].index;
				stackTile.SetMesh(stack.meshes[i].mesh);
				stackTile.SetMaterials(stack.materials.ToArray());
				//stackTile.SetMaterialColor(color);
				stackPins.Shuffle();
				StackPin stackPin = stackPins[0];
				stackTile.pinIndex = stackPin.pinIndex;
				stackPin.PushTile(stackTile.gameObject);
			}
		}

		ActionManager.TriggerEvent(GameEvents.STACK_LOAD_COMPLETE, new Hashtable() {
			{"level",currentLevel}
		});
		//ActionManager.TriggerEvent(GameEvents.STACK_LOAD_COMPLETE);
	}

	public void LoadStack(StackData stack,PinConfig config,List<Material> colors)
	{
		for (int j = 0; j < config.tiles.Count; j++)
		{
			var tileInfo = config.tiles[j];
			{
				GameObject tile = pooledGameObjects[0];
				pooledGameObjects.RemoveAt(0);
				var stackTile = tile.GetComponent<StackTile>();
				stackTile.SetData(stack);
				stackTile.colorCode = tileInfo.colorIndex;
				stackTile.index = tileInfo.size;
				stackTile.SetMesh(stack.meshes[tileInfo.size - 1].mesh);
				stackTile.SetMaterials(stack.materials.ToArray());
				stackTile.SetMaterialColor(colors[tileInfo.colorIndex]);
				StackPin stackPin = stackPins[config.pinIndex];
				stackTile.pinIndex = stackPin.pinIndex;
				stackPin.PushTile(stackTile.gameObject);
			}
		}

	}

	public void CheckStackCompletion()
	{
		moves++;
		int count = stacksToWin;
		foreach(StackPin stack in stackPins)
		{
			if(stack.CheckComplete())
			{
				count--;
			}
		}

		if(count <= 0)
		{
			Debug.LogError("You win");
			Win();
		}
	}

	public void Win()
	{
		ActionManager.TriggerEvent(GameEvents.SAVE_GAME, new Hashtable() {
			{"level",currentLevel+1},
			{"score",10 }
		});
		//foreach (StackPin stack in stackPins)
		//{
		//	stack.Celebrate();
		//}
		StartCoroutine("OnCelebrationComplete");
	}

	IEnumerator OnCelebrationComplete()
	{
        Debug.Log("OnCelebrationComplete");
        confettiSequence.RandomShoot();
		yield return new WaitForSeconds(1.5f);
		ActionManager.TriggerEvent(UIEvents.RESULT,new Hashtable() {
			{ "event", UIEvents.RESULT},
			{ "smiley", "UI_Icon_SmileyHappy"},
			{ "message", "Level is Completed"},
			{ "emotion", "Awesome!"},
			{ "callback",(Action<MessageBoxStatus>)ResultCallback}
		});

		foreach (StackPin stack in stackPins)
		{
			stack.RestoreToPool(pooledGameObjects, poolParent);
			stack.Reset();
		}

		if(SaveManager.SaveData.heptic)
			Handheld.Vibrate();
	}

	public void ResultCallback(MessageBoxStatus status)
	{
		if(activeSetup != null)
			activeSetup.gameObject.SetActive(false);

		int level = currentLevel + 1;
		if (level > 8)
		{
			level = UnityEngine.Random.Range(1, 8);
		}

		if (status == MessageBoxStatus.OK)
		{
			LoadLevel(level);
		}
	}

	private T[] ShuffleArray<T>(T[] array)
	{
		System.Random r = new System.Random();
		for (int i = array.Length; i > 0; i--)
		{
			int j = r.Next(i);
			T k = array[j];
			array[j] = array[i - 1];
			array[i - 1] = k;
		}

		return array;
	}
#if UNITY_EDITOR
    [Header("UNIT TEST")]
    [SerializeField] bool isTesting = false;
    [SerializeField] string testLevelId = "1";
    private void LateUpdate()
    {

    }

#endif
}

public static class IListExtensions
{
	/// <summary>
	/// Shuffles the element order of the specified list.
	/// </summary>
	public static void Shuffle<T>(this IList<T> ts)
	{
		var count = ts.Count;
		var last = count - 1;
		for (var i = 0; i < last; ++i)
		{
			var r = UnityEngine.Random.Range(i, count);
			var tmp = ts[i];
			ts[i] = ts[r];
			ts[r] = tmp;
		}
	}
}
