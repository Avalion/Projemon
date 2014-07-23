using UnityEngine;
using System.Collections.Generic;
using System.IO;

/**
 * This class defines a map and implements functions to display it.
 */
public class Map {
    public const string IMAGE_FOLDER = "Maps";

    public const int MAP_SCREEN_X = 32;
    public const int MAP_SCREEN_Y = 18;

    public static Vector2 Resolution { get { return new Vector2(Screen.width / (float)MAP_SCREEN_X, Screen.height / (float)MAP_SCREEN_Y); } } 

    public int ID;

    public string name;
    public Vector2 size;

    public List<Tile> tiles = new List<Tile>();
    public class Tile {
        // CONST
        public const int TILE_RESOLUTION = 32;

        // Display
        public Vector2 mapCoords;

        public int layer;

        // IMAGE
        public string originTile;
        public Vector2 originTileCoords;

        private Texture2D image = null;
        public Texture2D Image { 
            get {
                if (image == null)
                    LoadTexture();
                return image;
            }
        }
        
        public void LoadTexture() {
            Texture2D _origin = InterfaceUtility.GetTexture(Config.GetResourcePath(Map.IMAGE_FOLDER) + originTile);
            _origin.name = originTile;

            image = InterfaceUtility.SeparateTexture(_origin, (int)originTileCoords.x, (int)originTileCoords.y, TILE_RESOLUTION, TILE_RESOLUTION);
        }
    }

    public Map(int _id) {
        this.ID = _id;

        size = new Vector2(32, 32);

        Import();

        SortTiles();
    }

    public void Display() {
        foreach (Tile tile in tiles) {
            if (tile.Image != null)
                GUI.DrawTexture(new Rect(Resolution.x * tile.mapCoords.x, Resolution.y * tile.mapCoords.y, Resolution.x, Resolution.y), tile.Image);
        }
    }

    public void SortTiles() {
        tiles.Sort(delegate(Tile x, Tile y) { return x.layer.CompareTo(y.layer); });
    }
    
    public Tile GetTile(int layer, int x, int y) {
        return tiles.Find(T => T.mapCoords.x == x && T.mapCoords.y == y && T.layer == layer);
    }
    public List<Tile> GetTiles(int layer) {
        return tiles.FindAll(T => T.layer == layer);
    }

    public override string ToString() {
        return name;
    }

    // Serialization
    public void Export() {
        string filePath = Config.GetConfigPath("Maps") + ID + ".txt";
        if (tiles.Count == 0) {
            if (File.Exists(filePath))
                File.Delete(filePath);
            return;
        }

        StreamWriter sw = new StreamWriter(filePath);

        sw.WriteLine("------------");
        sw.WriteLine("- Anim " + InterfaceUtility.IntString(ID, 3) + " -");
        sw.WriteLine("------------");
        sw.WriteLine(name);
        sw.WriteLine(size.x + ";" + size.y);
        sw.WriteLine();

        List<Tile> toExport = tiles.FindAll(T => T.mapCoords.x < size.x && T.mapCoords.y < size.y);
        sw.WriteLine(toExport.Count);
        foreach (Tile i in toExport) {
            sw.WriteLine(i.mapCoords.x + "#" + i.mapCoords.y + "#" + i.layer + "#" + i.originTile + "#" + i.originTileCoords.x + "#" + i.originTileCoords.y);
        }
        sw.WriteLine();

        sw.Close();
        sw.Dispose();
    }
    public void Import() {
        string filePath = Config.GetConfigPath("Maps") + ID + ".txt";
        if (!File.Exists(filePath))
            return;
        
        StreamReader sr = new StreamReader(filePath);

        sr.ReadLine();
        sr.ReadLine();
        sr.ReadLine();
        name = sr.ReadLine();
        string line = sr.ReadLine();
        string[] values = line.Split(';');
        size.x = int.Parse(values[0]);
        size.y = int.Parse(values[1]);

        sr.ReadLine();

        tiles = new List<Tile>();
        int count = int.Parse(sr.ReadLine());
        for (int i = 0; i < count; i++) {
            line = sr.ReadLine();

            values = line.Split('#');

            Tile t = new Tile();
            t.mapCoords.x = int.Parse(values[0]);
            t.mapCoords.y = int.Parse(values[1]);
            t.layer = int.Parse(values[2]);
            t.originTile = values[3];
            t.originTileCoords.x = int.Parse(values[4]);
            t.originTileCoords.y = int.Parse(values[5]);

            tiles.Add(t);
        }
        sr.Close();
        sr.Dispose();
    }
}
