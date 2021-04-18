using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Types;
using UnityEditor;
using UnityEngine;

public class Generator : EditorWindow
{
    [MenuItem("Tools/Furious/Level Generator")]
    static void ShowWindow()
    {
        Generator window = GetWindow<Generator>();
        window.Show();
    }

    private int _pinCount = 3;
    private int _uniqueColors = 3;
    private DiscColors[] _colors = new DiscColors[0];
    private int _tileSizes = 4;
    private int MAX_TILE_LIMIT = 0;
    int MAX_LEVELS_PER_MOVE = 500;
    int MAX_TILE_SIZE_DIFF = 3;
    private void OnGUI()
    {
        InitInfo();
    }
    int selectedIndex = 0;
    string DIRECTORY_NAME_FORMATTER = "{0}Pin{1}Colors{2}Size";
    string FILE_NAME_FORMATTER = "/{0}Pin{1}Colors{2}Size{3}Moves.asset";

    public ColorMaterialMap colorMappingList;

    private void ResetGenerator()
    {
        loopCount = 0;
        generatedLevels.Clear();
        _defaultPinSetup.Clear();
        LevelMap.Clear();
        count = 0;
    }
    private void InitInfo()
    {
        GUIStyle style = GUI.skin.box;
        EditorGUILayout.BeginVertical(style);
        if (GUILayout.Button("Reset"))
        {
            ResetGenerator();
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("PINS");
        _pinCount = EditorGUILayout.IntField(_pinCount);
        EditorGUILayout.EndHorizontal(); 
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("UNIQUE COLORS");
        _uniqueColors = EditorGUILayout.IntField(_uniqueColors);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("TILE SIZES");
        _tileSizes = EditorGUILayout.IntField(_tileSizes);
        MAX_TILE_LIMIT = _tileSizes + 2;
        EditorGUILayout.EndHorizontal();
        colorMappingList = Resources.Load<ColorMaterialMap>("ColorMaterialMapData");
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("CHOOSE COLORS");
        EditorGUILayout.BeginHorizontal();
        if(_colors.Length < _uniqueColors)
        {
          _colors = new DiscColors[_uniqueColors];
            for (int i = 0; i < _uniqueColors; i++)
                _colors[i] = colorMappingList.ColorMaps[i].name;
        }
        for (int i =0;i<_uniqueColors;i++)
            _colors[i] = (Types.DiscColors)EditorGUILayout.EnumPopup(_colors[i]);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

      

        if(GUILayout.Button("Init Default Tiles"))
        {
            InitDefaultStates();
        }

        EditorGUILayout.LabelField("Select MAX Moves");
        EditorGUILayout.BeginHorizontal();
        //if (GUILayout.Button("1 MOVE"))
        //{
        //    GenerateLevel(0,_defaultPinSetup,new List<Moves>());
        //    Debug.Log("LEVEL COUNT"+generatedLevels.Count);
        //}
        //if (GUILayout.Button("2 MOVE"))
        //{
        //    int max = generatedLevels.Count;
        //    for (int i =0;i< max; i++)
        //        GenerateLevel(1, generatedLevels[i].PinConfig, generatedLevels[i].AppliedMoves);
        //    Debug.Log("LEVEL COUNT" + generatedLevels.Count);
        //}
        //if (GUILayout.Button("3 MOVE"))
        //{
        //    int max = generatedLevels.Count;
        //    int i = max-1;
        //    while(generatedLevels[i].AppliedMoves.Count == 2)
        //    {
        //        GenerateLevel(2, generatedLevels[i].PinConfig, generatedLevels[i].AppliedMoves);
        //        i--;
        //    }
        //    Debug.Log("LEVEL COUNT" + generatedLevels.Count);
        //}

        //if (GUILayout.Button("4 MOVE"))
        //{
        //    int max = generatedLevels.Count;
        //    int i = max - 1;
        //    while (generatedLevels[i].AppliedMoves.Count == 3)
        //    {
        //        GenerateLevel(3, generatedLevels[i].PinConfig, generatedLevels[i].AppliedMoves);
        //        i--;
        //    }
        //    Debug.Log("LEVEL COUNT" + generatedLevels.Count);
        //}
        int MoveCount = 12;

        string[] moveIndices = new string[MoveCount];
        for(int i = 0; i < MoveCount;i++)
        {
            moveIndices[i]= (i + 1) + " MOVE";
        }

        selectedIndex = EditorGUILayout.Popup("MOVE INDEX", selectedIndex, moveIndices);
        if (GUILayout.Button("GENERATE LEVEL"))
        {
            GenerateButtonTapped(selectedIndex);
        }
      
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
  
    private void GenerateButtonTapped(int maxMoves)
    {

        ResetGenerator();
        InitDefaultStates();
        for (int _selectedLevel = 0; _selectedLevel <= maxMoves; _selectedLevel++)
        {
            currentMoveIndexLevels = new List<GeneratedLevel>();
            if (generatedLevels.Count == 0)
                GenerateLevel(0, _defaultPinSetup, new List<Moves>(), InitDefaultStates());
            else
            {
                //int max = generatedLevels.Count;
                //int i = max - 1;
                //while (i>=0 && generatedLevels[i].AppliedMoves.Count == selectedIndex)
                //{
                //    GenerateLevel(selectedIndex, generatedLevels[i].PinConfig, generatedLevels[i].AppliedMoves);
                //    i--;
                //}

                int max = 0; //generatedLevels.Count;
                foreach (var e in generatedLevels)
                {
                    if (e.AppliedMoves.Count == _selectedLevel)
                        max++;
                }
                int skipIndex = 1;
                if (max > MAX_LEVELS_PER_MOVE)
                {
                    skipIndex = (max / MAX_LEVELS_PER_MOVE) + 1;
                }
                Debug.LogError("skip" + skipIndex);
                int i = generatedLevels.Count - 1;
                while (i >= 0 && generatedLevels[i].AppliedMoves.Count == _selectedLevel)
                {
                    GenerateLevel(_selectedLevel, generatedLevels[i].PinConfigs, generatedLevels[i].AppliedMoves, generatedLevels[i]);
                    i = i - skipIndex;
                }

            }
            //for(int k = 0;k< currentMoveIndexLevels.Count; k = k + 10)
            if (currentMoveIndexLevels.Count > 0)
            {
                LevelsData data = new LevelsData();
                data.AllLevels = new List<LevelsData.Data>();
                int limit = currentMoveIndexLevels.Count; //(k + 10 < currentMoveIndexLevels.Count ? k + 10 : currentMoveIndexLevels.Count);
                for (int i = 0; i < limit; i++)
                {
                    data.AllLevels.Add(new LevelsData.Data(currentMoveIndexLevels[i].PinConfigs, currentMoveIndexLevels[i].AppliedMoves, currentMoveIndexLevels[i].uniqueColors, currentMoveIndexLevels[i].pins, currentMoveIndexLevels[i].tileSizes));
                }
                string dir = string.Format(DIRECTORY_NAME_FORMATTER, _pinCount, _uniqueColors, _tileSizes);
                if (false == AssetDatabase.IsValidFolder(dataPath + "/" + dir))
                    AssetDatabase.CreateFolder(dataPath, dir);
                AssetDatabase.CreateAsset(data, dataPath + "/" + dir + string.Format(FILE_NAME_FORMATTER, _pinCount, _uniqueColors, _tileSizes, _selectedLevel + 1));
                Debug.Log("LEVEL COUNT" + generatedLevels.Count + "selectedIndex" + _selectedLevel + "data :" + data.AllLevels.Count);

            }
        }

    }
    List<GeneratedLevel> generatedLevels = new List<GeneratedLevel>();
    private List<GeneratedLevel> currentMoveIndexLevels = null;
    private static int loopCount = 0;
    private void GenerateLevel(int index, List<PinConfig> currentPinSetup,List<Moves> previousMoves, GeneratedLevel generated)
    {
        List<PinConfig> _pinSetup = null; List<Moves> _moves = null;

        Moves move ;
        int maxPinCount = Mathf.Min(index+2, _pinCount);

        for (int i = 0; i < _pinCount; i++)//Move from
        {
            for(int j = 0; j < _pinCount; j++)//Move to
            {
                if (i == j)
                    continue;
                if (currentPinSetup[i].tiles.Count == 0 || currentPinSetup[j].tiles.Count == MAX_TILE_LIMIT)
                    continue;
                if (currentPinSetup[j].tiles.Count > 0 && currentPinSetup[i].tiles[currentPinSetup[i].tiles.Count - 1].size - currentPinSetup[j].tiles[currentPinSetup[j].tiles.Count - 1].size >= MAX_TILE_SIZE_DIFF)
                    continue;
                //_pinSetup = currentPinSetup; //GetDuplicatedSetup(currentPinSetup);
                //_pinSetup = new List<PinConfig>();
                //_pinSetup.AddRange(currentPinSetup.ToArray());
                //_moves = new List<Moves>();
                //_moves.AddRange(previousMoves.ToArray());
                _pinSetup = generated.PinConfigs;
                move = new Moves(i,j, new TileInfo(_pinSetup[i].tiles[_pinSetup[i].tiles.Count - 1].colorIndex, _pinSetup[i].tiles[_pinSetup[i].tiles.Count - 1].size));
                //if (ReverseMoveExist(move, previousMoves))
                //{
                //    Debug.Log("DUPLICATE MOVE FOUND***");
                //    continue;
                //}

                // SerializeLevel(move, currentPinSetup, previousMoves);
                //_pinSetup[j].Add(move.tile);
                // _pinSetup[i].RemoveAt(_pinSetup[i].Count - 1);
                // _moves.Add(move);
              //  GeneratedLevel lv = generated; //new GeneratedLevel();

               // for(int k = 0; k < 20; k++)
                //{
                    GeneratedLevel lv = ScriptableObject.Instantiate(generated);
                    GeneratedLevel lvl = SerializeLevel(pins: _pinCount, uniqueColors: _uniqueColors, tileSizes: _tileSizes, move, _pinSetup, _moves, lv);
                 
                   
              //  }
                //GeneratedLevel lv = ScriptableObject.Instantiate(generated);
                //GeneratedLevel lvl = SerializeLevel(pins: _pinCount, uniqueColors: _uniqueColors, tileSizes: _tileSizes, move,_pinSetup, _moves ,lv);

                string key = lvl.GetHashKey();
                if (i < maxPinCount && j< maxPinCount&& false == LevelMap.Contains(key))
                {
                    LevelMap.Add(key);
                    generatedLevels.Add(lvl);
                    currentMoveIndexLevels.Add(lvl);
                }
                else if(false == LevelMap.Contains(key))
                {
                    LevelMap.Add(key);
                    Debug.Log("Adding dummy maps!!!");
                }

                else
                {
                    Debug.Log("DuplicateFound!!!");
                }
               
            }
        }


//        Debug.Log("generatedLevels "+ generatedLevels.Count);
        if(currentMoveIndexLevels.Count == 0)
        {
            PopupWindow.ShowWindow("NO NEW LEVEL TO BE GENERATED!!!!");
            Debug.LogError("NO NEW LEVEL TO BE GENERATED!!!!");
        }
        //LogGeneratedLevels();
    }
    HashSet<string> LevelMap = new HashSet<string>();
    private bool ReverseMoveExist(Moves _move, List<Moves> list)
    {
        bool found = false;
        int k = list.Count - 1;
        for(k = list.Count -1;k>= 0; k--)
        {
            if(list[k].from == _move.to && list[k].to == _move.from && list[k].tile.colorIndex == _move.tile.colorIndex &&  list[k].tile.size == _move.tile.size)
            {
                found = true;
                break;
            }
        }

        for(int i = k;i< list.Count && found; i++)
        {
            if (list[i].to == _move.to)
                return false;
        }
        return found;
    }
    static int count = 0;
    private string dataPath = "Assets/StackItUp/Code/editor/resources/generated";
    private GeneratedLevel SerializeLevel(int pins, int uniqueColors, int tileSizes, Moves move, List<PinConfig> _pinSetup, List<Moves> previousMoves, GeneratedLevel baseObj )
    {
        //Debug.Log("previous moves "+string.Join(",", previousMoves));
       // GeneratedLevel obj = (GeneratedLevel)ScriptableObject.CreateInstance("GeneratedLevel");
        //obj.pins = pins;
        //obj.uniqueColors = uniqueColors;
        //obj.tileSizes = tileSizes;
        //obj.AppliedMoves = previousMoves;
        //obj.PinConfigs = _pinSetup;
      //  obj = baseObj;
        //obj.SetPinConfig(previousMoves, _pinSetup);
        // obj.PinConfigs = _pinSetup.Select(item => (PinConfig)item.Clone()).ToList();
        // obj.AppliedMoves = previousMoves;
        //if(count == 0)
        {
          //  AssetDatabase.CreateAsset(baseObj, dataPath + "levenName" + count + ".asset");
        }
       

        // var obj1 = Resources.Load<GeneratedLevel>("generated/levenName"+count);
        baseObj.AppliedMoves.Add(move);
        baseObj.PinConfigs[move.to].tiles.Add(move.tile);
        baseObj.PinConfigs[move.from].tiles.RemoveAt(_pinSetup[move.from].tiles.Count - 1);
        baseObj.pins = pins;
        baseObj.uniqueColors = uniqueColors;
        baseObj.tileSizes = tileSizes;
        count++;
       // Debug.Log("is EQUAL "+(obj == obj1));
        return baseObj;
    }
    private void LogGeneratedLevels()
    {
        Debug.Log("TOTAL LEVELS "+ generatedLevels.Count);
        foreach(var lv in generatedLevels)
        {
            Debug.Log(lv.ToString() + string.Join(".",lv.PinConfigs));
        }
    }
   List<PinConfig> _defaultPinSetup = null;
    private GeneratedLevel InitDefaultStates()
    {
        GeneratedLevel obj = (GeneratedLevel)ScriptableObject.CreateInstance("GeneratedLevel");
        _defaultPinSetup = new List<PinConfig>();
        for (int i = 0; i < _pinCount; i++)
        {
            _defaultPinSetup.Add(new PinConfig(i));
           
        }
        for (int i = 0; i < _uniqueColors; i++)
            for (int j = _tileSizes - 1; j >= 0; j--)
            {
                _defaultPinSetup[i].tiles.Add(new TileInfo(i, j+1));
            }
        obj.PinConfigs = _defaultPinSetup;
        for (int i = 0; i < _pinCount; i++)
        {
            //for (int j = 0; j < _defaultPinSetup[i].Count; j++)
            {
                Debug.Log(" PIN " +(i+1) +string.Join(",", _defaultPinSetup[i].tiles));
            }
        }
        obj.pins = _pinCount;
        obj.uniqueColors = _uniqueColors;
        obj.tileSizes = _tileSizes;
        string key = obj.GetHashKey();
        if (false == LevelMap.Contains(key))
            LevelMap.Add(key);
        return obj;
    }

    private List<List<TileInfo>> GetDuplicatedSetup(List<List<TileInfo>> copy)
    {
        List<List<TileInfo>> newSetup = new List<List<TileInfo>>();

        for(int i =0;i< copy.Count; i++)
        {
            newSetup.Add(new List<TileInfo>());
            for (int j = 0;j< copy[i].Count; j++)
            {
                newSetup[i].Add(new TileInfo(copy[i][j].colorIndex, copy[i][j].size));
            }
        }
        return newSetup;
    }

   
   
}
