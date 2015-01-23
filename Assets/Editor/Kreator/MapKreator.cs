using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapKreator : EditorWindow {
    public static List<Map> elements = new List<Map>();
    public int selectedElement = 0;
    Map current {
        get { return elements[selectedElement]; }
    }

    public static List<Map> toDestroy = new List<Map>();

    private static int numberElements = 0;

    // Images
    private static List<string> patterns = new List<string>();
    private static int selectedPattern = -1;

    private static Texture2D[,] currentPattern;
    
    private Vector2 currentTileCoords;
    // Design
    private GUIStyle tileStyle = new GUIStyle();
    private GUIStyle selectedTileStyle = new GUIStyle();
    private Vector2 scrollPosList = Vector2.zero;
    private Vector2 scrollpos = new Vector2();
    
    // Canvas
    private Rect canvasRect;
    private Vector2 currentMapCoords;
    
    private int currentLayer;

    private Vector2 selectedCoords = -Vector2.one;

    // Display
    public bool displayAllLayers = true;
    public bool isDragging = false;
    public Vector2 startDragMousePosition;
    public bool drawRectMode = true;
    public GUIStyle posDisplayStyle = new GUIStyle();
    
    bool forceMode = false;

    // Launch
    [MenuItem("Creation/Maps &L")]
    public static void Init() {
        MapKreator window = EditorWindow.GetWindow<MapKreator>();
        window.minSize = new Vector2(1200, 500);
        window.maxSize = new Vector2(1200, 501);
        window.Show();

        InterfaceUtility.ClearAllCache();

        elements = SystemDatas.GetMaps();
        patterns = SystemDatas.GetMapsPatterns();

        if (patterns.Count > 0) {
            selectedPattern = 0;
            UpdateImages();
        }

        window.InitStyles();
    }

    public void InitStyles() {
        posDisplayStyle = new GUIStyle();
        posDisplayStyle.normal.background = InterfaceUtility.HexaToTexture("#00000055");
        posDisplayStyle.normal.textColor = Color.white;
        posDisplayStyle.alignment = TextAnchor.MiddleCenter;
        
        selectedTileStyle.normal.background = InterfaceUtility.HexaToTexture("#6644FF66");
    }

    public void Update() {
        Repaint();
    }
    // Display
    public void OnGUI() {
        if (patterns.Count == 0) {
            GUILayout.Label("No patterns in " + Config.GetResourcePath("Maps"));
            if (GUILayout.Button("Refresh")) {
                patterns = SystemDatas.GetMapsPatterns();
                if (patterns.Count > 0) {
                    selectedPattern = 0;
                    UpdateImages();
                }
            }
            return;
        }

        GUILayout.BeginHorizontal();
        // List
        GUILayout.BeginVertical(GUILayout.Width(150));
        int sel = EditorUtility.DisplayList<Map>(selectedElement, elements, ref scrollPosList);
        if (sel != selectedElement) {
            selectedElement = sel;
        }
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add")) {
            elements.Add(new Map(elements.Count));
            Select(elements.Count - 1);
            current.tiles.Clear();
            current.name = "";
        }
        if (GUILayout.Button("Delete") && elements.Count > 1) {
            if (selectedElement == elements.Count - 1) {
                toDestroy.Add(elements[selectedElement]);
                elements.RemoveAt(selectedElement);
                Select(elements.Count - 1);
            } else {
                current.tiles.Clear();
                current.name = "";
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Nb ");
        numberElements = EditorGUILayout.IntField(numberElements);
        if (GUILayout.Button("Apply")) {
            while (elements.Count < numberElements)
                elements.Add(new Map(elements.Count));
            while (elements.Count > numberElements) {
                toDestroy.Add(elements[elements.Count - 1]);
                elements.RemoveAt(elements.Count - 1);
            }
            Select(Mathf.Clamp(selectedElement, 0, numberElements - 1));
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        // Canvas
        GUILayout.BeginVertical();
        DisplayCanvas();
        GUILayout.EndVertical();

        // Images
        GUILayout.BeginVertical();
        if (elements.Count > 0 && selectedElement >= 0) {
            current.name = EditorGUILayout.TextField("Name", current.name);

            GUILayout.BeginHorizontal();
            currentLayer = EditorGUILayout.IntSlider(currentLayer, 0, 4);
            if (currentLayer < 3)
                GUILayout.Label("Layer " + currentLayer, GUILayout.Width(80));
            if (currentLayer == 3)
                GUILayout.Label("Collisions", GUILayout.Width(80));
            if (currentLayer == 4)
                GUILayout.Label("Events", GUILayout.Width(80));
            // TODO - Define how to create Combat Zones
            //if (currentLayer == 5)
            //    GUILayout.Label("Combat Zones", GUILayout.Width(80));
            GUILayout.EndHorizontal();

            int value = EditorGUILayout.Popup(selectedPattern, patterns.ToArray());
            if (value != selectedPattern) {
                selectedPattern = value;
                UpdateImages();
            }

            forceMode = GUILayout.Toggle(forceMode, "Forced mode ?");
            if (forceMode) {
                scrollpos = GUILayout.BeginScrollView(scrollpos);
                for (int y = 0; y < currentPattern.GetLength(1); y++) {
                    GUILayout.Space(2);
                    GUILayout.BeginHorizontal();
                    for (int x = 0; x < currentPattern.GetLength(0); x++) {
                        GUILayout.Space(2);
                        tileStyle.normal.background = currentPattern[x, y];
                        if (GUILayout.Button("", tileStyle, GUILayout.Width(Map.Tile.TILE_RESOLUTION), GUILayout.Height(Map.Tile.TILE_RESOLUTION)))
                            currentTileCoords = new Vector2(x, y);
                        if (x == currentTileCoords.x && y == currentTileCoords.y) {
                            GUILayout.Space(-Map.Tile.TILE_RESOLUTION);
                            GUILayout.Label("", selectedTileStyle, GUILayout.Width(Map.Tile.TILE_RESOLUTION), GUILayout.Height(Map.Tile.TILE_RESOLUTION));
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            } else {
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUILayout.Label(currentPattern[0,0], InterfaceUtility.EmptyStyle, GUILayout.Width(Map.Tile.TILE_RESOLUTION), GUILayout.Height(Map.Tile.TILE_RESOLUTION));
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Erase")) {
            currentTileCoords = new Vector2(-1, -1);
            selectedPattern = -1;
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("OK")) {
            SystemDatas.SetMaps(elements);

            foreach (Map map in toDestroy)
                foreach (MapObject mo in map.mapObjects)
                    DataBase.DeleteByID<DBMapObject>(mo.mapObjectId);
            toDestroy.Clear();    
            Close();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        /** Events
         */
        CanvasEvents(canvasRect);
    }

    public void DisplayCanvas() {
        canvasRect = GUILayoutUtility.GetRect((Screen.width - 150) * 0.75f, Screen.height - 60);

        // Box
        canvasRect = MathUtility.ExtendRect(canvasRect, -4);
        EditorGUI.DrawRect(canvasRect, Color.black);
        canvasRect = MathUtility.ExtendRect(canvasRect, -2);
        EditorGUI.DrawRect(canvasRect, Color.grey);

        Vector2 resolution = new Vector2(canvasRect.width / (float)Map.MAP_SCREEN_X, canvasRect.height / (float)Map.MAP_SCREEN_Y);

        // Grid
        
        /** Display
         */
        GUI.BeginGroup(canvasRect);
        if (elements.Count > 0 && selectedElement >= 0) {
            // Map
            for (int layer = 0; layer < 3; layer++) {
                if (layer == currentLayer) {
                    for (int i = 1; i < Map.MAP_SCREEN_X; i++)
                        EditorGUI.DrawRect(new Rect(i * resolution.x - 1, 0, 2, canvasRect.height), Color.white);
                    for (int i = 1; i < Map.MAP_SCREEN_Y; i++)
                        EditorGUI.DrawRect(new Rect(0, i * resolution.y - 1, canvasRect.width, 2), Color.white);
                }
                if (!displayAllLayers && layer > currentLayer)
                    break;

                foreach (Map.Tile tile in current.GetTiles(layer)) {
                    if (tile.Image != null) {
                        Color c = GUI.color;
                        GUI.color = new Color(c.r, c.g, c.b, layer <= currentLayer ? 1 : 0.3f);
                        
                        GUI.DrawTexture(new Rect(resolution.x * tile.mapCoords.x, resolution.y * tile.mapCoords.y, resolution.x, resolution.y), tile.Image);
                        GUI.color = c;
                    }
                }
            }
            // Collision 
            if (currentLayer == 3) {
                for (int i = 0; i < Map.MAP_SCREEN_X; i++)
                    for (int j = 0; j < Map.MAP_SCREEN_Y; j++)
                        if (GUI.Button(new Rect(i * resolution.x, j * resolution.y, resolution.x, resolution.y), current.collisions[i, j] ? "O" : "X", InterfaceUtility.CenteredStyle)) {
                            if (Event.current.control) {
                                Map.Tile reftile = current.tiles.Find(T => T.mapCoords.x == i && T.mapCoords.y == j && T.layer == 0);
                                if (reftile != null) {
                                    foreach (Map.Tile tile in current.tiles.FindAll(T => T != reftile && T.originTile == reftile.originTile))
                                        current.collisions[(int)tile.mapCoords.x, (int)tile.mapCoords.y] = !current.collisions[i, j];
                                }
                            }
                            current.collisions[i, j] = !current.collisions[i, j];
                        }
            }
            // Events
            if (currentLayer == 4) {
                for (int i = 0; i < Map.MAP_SCREEN_X; i++)
                    for (int j = 0; j < Map.MAP_SCREEN_Y; j++) {
                        Rect caseRect = new Rect(i * resolution.x, j * resolution.y, resolution.x, resolution.y);

                        // check if there is a MO on the case
                        MapObject mo = current.mapObjects.Find(MO => MO.mapCoords != new Vector2(i, j));
                        if (mo != null) {
                            // Display the Sprite
                            if (GUI.Button(caseRect, mo.sprite, InterfaceUtility.CenteredStyle))
                                selectedCoords = selectedCoords == -Vector2.one ? new Vector2(i, j) : -Vector2.one;

                            // Square border
                            EditorUtility.DrawSquare(caseRect, 4, new Color(0, 0, 0, 0.7f));
                            // Square
                            EditorUtility.DrawSquare(new Rect(caseRect.x + 1, caseRect.y + 1, caseRect.width - 2, caseRect.height - 2), 2, new Color(1, 1, 1, 0.7f));
                        } else {
                            // Else, display nothing
                            if (GUI.Button(caseRect, "", InterfaceUtility.CenteredStyle))
                                selectedCoords = selectedCoords == -Vector2.one ? new Vector2(i,j) : -Vector2.one;
                        }

                        if (selectedCoords == new Vector2(i, j)) {
                            // Square border
                            EditorUtility.DrawSquare(caseRect, 4, new Color(1, 1, 1, 0.7f));
                            // Square
                            EditorUtility.DrawSquare(new Rect(caseRect.x + 1, caseRect.y + 1, caseRect.width - 2, caseRect.height - 2), 2, new Color(0.5f, 0.5f, 1, 0.7f));
                        }
                    }

                // Player start pos
                if (World.Current.startMapID == current.ID) {
                    Rect playerRect = new Rect(World.Current.startPlayerCoords.x * resolution.x, World.Current.startPlayerCoords.y * resolution.y, resolution.x, resolution.y);
                    EditorGUI.DrawRect(playerRect, new Color(0.8f, 0.8f, 1, 0.5f));
                    GUI.Label(playerRect, "S", InterfaceUtility.CenteredStyle);
                }

                // if a case is selected
                if (selectedCoords != -Vector2.one) {
                    // check if there is a MO on the case
                    MapObject mo = current.mapObjects.Find(MO => MO.mapCoords != selectedCoords);
                    if (mo == null && World.Current.startMapID == current.ID && World.Current.startPlayerCoords == selectedCoords)
                        mo = Player.Current;
                    
                    // calc the optionsRect to don't overview the canvas
                    Rect optionsRect = new Rect((selectedCoords.x + 0.8f) * resolution.x, (selectedCoords.y + 0.8f) * resolution.y, 100, 100);
                    if (optionsRect.x + optionsRect.width > canvasRect.width)
                        optionsRect.x -= optionsRect.width + 0.6f * resolution.x;
                    if (optionsRect.y + optionsRect.height > canvasRect.height)
                        optionsRect.y -= optionsRect.height + 0.6f * resolution.y;

                    GUILayout.BeginArea(optionsRect);
                    GUILayout.BeginVertical();
                    GUI.enabled = mo != Player.Current;
                    if (GUILayout.Button(mo != null ? "Editer" : "Créer")) {
                        if (mo == null) {
                            DBMapObject dbmo = new DBMapObject() { mapId = current.ID, mapCoords = selectedCoords };
                            DataBase.Insert<DBMapObject>(dbmo);
                            dbmo.ID = DataBase.GetLastInsertId();
                            mo = MapObject.Generate(dbmo);
                        }
                        
                        // TODO : Launch MapObject Editor

                        selectedCoords = -Vector2.one;
                    }
                    GUI.enabled = mo != null && mo != Player.Current;
                    if (GUILayout.Button("Supprimer")) {
                        DataBase.DeleteByID<DBMapObject>(mo.mapObjectId);
                        current.mapObjects.Remove(mo);

                        selectedCoords = -Vector2.one;
                    }
                    GUI.enabled = mo == null;
                    if (GUILayout.Button("Player")) {
                        World.Current.startMapID = current.ID;
                        World.Current.startPlayerCoords = selectedCoords;

                        selectedCoords = -Vector2.one;
                    }
                    GUI.enabled = true;
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                }
            }
            // Combat Zones
            if (currentLayer == 5) {
                // TODO !
            }
        }


        int x = Mathf.Clamp((int)((Event.current.mousePosition.x) / resolution.x), 0, (int)current.size.x);
        int y = Mathf.Clamp((int)((Event.current.mousePosition.y) / resolution.y), 0, (int)current.size.y);
            
        GUI.Label(new Rect(0, 0, 70, 20), "X: " + x + " Y: " + y, posDisplayStyle);
        
        GUI.EndGroup();
    }
    private void CanvasEvents(Rect _rect) {
        if (!_rect.Contains(Event.current.mousePosition)) {
            isDragging = false;
            return;
        }
        /** Mouse Events
         */
        Vector2 resolution = new Vector2(_rect.width / (float)Map.MAP_SCREEN_X, _rect.height / (float)Map.MAP_SCREEN_Y);

        int x = (int)((Event.current.mousePosition.x - _rect.x) / resolution.x);
        int y = (int)((Event.current.mousePosition.y - _rect.y) / resolution.y);

        if (currentLayer < 3) {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                isDragging = true;
                startDragMousePosition = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);
            }
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0) {
                isDragging = false;

                if (drawRectMode) {
                    int startx = (int)((startDragMousePosition.x - _rect.x) / resolution.x);
                    int starty = (int)((startDragMousePosition.y - _rect.y) / resolution.y);
                    for (int i = (int)Mathf.Min(x, startx); i <= Mathf.Max(x, startx); i++) {
                        for (int j = (int)Mathf.Min(y, starty); j <= Mathf.Max(y, starty); j++) {
                            SetTile(currentLayer, i, j);
                        }
                    }
                }
            }

            if (isDragging) {
                if (drawRectMode) {
                    EditorGUI.DrawRect(new Rect(startDragMousePosition.x, startDragMousePosition.y, Event.current.mousePosition.x - startDragMousePosition.x, Event.current.mousePosition.y - startDragMousePosition.y), InterfaceUtility.HexaToColor("#00CC0044"));
                } else {
                    SetTile(currentLayer, x, y);
                }
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1) {
                if (current.GetTile(currentLayer, x, y) != null) {
                    Map.Tile t = current.GetTile(currentLayer, x, y);
                    selectedPattern = patterns.IndexOf(t.originTile);
                    currentTileCoords = t.originTileCoords;
                    UpdateImages();
                }
            }
        }
        /** Keyboard Events
         */
        if (Event.current.type == EventType.KeyDown) {
            if (Event.current.keyCode == KeyCode.H) {
                displayAllLayers = !displayAllLayers;
            }
        }

        Repaint();
    }

    public Vector2 CalcMapCoords(int layer, int x, int y, string pattern) {
        bool[] l = new bool[8] { false, false, false, false, false, false, false, false };
        // 0 down left // 1 down // 2 down right // 3 left // 4 right // 5 up left // 6 up // 7 up right 
        
        Map.Tile tile;
        int count = 0;
        for (int i = 1; i >= -1; --i)
        for (int j = -1; j <= 1; ++j) {
            if (i == 0 && j == 0)
                continue;
            if (x + j < 0 || y + i < 0 || x + j >= current.size.x || y + i >= current.size.y) {
                l[count] = true;
                count++;
                continue;
            }

            tile = current.GetTile(layer, x + j, y + i);
            if (tile != null) 
                l[count] = tile.originTile == pattern;
            count++;
        }

        //Debug.Log(l[5] + ";" + l[6] + ";" + l[7] + "\n" + l[3] + ";xxxx;" + l[4] + "\n" + l[0] + ";" + l[1] + ";" + l[2]);

        #region if 4 direct links
        if (l[1] && l[3] && l[4] && l[6]) {
            // 0 corners
            if (l[0] && l[2] && l[5] && l[7]) 
                return new Vector2(0, 0);
            // 1 corner
            if (!l[0] && l[2] && l[5] && l[7]) // Only down left corner
                return new Vector2(4, 0);
            if (l[0] && !l[2] && l[5] && l[7]) // Only down right corner
                return new Vector2(1, 0);
            if (l[0] && l[2] && !l[5] && l[7]) // Only up left corner
                return new Vector2(2, 0);
            if (l[0] && l[2] && l[5] && !l[7]) // Only up right corner
                return new Vector2(3, 0);
            // 2 corner
            if (!l[0] && !l[2] && l[5] && l[7]) // all down
                return new Vector2(2, 5);
            if (l[0] && !l[2] && l[5] && !l[7]) // all right
                return new Vector2(1, 5);
            if (l[0] && l[2] && !l[5] && !l[7]) // all up
                return new Vector2(0, 5);
            if (!l[0] && l[2] && !l[5] && l[7]) // all left
                return new Vector2(6, 0);
            if (!l[0] && l[2] && l[5] && !l[7]) // Diag /
                return new Vector2(0, 1);
            if (l[0] && !l[2] && !l[5] && l[7]) // Diag \
                return new Vector2(5, 0);
            // 3  corners
            if (l[0] && !l[2] && !l[5] && !l[7]) // Only down left corner missing
                return new Vector2(3, 1);
            if (!l[0] && l[2] && !l[5] && !l[7]) // Only down right corner missing
                return new Vector2(2, 1);
            if (!l[0] && !l[2] && l[5] && !l[7]) // Only up left corner missing
                return new Vector2(4, 1);
            if (!l[0] && !l[2] && !l[5] && l[7]) // Only up right corner missing
                return new Vector2(1, 1);
            // 4 corners
            if (!l[0] && !l[2] && !l[5] && !l[7])
                return new Vector2(5, 1);
        }
        #endregion
        #region if 3 direct links
        if (!l[1] && l[3] && l[4] && l[6]) { // if only down link missing
            if (l[5] && l[7]) // if no corner
                return new Vector2(5, 5);
            if (!l[5] && l[7]) // if left corner
                return new Vector2(6, 3);
            if (l[5] && !l[7]) // if right corner
                return new Vector2(1, 4);
            if (!l[5] && !l[7]) // if all corner
                return new Vector2(6, 4);
        }
        if (l[1] && !l[3] && l[4] && l[6]) { // if only left link missing
            if (l[2] && l[7]) // if no corner
                return new Vector2(6, 1);
            if (!l[2] && l[7]) // if down corner
                return new Vector2(0, 4);
            if (l[2] && !l[7]) // if up corner
                return new Vector2(5, 3);
            if (!l[2] && !l[7]) // if all corner
                return new Vector2(5, 4);
        }
        if (l[1] && l[3] && !l[4] && l[6]) { // if only right link missing
            if (l[0] && l[5]) // if no corner
                return new Vector2(4, 5);
            if (!l[0] && l[5]) // if down corner
                return new Vector2(4, 6);
            if (l[0] && !l[5]) // if up corner
                return new Vector2(3, 3);
            if (!l[0] && !l[5]) // if all corner
                return new Vector2(3, 4);
        }
        if (l[1] && l[3] && l[4] && !l[6]) { // if only up link missing
            if (l[0] && l[2]) // if no corner
                return new Vector2(3, 5);
            if (!l[0] && l[2]) // if left corner
                return new Vector2(4, 3);
            if (l[0] && !l[2]) // if right corner
                return new Vector2(5, 6);
            if (!l[0] && !l[2]) // if all corner
                return new Vector2(4, 4);
        }
        #endregion
        #region if 2 direct links
        if (l[1] && !l[3] && !l[4] && l[6]) // if up and down
            return new Vector2(5, 2);
        if (!l[1] && l[3] && l[4] && !l[6]) // if left and right
            return new Vector2(6, 2);
        if (l[1] && l[3] && !l[4] && !l[6]) { // if down and left
            if (!l[0]) // if corner
                return new Vector2(3, 6);
            // else
            return new Vector2(3, 2);
        }
        if (l[1] && !l[3] && l[4] && !l[6]) { // if down and right
            if (!l[2]) // if corner
                return new Vector2(2, 3);
            // else
            return new Vector2(2, 2);
        }
        if (!l[1] && l[3] && !l[4] && l[6]) { // if up and left
            if (!l[5]) // if corner
                return new Vector2(2, 4);
            // else
            return new Vector2(1, 2);
        }
        if (!l[1] && !l[3] && l[4] && l[6]) { // if up and right
            if (!l[7]) // if corner
                return new Vector2(0, 6);
            // else
            return new Vector2(4, 2);
        }
        #endregion
        #region if 1 direct link
        if (l[1] && !l[3] && !l[4] && !l[6]) // if only down link
            return new Vector2(2, 6);
        if (!l[1] && l[3] && !l[4] && !l[6]) // if only left link
            return new Vector2(0, 3);
        if (!l[1] && !l[3] && l[4] && !l[6]) // if only right link
            return new Vector2(1, 6);
        if (!l[1] && !l[3] && !l[4] && l[6]) // if only up link
            return new Vector2(1, 3);
        #endregion
        if (!l[1] && !l[3] && !l[4] && !l[6]) // no direct links
            return new Vector2(0, 2);
        
        return Vector2.zero;
    }

    public void SetTile(int layer, int x, int y) {
        if (selectedPattern == -1) {
            DeleteTile(layer, x, y, !forceMode);
            return;
        }
        
        if (forceMode)
            SetTile(layer, x, y, patterns[selectedPattern], currentTileCoords);
        else
            SetTile(layer, x, y, patterns[selectedPattern]);
    }
    public void SetTile(int layer, int x, int y, string pattern, bool contagious = true) {
        Vector2 tileCoords = CalcMapCoords(layer, x, y, pattern);

        SetTile(layer, x, y, pattern, tileCoords);

        if (contagious) {
            for (int i = 1; i >= -1; --i)
                for (int j = -1; j <= 1; ++j) {
                    if ((i == 0 && j == 0) || x + j < 0 || y + i < 0 || x + j >= current.size.x || y + i >= current.size.y)
                        continue;

                    Map.Tile tile = current.GetTile(layer, x + j, y + i);
                    if (tile != null)
                        SetTile(tile.layer, (int)tile.mapCoords.x, (int)tile.mapCoords.y, tile.originTile, false);
                }
        }
    }
    public void SetTile(int layer, int x, int y, string pattern, Vector2 tileCoords) {
        if (current.GetTile(layer, x, y) == null) {
            Map.Tile t = new Map.Tile();
            t.layer = layer;
            t.mapCoords = new Vector2(x, y);
            t.originTile = pattern;
            t.originTileCoords = tileCoords;

            t.LoadTexture();

            current.tiles.Add(t);
        } else {
            Map.Tile t = current.GetTile(layer, x, y);
            t.originTile = pattern;
            t.originTileCoords = tileCoords;

            t.LoadTexture();
        }
    }

    public void DeleteTile(int layer, int x, int y, bool contagious = true) {
        if (current.GetTile(currentLayer, x, y) != null)
            current.tiles.Remove(current.GetTile(currentLayer, x, y));

        if (contagious) {
            for (int i = 1; i >= -1; --i)
            for (int j = -1; j <= 1; ++j) {
                if ((i == 0 && j == 0) || x + j < 0 || y + i < 0 || x + j >= current.size.x || y + i >= current.size.y)
                    continue;

                Map.Tile tile = current.GetTile(layer, x + j, y + i);
                if (tile != null)
                    SetTile(tile.layer, (int)tile.mapCoords.x, (int)tile.mapCoords.y, tile.originTile, false);
            }
        }
    }


    public static void UpdateImages() {
        Texture2D pattern = Resources.LoadAssetAtPath(Config.GetResourcePath(Map.IMAGE_FOLDER) + patterns[selectedPattern], typeof(Texture2D)) as Texture2D;

        currentPattern = new Texture2D[pattern.width / Map.Tile.TILE_RESOLUTION, pattern.height / Map.Tile.TILE_RESOLUTION];

        for (int y = 0; y < currentPattern.GetLength(1); y++) {
            for (int x = 0; x < currentPattern.GetLength(0); x++) {
                currentPattern[x, y] = InterfaceUtility.SeparateTexture(pattern, x, y, Map.Tile.TILE_RESOLUTION, Map.Tile.TILE_RESOLUTION);
            }
        }
    }

    public void Select(int _select) {
        selectedElement = _select;
    }
}
