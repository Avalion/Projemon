﻿using UnityEngine;
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

    public List<Tile> tiles = new List<Tile>();
    public bool[,] collisions;
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

        SetSize(50, 30);

        Import();

        SortTiles();

        foreach (DBMapObject mo in DataBase.GetMapObjects(this.ID))
            mapObjects.Add(MapObject.Generate(mo));
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

    public void SetSize(int width, int height) {
        _size = new Vector2(width, height);
        //TODO : Set Tiles and Collisions !
        tiles = new List<Tile>();
        collisions = new bool[(int)Size.x, (int)Size.y];

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
}
