using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
//[CreateAssetMenu(fileName = "Stack", menuName = "Furious/ScriptableObjects/LevelData", order = 1)]
[CreateAssetMenu(fileName = "GeneratedLevel",menuName = "Furious/ScriptableObjects/Generated Levels", order =0)]
 public class GeneratedLevel : ScriptableObject
    {
        public int pins;
        public int uniqueColors;
        public int tileSizes;
        public List<Moves> AppliedMoves ;
        public List<PinConfig> PinConfigs;
       
    public GeneratedLevel(int pins, int uniqueColors, int tileSizes, List<Moves> appliedMoves, List<PinConfig> pinConfigs)
        {
            this.pins = pins;
            this.uniqueColors = uniqueColors;
            this.tileSizes = tileSizes;
            AppliedMoves = appliedMoves;
            PinConfigs = pinConfigs;
            Debug.Log(this);
        }
        public void SetPinConfig(List<Moves> appliedMoves, List<PinConfig> pinConfigs)
        {
            AppliedMoves = new List<Moves>();
            foreach(var m in appliedMoves)
            {
                AppliedMoves.Add(new Moves(m.from,m.to,m.tile));
            }
            PinConfigs = new List<PinConfig>();
            List<TileInfo> tiles;
            foreach (var m in pinConfigs)
            {
                tiles = new List<TileInfo>();
                foreach (var t in m.tiles) {
                    tiles.Add(new TileInfo(t.colorIndex, t.size));
                }
            
               PinConfigs.Add(new PinConfig(m.pinIndex, tiles));
               
            }
    }
    public override string ToString()
        {
            StringBuilder buff = new StringBuilder();
            for (int j = 0; j < PinConfigs.Count; j++)
            {
                buff.Append(" pin "+j +" : "+ PinConfigs[j].pinIndex);
               // for (int i = 0; i < PinConfigs[j].tiles.Count; i++)
               // {
                    buff.Append(string.Join( ",", PinConfigs[j].tiles));
               // }
                buff.Append("#");
            }
            return "pins : " + pins + " uniqueColors : " + uniqueColors + " tileSizes : " + tileSizes + " MOVES[ " +string.Join(",", AppliedMoves) +"] PinConfig : " + buff;
        }

        public string GetHashKey()
        {
            StringBuilder buff = new StringBuilder();
            buff.Append("p" + pins);
            buff.Append(",c" + uniqueColors);
            buff.Append(",ts" + tileSizes + ",");
            List<string> pin = new List<string>();
            for (int j = 0; j < PinConfigs.Count; j++)
            {
                pin.Add("pin" + ":"+string.Join(",", PinConfigs[j].tiles));
            }
            pin.Sort();
            buff.Append( string.Join("#", pin));
            return buff.ToString();
        }

   
}
