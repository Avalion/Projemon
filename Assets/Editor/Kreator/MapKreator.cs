using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapKreator : EditorWindow {
    public static List<Map> elements = new List<Map>();
    public int selectedElement = 0;
    private Map current {
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
    
    private static int currentLayer = 4;

    private Vector2 selectedCoords = -Vector2.one;

    // Display
    private bool displayAllLayers = true;
    private bool isDragging = false;
    private Vector2 startDragMousePosition;
    private bool drawRectMode = true;
    private GUIStyle posDisplayStyle = new GUIStyle();
    
    private bool forceMode = false;

    private Vector2 currentMapSize;

    private Vector2 mapScrollPos = Vector2.zero;


    // Launch
    [MenuItem("Creation/Maps &L")]
    public static void Init() {
        MapKreator window = EditorWindow.GetWindow<MapKreator>();
        window.minSize = new Vector2(1200, 500);
        window.maxSize = new Vector2(1200, 501);
        window.Show();

        if (!DataBase.IsConnected) DataBase.Connect(Application.dataPath + "/database.sql");

        InterfaceUtility.ClearAllCache();

        elements = SystemDatas.GetMaps();
        numberElements = elements.Count;

        if (numberElements > 0)
            window.Select(0);

        patterns = SystemDatas.GetMapsPatterns();
        
        if (patterns.Count > 0) {
            selectedPattern = 0;
            UpdateImages();
        }

        window.InitStyles();

        window.saved = false;
    }

    [MenuItem("Creation/Maps &L", true)]
    public static bool InitOK() { return !Application.isPlaying; }

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
    
    // Verify if subEditors are not open !
    public bool CanEdit() {
        return !MapObjectKreator.isOpen;
    }

    // Display
    public void OnGUI() {
        if (!CanEdit())
            GUI.Button(new Rect(0, 0, Screen.width, Screen.height), "", InterfaceUtility.EmptyStyle);

        if (patterns.Count == 0) {
            GUILayout.Label("No patterns in " + Config.GetResourcePath(Map.IMAGE_FOLDER));
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
        int sel = UtilityEditor.DisplayList<Map>(selectedElement, elements, ref scrollPosList);
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

        // Check return event before Unity Text because it use the KeyDown Event
        if (GUI.GetNameOfFocusedControl() == "EditorListSize" && Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return)) {
            while (elements.Count < numberElements)
                elements.Add(new Map(elements.Count));
            while (elements.Count > numberElements) {
                toDestroy.Add(elements[elements.Count - 1]);
                elements.RemoveAt(elements.Count - 1);
            }
            Select(Mathf.Clamp(selectedElement, 0, numberElements - 1));
        }

        GUI.SetNextControlName("EditorListSize");
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
        // Count the scroll bar (isn't in layout) !
        GUILayout.Space(20);

        // Images
        GUILayout.BeginVertical();
        if (elements.Count > 0 && selectedElement >= 0) {
            current.name = EditorGUILayout.TextField("Name", current.name);
            
            // Check return event before Unity Text because it use the KeyDown Event
            if ((GUI.GetNameOfFocusedControl() == "EditorMapSizeX" || GUI.GetNameOfFocusedControl() == "EditorMapSizeY") && Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return) && currentMapSize != current.Size) {
                current.SetSize((int)currentMapSize.x, (int)currentMapSize.y);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Size : ");
            GUILayout.Label("X");
            GUI.SetNextControlName("EditorMapSizeX");
            currentMapSize.x = Mathf.Max(EditorGUILayout.IntField((int)currentMapSize.x), Map.MAP_SCREEN_X);
            GUILayout.Label("Y");
            GUI.SetNextControlName("EditorMapSizeY");
            currentMapSize.y = Mathf.Max(EditorGUILayout.IntField((int)currentMapSize.y), Map.MAP_SCREEN_Y);
            if (GUILayout.Button("Apply"))
                current.SetSize((int)currentMapSize.x, (int)currentMapSize.y);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            currentLayer = EditorGUILayout.IntSlider(currentLayer, 0, 4);
            if (currentLayer < 3)
                GUILayout.Label("Layer " + currentLayer, GUILayout.Width(80));
            if (currentLayer == 3)
                GUILayout.Label("Collisions", GUILayout.Width(80));
            if (currentLayer == 4)
                GUILayout.Label("Events", GUILayout.Width(80));
            if (currentLayer == 5)
                GUILayout.Label("Combat Zones", GUILayout.Width(80));
            GUILayout.EndHorizontal();

            if (currentLayer < 3) {
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
                    GUILayout.Label(currentPattern[0, 0], InterfaceUtility.EmptyStyle, GUILayout.Width(Map.Tile.TILE_RESOLUTION), GUILayout.Height(Map.Tile.TILE_RESOLUTION));
                    GUILayout.EndHorizontal();
                }
            } else if (currentLayer == 3) {
                
            } else if (currentLayer == 4) {
                if (selectedCoords != -Vector2.one) {
                    // check if there is a MO on the case
                    MapObject mo = current.mapObjects.Find(MO => MO.mapCoords == selectedCoords);

                    if (mo == null && DataBase.SelectUnique<DBSystem>().playerMapID == current.ID && DataBase.SelectUnique<DBSystem>().playerCoords == selectedCoords)
                        mo = Player.Current;
                    
                    GUILayout.BeginVertical();
                    GUI.enabled = mo != Player.Current;
                    if (GUILayout.Button(mo != null ? "Editer" : "Créer")) {
                        if (mo == null) {
                            DBMapObject dbmo = new DBMapObject() { mapId = current.ID, mapCoords = selectedCoords, name = DataBaseEditorUtility.GetUniqueMapObjectName("MapObject") };
                            DataBase.Insert<DBMapObject>(dbmo);
                            dbmo.ID = DataBase.GetLastInsertId();
                            mo = MapObject.Generate(dbmo);

                            current.mapObjects.Add(mo);
                        }

                        MapObjectKreator.Open(mo);
                    }
                    GUI.enabled = mo != null && mo != Player.Current;
                    if (GUILayout.Button("Supprimer")) {
                        DataBase.SelectById<DBMapObject>(mo.mapObjectId).Delete();
                        current.mapObjects.Remove(mo);
                    }
                    GUI.enabled = mo == null;
                    if (GUILayout.Button("Player")) {
                        DataBase.Update<DBSystem>("playerMapID", current.ID);
                        DataBase.Update<DBSystem>("playerCoordsX", (int)selectedCoords.x);
                        DataBase.Update<DBSystem>("playerCoordsY", (int)selectedCoords.y);
                    }
                    GUI.enabled = true;
                    GUILayout.EndVertical();
                }
            } else if (currentLayer == 5) {
                //TODO : Define how to do Combat Zone
            }
        }
        GUILayout.FlexibleSpace();
        if (currentLayer < 3 && GUILayout.Button("Erase")) {
            currentTileCoords = new Vector2(-1, -1);
            selectedPattern = -1;
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("OK")) {
            Dispose();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        /** Events
         */
        CanvasEvents(canvasRect);
    }

    // Close
    private bool saved = false;
    public void Save() {
        SystemDatas.SetMaps(elements);
    }
    public void Dispose() {
        Save();
        saved = true;
        Close();
    }
    public void OnDestroy() {
        if (!saved)
            Save();
    }

    public void DisplayCanvas() {
        canvasRect = GUILayoutUtility.GetRect((Screen.width - 150) * 0.75f - 20, Screen.height - 80);

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

                foreach (Map.Tile tile in current.GetTiles(layer, mapScrollPos)) {
                    if (tile.Image != null) {
                        Color c = GUI.color;
                        GUI.color = new Color(c.r, c.g, c.b, layer <= currentLayer ? 1 : 0.3f);

                        GUI.DrawTexture(new Rect(resolution.x * (tile.mapCoords.x - (int)mapScrollPos.x), resolution.y * (tile.mapCoords.y - (int)mapScrollPos.y), resolution.x, resolution.y), tile.Image);
                        GUI.color = c;
                    }
                }
            }
            // Collision 
            if (currentLayer == 3) {
                for (int i = (int)mapScrollPos.x; i < (int)mapScrollPos.x + Map.MAP_SCREEN_X; i++)
                    for (int j = (int)mapScrollPos.y; j < (int)mapScrollPos.y + Map.MAP_SCREEN_Y; j++) {
                        if (GUI.Button(new Rect((i - (int)mapScrollPos.x) * resolution.x, (j - (int)mapScrollPos.y) * resolution.y, resolution.x, resolution.y), current.collisions[i, j] ? "O" : "X", InterfaceUtility.CenteredStyle)) {
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
                if (drawRectMode && isDragging)
                    EditorGUI.DrawRect(new Rect(startDragMousePosition.x, startDragMousePosition.y, Event.current.mousePosition.x - startDragMousePosition.x, Event.current.mousePosition.y - startDragMousePosition.y), new Color(0,0,0,0.5f));
            }
            // Events
            if (currentLayer == 4) {
                for (int i = (int)mapScrollPos.x; i < (int)mapScrollPos.x + Map.MAP_SCREEN_X; i++)
                    for (int j = (int)mapScrollPos.y; j < (int)mapScrollPos.y + Map.MAP_SCREEN_Y; j++) {
                        Rect caseRect = new Rect((i - (int)mapScrollPos.x) * resolution.x, (j - (int)mapScrollPos.y) * resolution.y, resolution.x, resolution.y);

                        // check if there is a MO on the case
                        MapObject mo = current.mapObjects.Find(MO => MO.mapCoords == new Vector2(i, j));
                        if (mo != null) {

                            Rect r = new Rect(caseRect);
                            if (mo.execCondition == MapObject.ExecutionCondition.Action) {
                                EditorGUI.DrawRect(new Rect(r.x - r.width / 2, r.y + r.height / 3, r.width, r.height / 3), new Color(1f, 0.2f, 0.2f, 0.2f)); // left
                                EditorGUI.DrawRect(new Rect(r.x + r.width / 2, r.y + r.height / 3, r.width, r.height / 3), new Color(1f, 0.2f, 0.2f, 0.2f)); // right
                                EditorGUI.DrawRect(new Rect(r.x + r.width / 3, r.y - r.height / 2, r.width / 3, r.height), new Color(1f, 0.2f, 0.2f, 0.2f)); // up
                                EditorGUI.DrawRect(new Rect(r.x + r.width / 3, r.y + r.height / 2, r.width / 3, r.height), new Color(1f, 0.2f, 0.2f, 0.2f)); // down
                            } else if (mo.execCondition == MapObject.ExecutionCondition.ActionFace) {
                                if (mo.orientation == MapObject.Orientation.Down)
                                    EditorGUI.DrawRect(new Rect(r.x + r.width / 3, r.y + r.height / 2, r.width / 3, r.height), new Color(1f, 0.2f, 0.2f, 0.2f)); // down
                                else if (mo.orientation == MapObject.Orientation.Up)
                                    EditorGUI.DrawRect(new Rect(r.x + r.width / 3, r.y - r.height / 2, r.width / 3, r.height), new Color(1f, 0.2f, 0.2f, 0.2f)); // up
                                else if (mo.orientation == MapObject.Orientation.Left)
                                    EditorGUI.DrawRect(new Rect(r.x - r.width / 2, r.y + r.height / 3, r.width, r.height / 3), new Color(1f, 0.2f, 0.2f, 0.2f)); // left
                                else if (mo.orientation == MapObject.Orientation.Right)
                                    EditorGUI.DrawRect(new Rect(r.x + r.width / 2, r.y + r.height / 3, r.width, r.height / 3), new Color(1f, 0.2f, 0.2f, 0.2f)); // right
                            } else if (mo.execCondition == MapObject.ExecutionCondition.Contact) {
                                EditorGUI.DrawRect(new Rect(r.x - r.width / 2, r.y + r.height / 3, r.width, r.height / 3), new Color(0.2f, 0.2f, 1f, 0.2f)); // left
                                EditorGUI.DrawRect(new Rect(r.x + r.width / 2, r.y + r.height / 3, r.width, r.height / 3), new Color(0.2f, 0.2f, 1f, 0.2f)); // right
                                EditorGUI.DrawRect(new Rect(r.x + r.width / 3, r.y - r.height / 2, r.width / 3, r.height), new Color(0.2f, 0.2f, 1f, 0.2f)); // up
                                EditorGUI.DrawRect(new Rect(r.x + r.width / 3, r.y + r.height / 2, r.width / 3, r.height), new Color(0.2f, 0.2f, 1f, 0.2f)); // down
                            } else if (mo.execCondition == MapObject.ExecutionCondition.Distance) {
                                for (int i2 = (int)mapScrollPos.x; i2 < (int)mapScrollPos.x + Map.MAP_SCREEN_X; i2++)
                                    for (int j2 = (int)mapScrollPos.y; j2 < (int)mapScrollPos.y + Map.MAP_SCREEN_Y; j2++) {
                                        Vector2 distance = mo.mapCoords - new Vector2(i2, j2);
                                        if (Mathf.Abs(distance.x) + Mathf.Abs(distance.y) <= mo.actionDistance)
                                            EditorGUI.DrawRect(new Rect((i2 - (int)mapScrollPos.x) * resolution.x, (j2 - (int)mapScrollPos.y) * resolution.y, resolution.x, resolution.y), new Color(1f, 0.2f, 0.2f, 0.2f));
                                    }
                            } else if (mo.execCondition == MapObject.ExecutionCondition.DistanceFace) {
                                if (mo.orientation == MapObject.Orientation.Down)
                                    EditorGUI.DrawRect(new Rect(r.x + r.width / 3, r.y + r.height / 2, r.width / 3, r.height * mo.actionDistance), new Color(1f, 0.2f, 0.2f, 0.2f)); // down
                                else if (mo.orientation == MapObject.Orientation.Up)
                                    EditorGUI.DrawRect(new Rect(r.x + r.width / 3, r.y - r.height * (mo.actionDistance - 0.5f), r.width / 3, r.height * mo.actionDistance), new Color(1f, 0.2f, 0.2f, 0.2f)); // up
                                else if (mo.orientation == MapObject.Orientation.Left)
                                    EditorGUI.DrawRect(new Rect(r.x - r.width * (mo.actionDistance - 0.5f), r.y + r.height / 3, r.width * mo.actionDistance, r.height / 3), new Color(1f, 0.2f, 0.2f, 0.2f)); // left
                                else if (mo.orientation == MapObject.Orientation.Right)
                                    EditorGUI.DrawRect(new Rect(r.x + r.width / 2, r.y + r.height / 3, r.width * mo.actionDistance, r.height / 3), new Color(1f, 0.2f, 0.2f, 0.2f)); // right
                            }

                            
                                

                            EditorGUI.DrawRect(caseRect, new Color(0, 0, 0, 0.4f));
                            
                            // Display the Sprite
                            if (GUI.Button(caseRect, mo.Sprite, InterfaceUtility.CenteredStyle))
                                selectedCoords = new Vector2(i, j);

                            // Square border
                            UtilityEditor.DrawEmptyRect(caseRect, 3, new Color(0, 0, 0, 0.7f));
                            // Square
                            UtilityEditor.DrawEmptyRect(new Rect(caseRect.x + 1, caseRect.y + 1, caseRect.width - 2, caseRect.height - 2), 1, new Color(0.7f, 0.7f, 0.7f, 1));
                        } else {
                            // Else, display nothing
                            if (GUI.Button(caseRect, "", InterfaceUtility.CenteredStyle) && Event.current.button == 0)
                                selectedCoords = new Vector2(i, j);
                        }
                    }

                // Player start pos
                if (DataBase.SelectUnique<DBSystem>().playerMapID == current.ID && IsVisible(DataBase.SelectUnique<DBSystem>().playerCoords)) {
                    Rect playerRect = new Rect((DataBase.SelectUnique<DBSystem>().playerCoords.x - (int)mapScrollPos.x) * resolution.x, (DataBase.SelectUnique<DBSystem>().playerCoords.y - (int)mapScrollPos.y) * resolution.y, resolution.x, resolution.y);

                    // bg
                    EditorGUI.DrawRect(playerRect, new Color(0.2f, 0.2f, 0.2f, 0.7f));
                    // Square border
                    UtilityEditor.DrawEmptyRect(playerRect, 3, new Color(0, 0, 0, 0.7f));
                    // Square
                    UtilityEditor.DrawEmptyRect(new Rect(playerRect.x + 1, playerRect.y + 1, playerRect.width - 2, playerRect.height - 2), 1, new Color(0.7f, 0.7f, 0.7f, 1));

                    GUI.Label(playerRect, "S", InterfaceUtility.CenteredStyle);
                }

                if (selectedCoords != -Vector2.one) {
                    Rect caseRect = new Rect((selectedCoords.x - (int)mapScrollPos.x) * resolution.x, (selectedCoords.y - (int)mapScrollPos.y) * resolution.y, resolution.x, resolution.y);
                    // Square border
                    UtilityEditor.DrawEmptyRect(caseRect, 3, new Color(0, 0, 0, 0.7f));
                    // Square
                    UtilityEditor.DrawEmptyRect(new Rect(caseRect.x + 1, caseRect.y + 1, caseRect.width - 2, caseRect.height - 2), 1, new Color(1, 1, 1, 1f));
                }

                if (isDragging) {
                    Vector2 currentCoords = new Vector2(Mathf.Clamp((int)((Event.current.mousePosition.x) / resolution.x), 0, (int)current.Size.x), Mathf.Clamp((int)((Event.current.mousePosition.y) / resolution.y), 0, (int)current.Size.y)) + mapScrollPos;

                    // check if there is a MO on the case
                    MapObject mo = current.mapObjects.Find(MO => MO.mapCoords != currentCoords);
                    if (mo == null) {
                        Rect caseRect = new Rect(currentCoords.x * resolution.x, currentCoords.y * resolution.y, resolution.x, resolution.y);
                        // Square border
                        UtilityEditor.DrawEmptyRect(caseRect, 4, new Color(0, 0, 0, 0.7f));
                        // Square
                        UtilityEditor.DrawEmptyRect(new Rect(caseRect.x + 1, caseRect.y + 1, caseRect.width - 2, caseRect.height - 2), 2, new Color(1, 1, 1, 0.7f));
                    }
                }
            }
            // Combat Zones
            if (currentLayer == 5) {
                // TODO !
            }
        }

        int x = Mathf.Clamp((int)((Event.current.mousePosition.x) / resolution.x), 0, Map.MAP_SCREEN_X) + (int)mapScrollPos.x;
        int y = Mathf.Clamp((int)((Event.current.mousePosition.y) / resolution.y), 0, Map.MAP_SCREEN_Y) + (int)mapScrollPos.y;
            
        GUI.Label(new Rect(0, 0, 70, 20), "X: " + x + " Y: " + y, posDisplayStyle);

        GUI.EndGroup();

        mapScrollPos.x = Mathf.RoundToInt(GUI.HorizontalScrollbar(new Rect(canvasRect.x, canvasRect.y + canvasRect.height + 2, canvasRect.width, 20), mapScrollPos.x, 1, 0, current.Size.x - Map.MAP_SCREEN_X + 1));
        mapScrollPos.y = Mathf.RoundToInt(GUI.VerticalScrollbar(new Rect(canvasRect.x + canvasRect.width + 2, canvasRect.y, 20, canvasRect.height), mapScrollPos.y, 1, 0, current.Size.y - Map.MAP_SCREEN_Y + 1));

        if (!IsVisible(selectedCoords))
            selectedCoords = -Vector2.one;
    }
    private void CanvasEvents(Rect _rect) {
        /** Keyboard Events
         */
        if (Event.current.type == EventType.KeyDown) {
            if (selectedCoords != -Vector2.one) {
                if (Event.current.keyCode == KeyCode.LeftArrow)
                    selectedCoords.x = Mathf.Max(0, selectedCoords.x - 1);
                if (Event.current.keyCode == KeyCode.RightArrow)
                    selectedCoords.x = Mathf.Min(current.Size.x - 1, selectedCoords.x + 1);
                if (Event.current.keyCode == KeyCode.UpArrow)
                    selectedCoords.y = Mathf.Max(0, selectedCoords.y - 1);
                if (Event.current.keyCode == KeyCode.DownArrow)
                    selectedCoords.y = Mathf.Min(current.Size.y - 1, selectedCoords.y + 1);

                if (selectedCoords.x < mapScrollPos.x)                    mapScrollPos.x = selectedCoords.x;
                if (selectedCoords.y < mapScrollPos.y)                    mapScrollPos.y = selectedCoords.y;
                if (selectedCoords.x > mapScrollPos.x + Map.MAP_SCREEN_X) mapScrollPos.x = selectedCoords.x - Map.MAP_SCREEN_X;
                if (selectedCoords.y > mapScrollPos.y + Map.MAP_SCREEN_Y) mapScrollPos.y = selectedCoords.y - Map.MAP_SCREEN_Y;
            }

            if (Event.current.keyCode == KeyCode.Escape)
                selectedCoords = -Vector2.one;
        }

        if (Event.current.type == EventType.KeyDown) {
            if (Event.current.keyCode == KeyCode.H) {
                displayAllLayers = !displayAllLayers;
            }
        }

        if (!_rect.Contains(Event.current.mousePosition)) {
            isDragging = false;
            Repaint();
            return;
        }
        /** Mouse Events
         */
        Vector2 resolution = new Vector2(_rect.width / (float)Map.MAP_SCREEN_X, _rect.height / (float)Map.MAP_SCREEN_Y);

        int x = (int)((Event.current.mousePosition.x - _rect.x) / resolution.x) + (int)mapScrollPos.x;
        int y = (int)((Event.current.mousePosition.y - _rect.y) / resolution.y) + (int)mapScrollPos.y;

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
            isDragging = true;
            startDragMousePosition = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);
        }

        bool isDraggingFinishing = false;

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0) {
            isDragging = false;
            isDraggingFinishing = true;
        }

        if (currentLayer < 3) {
            #region Tiles
            if (isDraggingFinishing) {
                if (drawRectMode) {
                    int startx = (int)((startDragMousePosition.x - _rect.x) / resolution.x) + (int)mapScrollPos.x;
                    int starty = (int)((startDragMousePosition.y - _rect.y) / resolution.y) + (int)mapScrollPos.y;
                    for (int i = (int)Mathf.Min(x, startx); i <= Mathf.Max(x, startx); i++) {
                        for (int j = (int)Mathf.Min(y, starty); j <= Mathf.Max(y, starty); j++) {
                            SetTile(currentLayer, i, j);
                        }
                    }
                }
            }

            if (isDragging && !drawRectMode)
                SetTile(currentLayer, x, y);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1) {
                if (current.GetTile(currentLayer, x, y) != null) {
                    Map.Tile t = current.GetTile(currentLayer, x, y);
                    selectedPattern = patterns.IndexOf(t.originTile);
                    currentTileCoords = t.originTileCoords;
                    UpdateImages();
                }
            }
            #endregion
        } else if (currentLayer == 4) {
            #region MapObjects
            if (isDraggingFinishing) {
                // check if there is a MO on the case
                Vector2 currentCoords = new Vector2(x, y);
                MapObject mo = current.mapObjects.Find(MO => MO.mapCoords == currentCoords);
                if (mo == null) {
                    mo = current.mapObjects.Find(MO => MO.mapCoords == selectedCoords);
                    if (mo != null)
                        mo.mapCoords = currentCoords;
                }
            }
            #endregion
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
            if (x + j < 0 || y + i < 0 || x + j >= current.Size.x || y + i >= current.Size.y) {
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
                    if ((i == 0 && j == 0) || x + j < 0 || y + i < 0 || x + j >= current.Size.x || y + i >= current.Size.y)
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
                if ((i == 0 && j == 0) || x + j < 0 || y + i < 0 || x + j >= current.Size.x || y + i >= current.Size.y)
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

        mapScrollPos = Vector2.zero;

        World.Current.currentMap = current;

        currentMapSize = current.Size;


        current.Import();

        current.SortTiles();

        current.mapObjects = DataBaseEditorUtility.GetMapObjects(current.ID);

        current.UpdateVisibleList(Vector2.zero);
    }

    private bool IsVisible(Vector2 coords) {
        return coords.x >= mapScrollPos.x && coords.x < mapScrollPos.x + Map.MAP_SCREEN_X && coords.y >= mapScrollPos.y && coords.y < mapScrollPos.y + Map.MAP_SCREEN_Y;
    }
}
