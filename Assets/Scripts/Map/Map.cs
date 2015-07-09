using UnityEngine;
using System.Collections.Generic;
using System.IO;

/**
 * This class defines a map and implements functions to display it.
 * 
 * WARNING ! When you modify this class, you should modify associated functions in SystemDatas !
 */
public class Map {
    public const string IMAGE_FOLDER = "Maps";

    public const int MAP_SCREEN_X = 32;
    public const int MAP_SCREEN_Y = 18;

    public static Vector2 Resolution { get { return new Vector2(Screen.width / (float)MAP_SCREEN_X, Screen.height / (float)MAP_SCREEN_Y); } } 

    public int ID;

    // List of mapObjects on this map to display
    public List<MapObject> mapObjects = new List<MapObject>();

    public string name;
    private Vector2 _size = new Vector2(MAP_SCREEN_X, MAP_SCREEN_Y);
    public Vector2 Size { get { return _size; }}

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

        public int CompareTo(Tile other) {
            if (layer != other.layer)
                return layer.CompareTo(other.layer);
            if (mapCoords.x != other.mapCoords.x)
                return mapCoords.x.CompareTo(other.mapCoords.x);

            return mapCoords.y.CompareTo(other.mapCoords.y);
        }

        public void Dispose() {
            Texture2D.Destroy(image);
            image = null;
        }
    }

    public List<Tile> tiles = new List<Tile>();
    private List<Tile> VisibleTiles = new List<Tile>();
    
    public bool[,] collisions;

    public Map(int _id) {
        this.ID = _id;

        ImportName();
    }

    public void Load() {
        Import();

        SortTiles();

        foreach (DBMapObject mo in GameData.LoadMapObjects(ID))
            mapObjects.Add(MapObject.Generate(mo));

        UpdateVisibleList(Vector2.zero);
    }

    public void Dispose() {
        VisibleTiles.Clear();
        
        foreach (Tile t in tiles) {
            t.Dispose();
        }
        tiles.Clear();
        tiles = null;

        foreach (MapObject obj in mapObjects) {
            obj.Dispose();
        }
        mapObjects.Clear();
        mapObjects = null;
    }



    public void Display() {
        Vector2 delta = (Player.Current.lerp * World.Current.scrolling);
        delta = new Vector2(delta.x * Resolution.x, delta.y * Resolution.y);

        foreach (Tile tile in VisibleTiles) {
            if (tile.Image != null)
                GUI.DrawTexture(new Rect(Resolution.x * (tile.mapCoords.x - World.Current.coordsOffset.x) - delta.x, Resolution.y * (tile.mapCoords.y - World.Current.coordsOffset.y) - delta.y, Resolution.x, Resolution.y), tile.Image);
        }
    }

    public void UpdateVisibleList(Vector2 _startCoords) {
        VisibleTiles = new List<Tile>();
        foreach (Tile t in tiles) {
            if (t.mapCoords.x >= Mathf.Max(_startCoords.x - 1, 0) && 
                t.mapCoords.x <= Mathf.Min(_startCoords.x + MAP_SCREEN_X + 1, Size.x) &&
                t.mapCoords.y >= Mathf.Max(_startCoords.y - 1, 0) && 
                t.mapCoords.y <= Mathf.Min(_startCoords.y + MAP_SCREEN_Y + 1, Size.y))
                VisibleTiles.Add(t);
        }
    }
    public void SortTiles() {
        tiles.Sort(delegate(Tile x, Tile y) { return x.layer.CompareTo(y.layer); });
    }

    public void SetSize(int width, int height) {
        _size = new Vector2(width, height);

        if (tiles == null)
            tiles = new List<Tile>();
        else
            tiles = tiles.FindAll(T => T.mapCoords.x < width && T.mapCoords.y < height);

        bool[ , ] newcollisions = new bool[width, height];
        for (int i = 0; i < width; i++)
            for (int j= 0; j < height; j++)
                newcollisions[i, j] = collisions != null && collisions.GetLength(0) < i && collisions.GetLength(1) > j ? collisions[i, j] : true;

        collisions = newcollisions;
    }

    public Tile GetTile(int layer, int x, int y) {
        return tiles.Find(T => T.mapCoords.x == x && T.mapCoords.y == y && T.layer == layer);
    }
    public List<Tile> GetTiles(int layer) {
        return tiles.FindAll(T => T.layer == layer);
    }
    public List<Tile> GetTiles(int layer, Vector2 minPos) {
        return tiles.FindAll(T => T.layer == layer && T.mapCoords.x >= minPos.x && T.mapCoords.x < minPos.x + MAP_SCREEN_X && T.mapCoords.y >= minPos.y && T.mapCoords.y < minPos.y + MAP_SCREEN_Y);
    }

    public override string ToString() {
        return name;
    }

    // Serialization
    public void Export() {
        string filePath = Config.GetConfigPath(IMAGE_FOLDER) + ID + ".txt";
        if (tiles.Count == 0) {
            if (File.Exists(filePath))
                File.Delete(filePath);
            return;
        }

        StreamWriter sw = new StreamWriter(filePath);

        sw.WriteLine("------------");
        sw.WriteLine("- Map " + InterfaceUtility.IntString(ID, 4) + " -");
        sw.WriteLine("------------");
        sw.WriteLine(name);
        sw.WriteLine(_size.x + ";" + _size.y);
        sw.WriteLine();

        List<Tile> toExport = tiles.FindAll(T => T.mapCoords.x < _size.x && T.mapCoords.y < _size.y);
        // Sort !
        toExport.Sort(delegate(Tile a, Tile b) { return a.CompareTo(b); });
        
        sw.WriteLine(toExport.Count);
        foreach (Tile i in toExport) {
            sw.WriteLine(i.mapCoords.x + "#" + i.mapCoords.y + "#" + i.layer + "#" + i.originTile + "#" + i.originTileCoords.x + "#" + i.originTileCoords.y);
        }
        sw.WriteLine();

        sw.WriteLine(collisions.Length);
        for (int i = 0; i < _size.x; i++)
            for (int j= 0; j < _size.y; j++)
                sw.WriteLine(i + "#" + j + "#" + collisions[i, j]);
        sw.WriteLine();

        sw.Close();
        sw.Dispose();
    }
    public void Import() {
        string filePath = Config.GetConfigPath(IMAGE_FOLDER) + ID + ".txt";
        
        if (!File.Exists(filePath))
            throw new System.Exception("Try to load a map with inexistant ID : " + ID);
        
        StreamReader sr = new StreamReader(filePath);
        
        // Read title
        sr.ReadLine();
        sr.ReadLine();
        sr.ReadLine();

        // Read file info
        name = sr.ReadLine();
        string line = sr.ReadLine();
        string[] values = line.Split(';');
        _size.x = int.Parse(values[0]);
        _size.y = int.Parse(values[1]);

        sr.ReadLine();

        // Read tiles
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

        line = sr.ReadLine();

        // Read collisions
        collisions = new bool[(int)_size.x, (int)_size.y];
        for (int i = 0; i < _size.x; i++)
            for (int j= 0; j < _size.y; j++)
                collisions[i, j] = true;
            
        try {
            count = int.Parse(sr.ReadLine());
            for (int i = 0; i < count; i++) {
                line = sr.ReadLine();

                values = line.Split('#');

                collisions[int.Parse(values[0]), int.Parse(values[1])] = bool.Parse(values[2]);
            }
        } catch { }
        line = sr.ReadLine();


        sr.Close();
        sr.Dispose();
    }

    public static bool Exists(int id) {
        string filePath = Config.GetConfigPath(IMAGE_FOLDER) + id + ".txt";
        return File.Exists(filePath);
    }

    public void ImportName() {
        string filePath = Config.GetConfigPath(IMAGE_FOLDER) + ID + ".txt";

        if (!File.Exists(filePath))
            throw new System.Exception("Try to load a map with inexistant ID : " + ID);

        StreamReader sr = new StreamReader(filePath);

        // Read title
        sr.ReadLine();
        sr.ReadLine();
        sr.ReadLine();

        // Read file info
        name = sr.ReadLine();

        sr.Close();
        sr.Dispose();
    }
}
