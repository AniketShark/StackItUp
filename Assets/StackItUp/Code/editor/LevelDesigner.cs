using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Types;
using System;

public class LevelDesigner : EditorWindow
{

    [MenuItem("Tools/LevelDesigner")]
    static void OpenDesigner()
    {
        LevelDesigner window = (LevelDesigner)GetWindow(typeof(LevelDesigner));
        window.minSize = new Vector2(250f, 400f);
        window.Show();
    }

    //Level config Section
    Texture2D configTexture;
    Rect levelConfigSection;
    //Config info data
    public int LevelId;
    public StackData stackData;
    public Types.LevelType Pins;
    public int ColorCount;

    public List<Types.DiscColors> DiscColors = new List<DiscColors>();

    //Pin Details
    Texture2D pinSectionTexture;
    Rect pinSectionRect;

    Rect saveBtnSectionRect;
    Texture2D saveBtnTexture;
    Texture2D overrideBtnTexture;

    [SerializeField]
    Dictionary<string, int> DiscMap;
    public int MAX_DISC = 4;
    private bool isLevelDataInitialized = false;

    private LevelData levelData;
    private string dataPath = "Assets/StackItUp/Resources/Levels/";
    GUIStyle configSectionStlye = null;

    private void OnEnable()
    {
       
    }
    

    public ColorMaterialMap colorMappingList;
    void InitTexture()
    {
        if(configTexture == null)
        {
            configTexture = new Texture2D(1, 1);
            configTexture.SetPixel(0, 0, Color.gray);
            configTexture.Apply();
        }

        if (pinSectionTexture == null)
        {
            pinSectionTexture = new Texture2D(1, 1);
            pinSectionTexture.SetPixel(0, 0, Color.white);
            pinSectionTexture.Apply();
        }


        if (saveBtnTexture == null)
        {
            saveBtnTexture = new Texture2D(1, 1);
            saveBtnTexture.SetPixel(0, 0, new Color(0.3f, 0.55f, 0.15f));
            saveBtnTexture.Apply();
        }

        if (overrideBtnTexture == null)
        {
            overrideBtnTexture = new Texture2D(1, 1);
            overrideBtnTexture.SetPixel(0, 0, new Color(0.3f, 0.55f, 0.55f));
            overrideBtnTexture.Apply();
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
    private void OnGUI()
    {
        InitTexture();
        DrawSections();
        DrawLevelConfig();
        DrawPinDetails();
    }
    private void DrawSections()
    {
        levelConfigSection = new Rect(0, 0, Screen.width, 350f);
        saveBtnSectionRect = new Rect(0, 400f, Screen.width, 450f);
        pinSectionRect = new Rect(0, 450f, Screen.width, Screen.height - 450f);
    }
  

    void DrawLevelConfig()
    {
        GUI.DrawTexture(levelConfigSection, configTexture);
        if(configSectionStlye == null)
        {
            configSectionStlye = GUI.skin.box;
            configSectionStlye.normal.background = configTexture;
        }
       
        EditorGUILayout.BeginVertical(configSectionStlye);
       // GUILayout.BeginArea(levelConfigSection);

        GUILayout.BeginHorizontal();
        GUILayout.Label("LEVEL ID");
        LevelId = (int)EditorGUILayout.IntField(LevelId);
        if(GUILayout.Button("IMPORT LEVEL"))
        {
            ImportLevelData();
        }

        if (GUILayout.Button("RESET LEVEL"))
        {
            ResetLevelData();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("STACK DATA");
        stackData = (StackData)EditorGUILayout.ObjectField(stackData, typeof(StackData), false);
        GUILayout.EndHorizontal();

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

        if (GUILayout.Button("INIT LEVEL"))
        {
            isLevelDataInitialized = false;
            SetUpLevel();
        }

        if (GUILayout.Button("GENERATE LEVEL DATA"))
        {
           

        }

        if (isLevelDataInitialized)
        {

            GUILayout.Label("Unique Disc Colors");
            GUILayout.BeginHorizontal();
            //DiscColors = new List<Disc_Colors>(ColorCount);
            while (DiscColors.Count < ColorCount)
            {
                DiscColors.Add(Types.DiscColors.BLUE);
            }

            for (int i = 0; i < ColorCount; i++)
            {
                DiscColors[i] = (DiscColors)EditorGUILayout.EnumPopup(DiscColors[i]);
            }

            GUILayout.EndHorizontal();

            InitAllAvailableDiscs();
        }

        GUILayout.EndArea();
        GUILayout.BeginArea(saveBtnSectionRect);
        if (GUILayout.Button("CHECK FOR DUPLICATE"))
        {
            if (IsLevelDuplicated())
            {
                PopupWindow.ShowWindow("ERROR!!! LEVEL IS DUPLICATED");
                return;
            }
            PopupWindow.ShowWindow("NO LEVEL WITH DATA FOUND!!!");
        }
        GUILayout.BeginHorizontal();
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 24;
        style.normal.textColor = Color.white;
        style.normal.background = saveBtnTexture;
        if(GUILayout.Button("SAVE LEVEL", style))
        {
            SaveLevelData();
        }

        style = new GUIStyle(GUI.skin.button);
        style.fontSize = 24;
        style.normal.textColor = Color.white;
        style.normal.background = overrideBtnTexture;
        if (GUILayout.Button("OVERRIDE LEVEL", style))
        {
            SaveLevelData(true);
        }

        GUILayout.EndHorizontal();
       // GUILayout.EndArea();
        EditorGUILayout.EndVertical();
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
                        TileInfo tile = new TileInfo(i,j+1);
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
        if(colorMappingList == null)
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
        if (isLevelDataInitialized == false)
            return;
        GUI.DrawTexture(pinSectionRect, pinSectionTexture);
        EditorGUILayout.BeginVertical();
        //
      //  GUILayout.BeginArea(pinSectionRect);
        GUILayout.BeginHorizontal();
        for (int i = 0; i < GetPinsCount(Pins); i++)
        {
            GUILayout.BeginVertical();
            DrawPinState(i);
            GUILayout.Label("PIN: " + (i + 1));
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
      
        EditorGUILayout.EndVertical();
    }
    List<string> PinMoves;
    List<PinConfig> PinConfigs = new List<PinConfig>();
    private void DrawPinState(int index)
    {
        //  GUILayout.BeginVertical();
        for (int i = 0; i < PinConfigs.Count; i++)
        {
            if (PinConfigs[i].pinIndex == index)
            {
                for (int j = PinConfigs[i].tiles.Count - 1; j >= 0; j--)
                {
                    // Debug.Log("PIN " + index + ":: Tiles " + PinConfigs[i].tiles[j].colorIndex + " size " + PinConfigs[i].tiles[j].size);
                    if (GUILayout.Button(DiscColors[PinConfigs[i].tiles[j].colorIndex].ToString() + " " + (PinConfigs[i].tiles[j].size), GetButtonStyle(DiscColors[PinConfigs[i].tiles[j].colorIndex]), GUILayout.Width(80 + PinConfigs[i].tiles[j].size * 15)))
                    {
                        string key = DiscColors[PinConfigs[i].tiles[j].colorIndex].ToString() + (PinConfigs[i].tiles[j].size);
                        DiscMap[key] = 0;
                        PinConfigs[i].RemoveTile(PinConfigs[i].tiles[j]);
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

    private LevelData GetLevelFromResources()
    {
        return Resources.Load<LevelData>("Levels/" + LevelId);
    }
    private void ImportLevelData()
    {
      
        levelData = GetLevelFromResources(); //Resources.Load<LevelData>("Levels/" + LevelId);
        if(levelData == null)
        {
            PopupWindow.ShowWindow("LEVEL NOT FOUND!!!");
            return;
        }
        SetUpLevel();
        //stackData = levelData.stack;
        WinSequence = levelData.sequence;
        MAX_DISC = levelData.sequence.Count;
        ColorCount = levelData.stackCount;
        ColorMaterials = levelData.colors;
        UpdateDiscColors();
        Pins = GetPinEnumFromCount(levelData.pins);
        PinConfigs.Clear();
        PinConfigs.AddRange(levelData.pinConfig);

        if (DiscMap == null)
            DiscMap = new Dictionary<string, int>();
        else
            DiscMap.Clear();
        string key;//= DiscColors[PinConfigs[i].tiles[j].colorIndex].ToString() + (PinConfigs[i].tiles[j].size);
        for (int i = 0; i < PinConfigs.Count; i++)
        {
            for(int j = 0; j < PinConfigs[i].tiles.Count; j++)
            {
                key = DiscColors[PinConfigs[i].tiles[j].colorIndex].ToString() + (PinConfigs[i].tiles[j].size);
               // DiscMap.Add(key, 0);
                DiscMap[key] = PinConfigs[i].pinIndex+1;
            }
        }

       //isLevelDataInitialized = false;
    }

    private void UpdateDiscColors()
    {
        if (DiscColors == null)
            DiscColors = new List<DiscColors>();
        DiscColors.Clear();

        foreach (Material mat in ColorMaterials)
        {
            DiscColors.Add(GetDiscColorFromMat(mat));
        }
    }

    private DiscColors GetDiscColorFromMat(Material mat)
    {
        foreach (ColorMat cm in colorMappingList.ColorMaps)
        {
            if (cm.material == mat)
                return cm.name;
        }
        return Types.DiscColors.BLUE;
    }

    private void SaveLevelData( bool isOverride = false)
    {
        //VALIDATIONS
        if (stackData == null)
        {
            PopupWindow.ShowWindow("STACK DATA MISSING");
            Debug.LogError("STACK DATA MISSING");
            return;
        }
        if (ColorCount == 0)
        {
            PopupWindow.ShowWindow("STACK COUNT CAN'T BE ZERO");
            Debug.LogError("STACK COUNT CAN'T BE ZERO");
            return;
        }

        if (MAX_DISC == 0)
        {
            PopupWindow.ShowWindow("DISC COUNT CAN'T BE ZERO");
            Debug.LogError("DISC COUNT CAN'T BE ZERO");
            return;
        }

        if (DiscColors.Count == 0)
        {
            PopupWindow.ShowWindow("COLORS COUNT CAN'T BE ZERO");
            Debug.LogError("COLORS COUNT CAN'T BE ZERO");
            return;
        }
        for (int i = 0; i < ColorCount; i++)
        {
            for (int j = 0; j < MAX_DISC; j++)
            {
                string key = DiscColors[i].ToString() + (j + 1);
                if (0 == DiscMap[key])
                {
                    PopupWindow.ShowWindow("THERE ARE ONE OR MORE DISC UNASSIGNED TO A PIN");
                    Debug.LogError("THERE ARE ONE OR MORE DISC UNASSIGNED TO A PIN");
                    return;
                }
            }
        }

        if (PinConfigs.Count == 0)
        {
            PopupWindow.ShowWindow("ARRANGE PINS FOR THE LEVEL");
            Debug.LogError("ARRANGE PINS FOR THE LEVEL");
            return;
        }
        levelData = GetLevelFromResources();
        if (isOverride == false && null != levelData)
        {
            PopupWindow.ShowWindow(string.Format( "LEVEL {0} DATA ALREADY EXIST!!! USE OVERRIDE BUTTON UPDATE THE LEVEL",LevelId));
            return;
        }else if (isOverride || levelData == null)
        {
            levelData = (LevelData)ScriptableObject.CreateInstance("LevelData");
            AssetDatabase.CreateAsset(levelData, dataPath + LevelId + ".asset");
        }

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
        if (IsLevelDuplicated())
        {
            PopupWindow.ShowWindow("LEVEL IS DUPLICATED");
            return;
        }

        PopupWindow.ShowWindow(isOverride ? " LEVEL DATA OVERRIDEN!!! ":" NEW LEVEL CREATED!!!");
    }


    private List<int> WinSequence = null;
    [SerializeField]
    private static Dictionary<DiscColors, GUIStyle> StyleColorMap = new Dictionary<DiscColors, GUIStyle>();
    private GUIStyle GetButtonStyle(DiscColors color)
    {
      //  Debug.Log("StyleColorMap.ContainsKey(color) "+color + (StyleColorMap.ContainsKey(color)));
        if(false == StyleColorMap.ContainsKey(color))
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

    private bool IsLevelDuplicated()
    {
        string curr = levelData.GetHashKey();
        LevelData[] arr = Resources.LoadAll<LevelData>("Levels/");
       
        for (int i = 0; i < arr.Length; i++)
        {
            if (string.Equals(curr, arr[i].GetHashKey()))
                return true;
        }
        return false;
    }
}

public class PopupWindow : EditorWindow
{
    private static string _message;
    public static void ShowWindow(string msg)
    {
        EditorWindow window = GetWindow(typeof(PopupWindow));
        window.maxSize = new Vector2(450, 180);
        window.Show();
        _message = msg;
        Callback = null;
    }
    private static Action<bool> Callback;
    public static void ShowWindow(string msg, Action<bool> callback)
    {
        EditorWindow window = GetWindow(typeof(PopupWindow));
        window.maxSize = new Vector2(450, 180);
        window.Show();
        _message = msg;
        Callback = callback;

    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        GUIStyle st = new GUIStyle(GUI.skin.label);
        st.fontSize = 16;
        st.normal.textColor = Color.white;
        GUILayout.Label(_message, st);

        GUIStyle colorstyle = new GUIStyle(GUI.skin.button);
        Texture2D text = new Texture2D(1, 1);
        text.SetPixel(0, 0, Color.green);
        text.Apply();
        colorstyle.normal.background = text;
        colorstyle.normal.textColor = Color.black;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK", colorstyle,GUILayout.Width(100f)))
        {
            this.Close();
            if (Callback != null)
                Callback.Invoke(true);
        }

        if (GUILayout.Button("Cancel", colorstyle, GUILayout.Width(100f)))
        {
            this.Close();
            if (Callback != null)
                Callback.Invoke(false);
        }

        GUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }



}
