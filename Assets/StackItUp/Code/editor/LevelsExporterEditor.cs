using System.Collections.Generic;
using System.IO;
using Types;
using UnityEditor;
using UnityEngine;

public class LevelsExporterEditor : EditorWindow
{
    [MenuItem("Tools/Furious/Level Exporter")]
    static void ShowWindow()
    {
        LevelsExporterEditor window = GetWindow<LevelsExporterEditor>();
        window.Show();
    }

    private void OnGUI()
    {
        InitTexture();
        DrawSections();
        InitInfo();
        if (showInfo)
        {
            isLevelDataInitialized = true;
            ShowInfo();
        }

    }

    private LevelsData dataObject = null;
    private LevelsData.Data selectedLevelData;
    private int selectedLevelIndex = -1;

    private int configCode = 0;
    private void InitInfo()
    {
        GUIStyle style = GUI.skin.box;
        GUILayout.BeginArea(infoConfigSection);//***************************
    
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Select Levels data object");
        dataObject = EditorGUILayout.ObjectField(dataObject, typeof(LevelsData),false) as LevelsData;
        if (dataObject != null)
        {
            EditorGUILayout.LabelField("Total Levels Count :"+dataObject.AllLevels.Count);
        }

        EditorGUILayout.LabelField("Choose level index");
        selectedLevelIndex = EditorGUILayout.IntField(selectedLevelIndex);

        if (GUILayout.Button("Show level info"))
        {
            ShowLevelInfo();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("CONFIG CODE");
        configCode = EditorGUILayout.IntField(configCode);

        if (GUILayout.Button("Batch Export"))
        {
            //return fileInfo[index % fileInfo.Length].Name.Split('.')[0];

            int _currentCode = configCode;
            int pin = _currentCode / 1000;
            _currentCode = _currentCode % 1000;

            int Color = _currentCode / 100;
            _currentCode = _currentCode % 100;

            int size = _currentCode / 10;
            int MaxMove = _currentCode % 10;

            MaxMove = MaxMove == 0 ? 12 : MaxMove;
            //string folderName = dataPath + "/" + dir + string.Format(FILE_NAME_FORMATTER, _pinCount, _uniqueColors, _tileSizes, _selectedLevel + 1)
            for (int i = 0; i < MaxMove; i++)
            {
                dataObject = Resources.Load<LevelsData>(string.Format(dataPath, pin, Color, size) + string.Format(FILE_NAME_FORMATTER, pin, Color, size, i));

                Debug.Log("files" + (string.Format(dataPath, pin, Color, size) + string.Format(FILE_NAME_FORMATTER, pin, Color, size, i)));
                if (dataObject == null)
                    continue;

                for (int m = 0; m < ColorCount; m++)
                {
                    DiscColors[m] = colorMappingList.ColorMaps[m].name;
                }

                for (int j = 0; j < dataObject.AllLevels.Count; j++)
                {
                    selectedLevelIndex = j;
                    ShowLevelInfo();
                    SaveLevelData();
                }

            }

        }
        if (GUILayout.Button(" MOVE FILE"))
        {
            var info = new DirectoryInfo("Assets/StackItUp/Resources/Levels");
            var fileInfo = info.GetFiles();
            for (int i = 0; i < fileInfo.Length; i++)
            {
                fileInfo[i].MoveTo("Assets/StackItUp/Resources/MovedLevels/"+ fileInfo[i].Name);
            }

        }

        EditorGUILayout.EndHorizontal();

        GUILayout.EndArea();//***************************
    }

    private void ShowLevelInfo()
    {
        if (selectedLevelIndex != -1)
        {
            selectedLevelData = dataObject.AllLevels[selectedLevelIndex];
            InitData();
            showInfo = true;
        }
    }
    private bool showInfo = false;
    //============================================================//
    //Level config Section
    Texture2D configTexture;
    Rect levelConfigSection;
    Rect infoConfigSection;
    //Config info data
    public int LevelId;
    public StackData stackData;
    public Types.LevelType Pins;
    public int ColorCount;

    public List<Types.DiscColors> DiscColors = new List<Types.DiscColors>();

    //Pin Details
    Texture2D pinSectionTexture;
    Rect pinSectionRect;

    Rect saveBtnSectionRect;
    Texture2D saveBtnTexture;
    Texture2D exportedLabelTexture;

    [SerializeField]
    Dictionary<string, int> DiscMap;

    GUIStyle configSectionStlye = null;
    public ColorMaterialMap colorMappingList;

    public int MAX_DISC = 4;
    private bool isLevelDataInitialized = false;

    private LevelData levelData;

    List<string> PinMoves;
    List<PinConfig> PinConfigs = new List<PinConfig>();
    private List<int> WinSequence = null;
    private void InitData()
    {
        LevelId = selectedLevelIndex;
        Pins = GetPinEnumFromCount(selectedLevelData.pins);
        ColorCount = selectedLevelData.colors;
        MAX_DISC = selectedLevelData.tileSize;
        PinConfigs = selectedLevelData.PinConfigs;
        colorMappingList = Resources.Load<ColorMaterialMap>("ColorMaterialMapData");
        stackData = Resources.Load<StackData>("Stacks/Stack2");
    }
    private void ShowInfo()
    {

        DrawLevelConfig();
        DrawPinDetails();
    }

    void InitTexture()
    {
        if (configTexture == null)
        {
            configTexture = new Texture2D(1, 1);
            configTexture.SetPixel(0, 0, Color.gray);
            configTexture.Apply();
        }

        if (pinSectionTexture == null)
        {
            pinSectionTexture = new Texture2D(1, 1);
            pinSectionTexture.SetPixel(0, 0, Color.black);
            pinSectionTexture.Apply();
        }


        if (saveBtnTexture == null)
        {
            saveBtnTexture = new Texture2D(1, 1);
            saveBtnTexture.SetPixel(0, 0, new Color(0.3f, 0.55f, 0.15f));
            saveBtnTexture.Apply();
        }

        if (exportedLabelTexture == null)
        {
            exportedLabelTexture = new Texture2D(1, 1);
            exportedLabelTexture.SetPixel(0, 0, new Color(0.8f, 0f, 0f));
            exportedLabelTexture.Apply();
        }

        foreach (var col in DiscColors)
        {
            if (false == StyleColorMap.ContainsKey(col))
            {
                GUIStyle colorstyle = new GUIStyle(GUI.skin.button);
                Texture2D text = new Texture2D(1, 1);
                text.SetPixel(0, 0, GetColorFromMap(col));
                text.Apply();
                colorstyle.normal.background = text;
                colorstyle.normal.textColor = Color.black;
                StyleColorMap.Add(col, colorstyle);
            }

            if (StyleColorMap[col].normal.background == null)
            {
                GUIStyle colorstyle = StyleColorMap[col];
                Texture2D text = new Texture2D(1, 1);
                text.SetPixel(0, 0, GetColorFromMap(col));
                text.Apply();
                colorstyle.normal.background = text;
                StyleColorMap[col] = colorstyle;
            }
        }

    }
    private void DrawSections()
    {
        infoConfigSection = new Rect(0, 0f, Screen.width, 150f);
        levelConfigSection = new Rect(0, 150f, Screen.width, 300f);
        pinSectionRect = new Rect(0, 300f, Screen.width, Screen.height- 450f);
    }

    GUIStyle GetExportedLabelStyle()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 14;
        style.alignment = TextAnchor.LowerCenter;
        style.normal.textColor = Color.black;
        style.normal.background = exportedLabelTexture;
        return style;
    }
    void DrawLevelConfig()
    {
        GUI.DrawTexture(levelConfigSection, configTexture);
        if (configSectionStlye == null)
        {
            configSectionStlye = GUI.skin.box;
            configSectionStlye.normal.background = configTexture;
        }
        GUILayout.BeginArea(levelConfigSection);//***************************
        EditorGUILayout.BeginVertical(configSectionStlye);

                GUILayout.BeginHorizontal();
                if (selectedLevelData.IsExported)
                {
                    GUILayout.Label("EXPORTED : " + selectedLevelData.IsExported.ToString(), GetExportedLabelStyle());
                    GUILayout.Label("FILE NAME : " + selectedLevelData.ExportedFileName, GetExportedLabelStyle());
                }
                else
                {
                    GUILayout.Label("EXPORTED : " + selectedLevelData.IsExported.ToString());
                    GUILayout.Label("FILE NAME : " + selectedLevelData.ExportedFileName);
                }
        GUILayout.EndHorizontal();

                //GUILayout.BeginHorizontal();
                //GUILayout.Label("STACK DATA");
                //stackData = (StackData)EditorGUILayout.ObjectField(stackData, typeof(StackData), false);
                //GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Number of Pins");
                Pins = (LevelType)EditorGUILayout.EnumPopup(Pins);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("COLOR MAPPING LIST");
                colorMappingList = (ColorMaterialMap)EditorGUILayout.ObjectField(colorMappingList, typeof(ColorMaterialMap), false);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("UNIQUE COLOR COUNT");
                ColorCount = (int)EditorGUILayout.IntField(ColorCount);
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("DISC SIZES ");
                MAX_DISC = (int)EditorGUILayout.IntField(MAX_DISC);
                GUILayout.EndHorizontal();

                //if (GUILayout.Button("INIT LEVEL"))
                //{
                //    isLevelDataInitialized = false;
                //    SetUpLevel();
                //}

                //if (GUILayout.Button("GENERATE LEVEL DATA"))
                //{


                //}

                if (isLevelDataInitialized)
                {

                    GUILayout.Label("Unique Disc Colors");
                    GUILayout.BeginHorizontal();
                    //DiscColors = new List<Disc_Colors>(ColorCount);
                    int colorIndex = 0;
                    while (DiscColors.Count < ColorCount)
                    {
                        DiscColors.Add(colorMappingList.ColorMaps[colorIndex++].name);
                    }

                    for (int i = 0; i < ColorCount; i++)
                    {
                        DiscColors[i] = (DiscColors)EditorGUILayout.EnumPopup(DiscColors[i]);
                    }

                    GUILayout.EndHorizontal();

                    InitAllAvailableDiscs();
                  }
                    
                    
                      
                        //GUIStyle style = new GUIStyle(GUI.skin.button);
                        //style.fontSize = 24;
                        //style.normal.textColor = Color.white;
                        //style.normal.background = saveBtnTexture;
                        //if (GUILayout.Button("SAVE LEVEL", style))
                        //{
                        //   // SaveLevelData();
                        //}

                        //style = new GUIStyle(GUI.skin.button);
                        //style.fontSize = 24;
                        //style.normal.textColor = Color.white;
                        //style.normal.background = overrideBtnTexture;
                        //if (GUILayout.Button("OVERRIDE LEVEL", style))
                        //{
                        //   // SaveLevelData(true);
                        //}

                        //GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        GUILayout.EndArea();//***************************

    }

    private void InitAllAvailableDiscs()
    {
        if (DiscMap == null)
            return;
        int pinsCount = GetPinsCount(Pins);
        GUILayout.Label("AVALIABLE DISCS");
        EditorGUILayout.BeginHorizontal();
        string[] PinsPopupArray = GetPinsPopupArray();
        string key = null;

        for (int i = 0; i < ColorCount; i++)
        {
            GUILayout.BeginVertical();
            for (int j = 0; j < MAX_DISC; j++)
            {
                key = DiscColors[i].ToString() + (j + 1);
                if (false == DiscMap.ContainsKey(key))
                    DiscMap.Add(key, 0);
                if (0 == DiscMap[key])
                {
                    PinsPopupArray[0] = key;
                    DiscMap[key] = EditorGUILayout.Popup(DiscMap[key], PinsPopupArray, GUILayout.Width(80 + j * 15));
                    if (DiscMap[key] != 0)
                    {
                        TileInfo tile = new TileInfo(i, j + 1);
                        PinConfig config = GetPinConfig(DiscMap[key] - 1);
                        config.AddTile(tile);
                        // PinConfigs.Add(config);
                        PinMoves.Add(DiscMap[key] + "-" + key);
                    }
                }
            }
            GUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }
    void SetUpLevel()
    {
        if (DiscMap == null)
            DiscMap = new Dictionary<string, int>();
        else
            DiscMap.Clear();

        if (PinMoves == null)
            PinMoves = new List<string>();
        else
            PinMoves.Clear();
        PinConfigs.Clear();
        if (colorMappingList == null)
            colorMappingList = Resources.Load<ColorMaterialMap>("ColorMaterialMapData");
        isLevelDataInitialized = true;
    }
    void ResetLevelData()
    {
        levelData = null;
        LevelId = 0;
        MAX_DISC = 0;
        ColorCount = 0;
        SetUpLevel();
    }

    void DrawPinDetails()
    {
        GUI.DrawTexture(pinSectionRect, pinSectionTexture);
        GUILayout.BeginArea(pinSectionRect);//***************************
        EditorGUILayout.BeginVertical();
        GUIStyle st = new GUIStyle(GUI.skin.label);
        st.fontSize = 16;
        st.normal.textColor = Color.white;
        GUILayout.Label("Pin Setup", st);
        //
        //  GUILayout.BeginArea(pinSectionRect);
        GUILayout.BeginHorizontal();
        for (int i = 0; i < selectedLevelData.pins; i++)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("PIN: " + (i + 1));
            DrawPinState(i);
           
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 24;
        style.normal.textColor = Color.white;
        style.normal.background = saveBtnTexture;
        if (GUILayout.Button("Export Level", style))
        {
            SaveLevelData();
        }

       

        EditorGUILayout.EndVertical();
        GUILayout.EndArea();//***************************
    }
    private string dataPath = "generated/{0}Pin{1}Colors{2}Size";
    string FILE_NAME_FORMATTER = "/{0}Pin{1}Colors{2}Size{3}Moves";
    private void DrawPinState(int index)
    {
      
        for (int i = 0; i < PinConfigs.Count; i++)
        {
            if (PinConfigs[i].pinIndex == index)
            {
                for (int j = PinConfigs[i].tiles.Count - 1; j >= 0; j--)
                {
                    // Debug.Log("PIN " + index + ":: Tiles " + PinConfigs[i].tiles[j].colorIndex + " size " + PinConfigs[i].tiles[j].size);
                    if (GUILayout.Button(DiscColors[PinConfigs[i].tiles[j].colorIndex].ToString() + " " + (PinConfigs[i].tiles[j].size), GetButtonStyle(DiscColors[PinConfigs[i].tiles[j].colorIndex]), GUILayout.Width(80 + PinConfigs[i].tiles[j].size * 15)))
                    {
                        //string key = DiscColors[PinConfigs[i].tiles[j].colorIndex].ToString() + (PinConfigs[i].tiles[j].size);
                        //DiscMap[key] = 0;
                        //PinConfigs[i].RemoveTile(PinConfigs[i].tiles[j]);
                    }
                }
            }

        }
        // GUILayout.EndVertical();
    }
    private PinConfig GetPinConfig(int index)
    {
        for (int i = 0; i < PinConfigs.Count; i++)
            if (PinConfigs[i].pinIndex == index)
                return PinConfigs[i];
        PinConfig config = new PinConfig(index);
        PinConfigs.Add(config);
        return config;
    }

    private int GetPinsCount(LevelType type)
    {
        switch (type)
        {
            case LevelType.TWO_PIN: return 2;
            case LevelType.THREE_PIN: return 3;
            case LevelType.FOUR_PIN: return 4;
            case LevelType.FIVE_PIN: return 5;
            case LevelType.SIX_PIN: return 6;
            case LevelType.SEVEN_PIN: return 7;

            default: return 3;
        }
    }
    private LevelType GetPinEnumFromCount(int pins)
    {
        switch (pins)
        {
            case 2: return LevelType.TWO_PIN;
            case 3: return LevelType.THREE_PIN;
            case 4: return LevelType.FOUR_PIN;
            case 5: return LevelType.FIVE_PIN;
            case 6: return LevelType.SIX_PIN;
            case 7: return LevelType.SEVEN_PIN;

            default: return LevelType.THREE_PIN;
        }
    }

    private string[] GetPinsPopupArray()
    {
        string[] names = new string[GetPinsCount(Pins) + 1];
        for (int i = 1; i < names.Length; i++)
        {
            names[i] = "PIN " + i;

        }
        names[0] = "NONE";
        return names;
    }

    private static Dictionary<DiscColors, GUIStyle> StyleColorMap = new Dictionary<DiscColors, GUIStyle>();
    private GUIStyle GetButtonStyle(DiscColors color)
    {
        //  Debug.Log("StyleColorMap.ContainsKey(color) "+color + (StyleColorMap.ContainsKey(color)));
        if (false == StyleColorMap.ContainsKey(color))
        {
            GUIStyle colorstyle = new GUIStyle(GUI.skin.button);
            Texture2D text = new Texture2D(1, 1);
            text.SetPixel(0, 0, GetColorFromMap(color));
            text.Apply();
            colorstyle.normal.background = text;
            colorstyle.normal.textColor = Color.black;
            StyleColorMap.Add(color, colorstyle);
        }
        //  Debug.Log("GetColorFromMap(DiscColors discColor)"+color + " name "+ StyleColorMap[color]);
        return StyleColorMap[color];
    }

    private Color GetColorFromMap(DiscColors discColor)
    {
        foreach (var mat in colorMappingList.ColorMaps)
        {
            if (mat.name == discColor)
                return mat.material.color;
        }
        return Color.black;
    }
    private List<Material> ColorMaterials = null;
    private Material GetMaterialFromColorEnum(DiscColors color)
    {
        foreach (ColorMat cl in colorMappingList.ColorMaps)
        {
            if (cl.name == color)
                return cl.material;
        }
        return null;
    }
    int poolIndex = 0;
    FileInfo[] pooledObjects = null;
    private LevelData GetPooledLevelData(string fileName)
    {
        if(pooledObjects == null)
        {
            var info = new DirectoryInfo("Assets/StackItUp/Resources/Pool");
            pooledObjects = info.GetFiles();

        }

        if (pooledObjects.Length>0)
        {
            var file=  pooledObjects[0];
            //fileInfo[i].MoveTo("Assets/StackItUp/Resources/MovedLevels/" + fileInfo[i].Name);
            Debug.Log("file"+file.FullName);
            file.MoveTo("Assets/StackItUp/Resources/Levels/" + fileName+".asset" );
            levelData = Resources.Load<LevelData>("Levels/" + fileName);
        }
        else
        {
            levelData = (LevelData)ScriptableObject.CreateInstance("LevelData");
            AssetDatabase.CreateAsset(levelData, SavedLevelDataPath + fileName + ".asset");
        }

        return levelData;
    }
    private void SaveLevelData(bool isOverride = false)
    {
        //Debug.Log("SaveLevelData**");
        //VALIDATIONS
        //if (stackData == null)
        //{
        //    PopupWindow.ShowWindow("STACK DATA MISSING");
        //    Debug.LogError("STACK DATA MISSING");
        //    return;
        //}
        //Debug.Log("stackData SaveLevelData**");
        if (ColorCount == 0)
        {
            PopupWindow.ShowWindow("STACK COUNT CAN'T BE ZERO");
            Debug.LogError("STACK COUNT CAN'T BE ZERO");
            return;
        }
        //Debug.Log("MAX_DISC SaveLevelData**");
        if (MAX_DISC == 0)
        {
            PopupWindow.ShowWindow("DISC COUNT CAN'T BE ZERO");
            Debug.LogError("DISC COUNT CAN'T BE ZERO");
            return;
        }
       // Debug.Log("DiscColors SaveLevelData**");
        if (DiscColors.Count == 0)
        {
            PopupWindow.ShowWindow("COLORS COUNT CAN'T BE ZERO");
            Debug.LogError("COLORS COUNT CAN'T BE ZERO");
            return;
        }
        //Debug.Log("DiscColors loop SaveLevelData**"+ ColorCount+ MAX_DISC);
        //for (int i = 0; i < ColorCount; i++)
        //{
        //    for (int j = 0; j < MAX_DISC; j++)
        //    {
        //        string key = DiscColors[i].ToString() + (j + 1);
        //        if (0 == DiscMap[key])
        //        {
        //            PopupWindow.ShowWindow("THERE ARE ONE OR MORE DISC UNASSIGNED TO A PIN");
        //            Debug.LogError("THERE ARE ONE OR MORE DISC UNASSIGNED TO A PIN");
        //            return;
        //        }
        //    }
        //}
       // Debug.Log("PinConfigs SaveLevelData**");
        if (PinConfigs.Count == 0)
        {
            PopupWindow.ShowWindow("ARRANGE PINS FOR THE LEVEL");
            Debug.LogError("ARRANGE PINS FOR THE LEVEL");
            return;
        }
        levelData = GetLevelFromResources();
       // Debug.Log("level data"+(levelData == null));
        string fileName = dataObject.name + "_" + selectedLevelIndex;
        if (isOverride == false && null != levelData)
        {
            PopupWindow.ShowWindow(string.Format("LEVEL {0} DATA ALREADY EXIST!!! USE OVERRIDE BUTTON UPDATE THE LEVEL", LevelId));
            return;
        }
        else if (isOverride || levelData == null)
        {

            levelData = GetPooledLevelData(fileName);// (LevelData)ScriptableObject.CreateInstance("LevelData");
           // AssetDatabase.CreateAsset(levelData, SavedLevelDataPath + fileName + ".asset");
        }
       // Debug.Log("string level data" + (levelData.ToString()) + " path "+ SavedLevelDataPath + dataObject.name + "_" + selectedLevelIndex + ".asset");
        //levelData.stack = stackData;

        levelData.stackCount = ColorCount;

        if (WinSequence == null)
            WinSequence = new List<int>();
        WinSequence.Clear();
        for (int i = 0; i < MAX_DISC; i++)
            WinSequence.Add(i + 1);

        levelData.sequence = WinSequence;

        if (ColorMaterials == null)
            ColorMaterials = new List<Material>();
        ColorMaterials.Clear();
        for (int i = 0; i < DiscColors.Count; i++)
            ColorMaterials.Add(GetMaterialFromColorEnum(DiscColors[i]));

        levelData.colors = ColorMaterials;

        levelData.pins = GetPinsCount(Pins);

        levelData.pinConfig = PinConfigs;

        selectedLevelData.IsExported = true;
        selectedLevelData.ExportedFileName = fileName;
        dataObject.AllLevels[selectedLevelIndex] = selectedLevelData;
        PopupWindow.ShowWindow(isOverride ? " LEVEL DATA OVERRIDEN!!! " : " NEW LEVEL CREATED!!!");
    }
    private LevelData GetLevelFromResources()
    {
        return Resources.Load<LevelData>("Levels/" + selectedLevelData.ExportedFileName);
    }

    private string SavedLevelDataPath = "Assets/StackItUp/Resources/Levels/";
}
