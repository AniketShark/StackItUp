using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsData : ScriptableObject
{
    [Serializable]
    public struct Data
    {
        public int colors;
        public int pins;
        public int tileSize;
        public List<Moves> AppliedMoves;
        public List<PinConfig> PinConfigs;
        public bool IsExported ;
        public string ExportedFileName;
        public Data(List<PinConfig> pinConfigs, List<Moves> appliedMoves, int colors, int pins,int tileSize)
        {
            AppliedMoves = appliedMoves;
            PinConfigs = pinConfigs;
            this.colors = colors;
            this.pins = pins;
            this.tileSize = tileSize;
            this.IsExported = false;
            this.ExportedFileName = string.Empty;
        }

    }
    [SerializeField]
    public List<Data> AllLevels;


}
