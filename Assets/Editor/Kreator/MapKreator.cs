using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapKreator : EditorWindow {
    public static List<Map> elements = new List<Map>();
    public int selectedElement = 0;
    Map current {
        get { return elements[selectedElement]; }
    }

    // Images
    private static List<string> patterns = new List<string>();
    private static int selectedPattern = -1;
    private static Texture2D[,] currentPattern;

    private Vector2 currentTileCoords;
    // Design
    private GUIStyle tileStyle = new GUIStyle();
    private static GUIStyle selectedTileStyle = new GUIStyle();
    private Vector2 scrollPosList = Vector2.zero;
    
    // Canvas
    private Rect canvasRect;
    private Vector2 scrollpos = new Vector2();
    private Vector2 currentMapCoords;

    private int currentLayer;

    // Display
    public bool displayAllLayers = true;
    public bool isDragging = false;
    public Vector2 startDragMousePosition;
    public bool drawRectMode = true;

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

        InitStyles();
    }

    public static void InitStyles() {
        selectedTileStyle.normal.background = InterfaceUtility.HexaToTexture("#6644FF66");
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
            selectedElement = elements.Count - 1;
            current.tiles.Clear();
            current.name = "";
        }
        if (GUILayout.Button("Delete") && elements.Count > 1) {
            if (selectedElement == elements.Count - 1) {
                elements.RemoveAt(selectedElement);
                selectedElement = elements.Count - 1;
            } else {
                current.tiles.Clear();
                current.name = "";
            }
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
            GUILayout.EndHorizontal();

            int value = EditorGUILayout.Popup(selectedPattern, patterns.ToArray());
            if (value != selectedPattern) {
                selectedPattern = value;
                UpdateImages();
            }

            // Display pattern
            scrollpos = GUILayout.BeginScrollView(scrollpos);
            for (int y = 0; y < currentPattern.GetLength(1); y++) {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                for (int x = 0; x < currentPattern.GetLength(0); x++) {
                    GUILayout.Space(2);
                    tileStyle.normal.background = currentPattern[x,y];
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
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Erase")) {
            currentTileCoords = new Vector2(-1, -1);
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("OK")) {
            SystemDatas.SetMaps(elements);
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
                    if (tile.Image != null)
                        GUI.DrawTexture(new Rect(resolution.x * tile.mapCoords.x, resolution.y * tile.mapCoords.y, resolution.x, resolution.y), tile.Image);
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
                                    foreach (Map.Tile tile in current.tiles.FindAll(T => T != reftile && T.originTile == reftile.originTile && T.originTileCoords == reftile.originTileCoords))
                                        current.collisions[(int)tile.mapCoords.x, (int)tile.mapCoords.y] = !current.collisions[i, j];
                                }
                            }
                            current.collisions[i, j] = !current.collisions[i, j];
                            
                        }
            }
            // EVents
            if (currentLayer == 4) {

            }
        }
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
                            SetTile(currentLayer, i, j, patterns[selectedPattern], currentTileCoords);
                        }
                    }
                }
            }

            if (isDragging) {
                if (drawRectMode) {
                    EditorGUI.DrawRect(new Rect(startDragMousePosition.x, startDragMousePosition.y, Event.current.mousePosition.x - startDragMousePosition.x, Event.current.mousePosition.y - startDragMousePosition.y), InterfaceUtility.HexaToColor("#00CC0044"));
                } else {
                    SetTile(currentLayer, x, y, patterns[selectedPattern], currentTileCoords);
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
        for (int i = -1; i <= 1; ++i)
        for (int j = 1; j >= -1; --j) {
            if (i == 0 && j == 0)
                continue;
            tile = current.GetTile(layer, x + j, y + i);
            if (tile != null) l[0] = tile.originTile == pattern;
        }

        Vector2 mapCoords = Vector2.zero;

        if (!l[1] && !l[3] && !l[4] && !l[6]) // no direct links
            return new Vector2(0, 2);
        if (l[0] && l[1] && l[2] && l[3] && l[4] && l[5] && l[6] && l[7] && l[8]) // Full links
            return new Vector2(0, 0);
        if (l[1] && l[3] && l[4] && l[6]) { // if 4 direct links
            // 0 corners
            if (!l[0] && !l[2] && !l[5] && !l[7]) 
                return new Vector2(4, 0);
            // 1 corner
            if (l[0] && !l[2] && !l[5] && !l[7]) // Only down left corner
                return new Vector2();
            if (!l[0] && l[2] && !l[5] && !l[7]) // Only down right corner
                return new Vector2();
            if (!l[0] && !l[2] && l[5] && !l[7]) // Only up left corner
                return new Vector2();
            if (!l[0] && !l[2] && !l[5] && l[7]) // Only up right corner
                return new Vector2();
            // 2 corner
            if (!l[0] && !l[2] && l[5] && l[7]) // all down
                return new Vector2();
            if (l[0] && !l[2] && l[5] && !l[7]) // all right
                return new Vector2();
            if (l[0] && l[2] && !l[5] && !l[7]) // all up
                return new Vector2();
            if (!l[0] && l[2] && !l[5] && l[7]) // all left
                return new Vector2();
            if (!l[0] && l[2] && l[5] && !l[7]) // Diag /
                return new Vector2();
            if (l[0] && !l[2] && !l[5] && l[7]) // Diag \
                return new Vector2();
            // 3  corners
            if (!l[0] && l[2] && l[5] && l[7]) // Only down left corner missing
                return new Vector2(4, 0);
            if (l[0] && !l[2] && l[5] && l[7]) // Only down right corner missing
                return new Vector2(1, 0);
            if (l[0] && l[2] && !l[5] && l[7]) // Only up left corner missing
                return new Vector2(2, 0);
            if (l[0] && l[2] && l[5] && !l[7]) // Only up right corner missing
                return new Vector2(3, 0);
        }
            
        

        return mapCoords;
    }

    public void SetTile(int layer, int x, int y, string pattern, Vector2 tileCoords) {
        if (tileCoords.x < 0 || tileCoords.y < 0) {
            if (current.GetTile(currentLayer, x, y) != null) {
                current.tiles.Remove(current.GetTile(currentLayer, x, y));
            }
        }
        
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

    public static void UpdateImages() {
        Texture2D pattern = Resources.LoadAssetAtPath(Config.GetResourcePath(Map.IMAGE_FOLDER) + patterns[selectedPattern], typeof(Texture2D)) as Texture2D;

        currentPattern = new Texture2D[pattern.width / Map.Tile.TILE_RESOLUTION, pattern.height / Map.Tile.TILE_RESOLUTION];

        for (int y = 0; y < currentPattern.GetLength(1); y++) {
            for (int x = 0; x < currentPattern.GetLength(0); x++) {
                currentPattern[x, y] = InterfaceUtility.SeparateTexture(pattern, x, y, Map.Tile.TILE_RESOLUTION, Map.Tile.TILE_RESOLUTION);
            }
        }
    }
}
