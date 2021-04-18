using System;
using System.Collections;
using System.Collections.Generic;
using FuriousPlay;
using UnityEditor;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    public int Pins, Colors, DiscSizes, Moves;
    private string lastFileName = null;

    [HideInInspector]
    public int currentLevelIndex = 0;

    private LevelsData levelsInfo = null;
    private string fileFormater = "generated/{0}Pin{1}Colors{2}Size/{0}Pin{1}Colors{2}Size{3}Moves";

    Game gameManager;

    IDictionary<string, object> SavedLevelData = null;
    private void Awake()
    {
        gameManager = GetComponent<Game>();
        ActionManager.SubscribeToEvent(UIEvents.RESULT, OnLevelComplete);
        if (PlayerPrefs.HasKey("exported_levels"))
        {
            SimpleJson.JsonObject obj = (SimpleJson.JsonObject)(SimpleJson.SimpleJson.DeserializeObject(PlayerPrefs.GetString("exported_levels")));
            SavedLevelData = obj;
        }
        else
            SavedLevelData = new Dictionary<string, object>();

        Debug.Log("Awake POST ===" + JsonUtility.ToJson(SavedLevelData));
         
    }

    private void OnDestroy()
    {
        ActionManager.UnsubscribeToEvent(UIEvents.RESULT, OnLevelComplete);
    }

    private void OnLevelComplete(Hashtable paramaters)
    {
        currentLevelIndex++;
    }
    private LevelData selectedLevelData;
    public LevelData GetTestLevel()
    {
        if (levelsInfo == null)
        {
            levelsInfo = Resources.Load<LevelsData>(string.Format(fileFormater, Pins, Colors, DiscSizes, Moves));
            _maxLevels = levelsInfo.AllLevels.Count;
        }
       
        if (currentLevelIndex >= levelsInfo.AllLevels.Count)
            Debug.LogError("ALL LEVELS EXPLORED!!!");
        IsSelected = SavedLevelData.ContainsKey(key);
        if (currentLevelIndex >= levelsInfo.AllLevels.Count)
            currentLevelIndex %= levelsInfo.AllLevels.Count;
        selectedLevelData = LevelData.TransformToLevelData(levelsInfo.AllLevels[currentLevelIndex % levelsInfo.AllLevels.Count]);
        return selectedLevelData;
    }
    public void ReloadFolder()
    {

        levelsInfo = Resources.Load<LevelsData>(string.Format(fileFormater, Pins, Colors, DiscSizes, Moves));
        currentLevelIndex = 0;
        _maxLevels = levelsInfo.AllLevels.Count;
        ActionManager.TriggerEvent(GameEvents.RELOAD_LEVEL);
    }
    private string LevelDataExportPath
    {
        get
        {
            string dir = string.Format(DIRECTORY_NAME_FORMATTER, Pins, Colors, DiscSizes);
            return dataPath + "/" + dir + string.Format(FILE_NAME_FORMATTER, Pins, Colors, DiscSizes, Moves, currentLevelIndex);
        }
    }
    private string key
    {
        get
        {
            return string.Format(fileFormater, Pins, Colors, DiscSizes, Moves) + ":" + currentLevelIndex;
        }
    }

    string DIRECTORY_NAME_FORMATTER = "{0}Pin{1}Colors{2}Size";
    string FILE_NAME_FORMATTER = "/{5}_{0}Pin{1}Colors{2}Size{3}Moves{4}.asset";
    private string dataPath = "Assets/StackItUp/Code/editor/resources/selected";

    private LevelsData.Data currentLevelData;
    [SerializeField]
    bool IsSelected = false;
    public void ShowNext()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levelsInfo.AllLevels.Count)
            currentLevelIndex = levelsInfo.AllLevels.Count - 1;
        ActionManager.TriggerEvent(GameEvents.RELOAD_LEVEL);
    }

    public void ShowPrevious()
    {
        currentLevelIndex--;
        if (currentLevelIndex < 0)
            currentLevelIndex = 0; ;
        ActionManager.TriggerEvent(GameEvents.RELOAD_LEVEL);
    }

    public void LoadLevelDirectly(int index)
    {
        if (index < 0 || index >= GetMaxLevels())
            return;

        currentLevelIndex = index;
        ActionManager.TriggerEvent(GameEvents.RELOAD_LEVEL);
    }
    public void DeleteLevel()
    {
        PopupWindow.ShowWindow("Do you want to delete level?", status =>
        {
            if (status)
            {
                if (SavedLevelData.ContainsKey(key))
                    SavedLevelData.Remove(key);

                AssetDatabase.DeleteAsset(LevelDataExportPath);
            }
            else
            {
                return;
            }
        });
    }


    public void MarkSelected()
    {
        // levelsInfo.AllLevels[currentLevelIndex] = currentLevelData;
        // string key = string.Format(fileFormater, Pins, Colors, DiscSizes, Moves) + ":" + currentLevelIndex;
        if (false == SavedLevelData.ContainsKey(key))
            SavedLevelData.Add(key, true);
        else
            SavedLevelData[key] = true;

        PlayerPrefs.SetString("exported_levels", SimpleJson.SimpleJson.SerializeObject(SavedLevelData));

        PlayerPrefs.Save();

        string dir = string.Format(DIRECTORY_NAME_FORMATTER, Pins, Colors, DiscSizes);
        if (false == AssetDatabase.IsValidFolder(dataPath + "/" + dir))
            AssetDatabase.CreateFolder(dataPath, dir);
        // Debug.Log("Persistent Path:" + dataPath + "/" + dir + string.Format(FILE_NAME_FORMATTER, Pins, Colors, DiscSizes, Moves, currentLevelIndex));
        TotalLevelsGenerated++;
        AssetDatabase.CreateAsset(selectedLevelData, dataPath + "/" + dir + string.Format(FILE_NAME_FORMATTER, Pins, Colors, DiscSizes, Moves, currentLevelIndex,TotalLevelsGenerated));
        // AssetDatabase.CreateFolder(Application.persistentDataPath, "generated");
        //AssetDatabase.CreateAsset(selectedLevelData, Application.persistentDataPath+string.Format( "/generated/{0}Pin{1}Colors{2}Size{3}Moves_{4}",Pins,Colors,DiscSizes,Moves,currentLevelIndex)+".asset");
        //  Debug.Log("exported_levels POST ===" + PlayerPrefs.GetString("exported_levels"));
        IsSelected = true;

        PopupWindow.ShowWindow("Level exported!!!");
    }

    int TotalLevelsGenerated
    {
        get
        {
            return PlayerPrefs.GetInt("totalLevels");
        }

        set
        {
            PlayerPrefs.SetInt("totalLevels", value);
            PlayerPrefs.Save();
        }
    }

    private int _maxLevels = -1;
    public int GetMaxLevels()
    {
        return _maxLevels;
    }
}
[Serializable]
class ExportInfo
{
    public string Id;
    public bool exported;

    public ExportInfo(string id, bool exported)
    {
        Id = id;
        this.exported = exported;
    }

    public static ExportInfo GetInstance(string _id, bool _xported)
    {
        return new ExportInfo(_id, _xported);
    }
}
