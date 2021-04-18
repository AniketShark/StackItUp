using System;

[Serializable]
public class Moves
{
    public int from;
    public int to;
    public TileInfo tile;

    public Moves(int from, int to, TileInfo tile)
    {
        this.from = from;
        this.to = to;
        this.tile = tile;
    }

    public override string ToString()
    {
        return from + " ==> " + to + " tile: " + ((char)(65 + tile.colorIndex)) + tile.size;
    }
}