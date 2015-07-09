using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapObjectKreator : EditorWindow {
    public static bool isOpen = false;
    
    private MapObject target = null;

    private Map displayMap = null;

    private int selectedElement = -1;

    private static List<string> sprites = new List<string>();
    private static List<string> sounds = new List<string>();
    private static string[] actionTypes;
    
    private Vector2 scrollPosList;
    private Vector2 scrollPosEdit;

    private static GUIStyle UnityButton;
    private static GUIStyle UnityActiveButton;

    private int indent = 0;




    public static void Open(MapObject mo) {
        if (mo == null)
            return;
        MapObjectKreator window = EditorWindow.GetWindow<MapObjectKreator>();
        window.minSize = new Vector2(1000, 500);
        window.maxSize = new Vector2(1000, 501);
        window.target = mo;
        window.Show();

        InitStyles();
        InitLists(); 

        isOpen = true;
    }

    private static void InitStyles() {
        UnityButton = new GUIStyle(GUI.skin.button);
        UnityActiveButton = new GUIStyle(GUI.skin.button);
        UnityActiveButton.normal.background = UnityActiveButton.active.background;
    }
    private static void InitLists() {
        sprites = new List<string>() { "" };
        sprites.AddRange(SystemDatas.GetMapObjectsPaths());
        sounds = new List<string>() { "" };
        sounds.AddRange(SystemDatas.GetMusics());

        actionTypes = new string[] {
            "Select an Action to Add", 
            typeof(ActionIf).ToString(), 
            typeof(ActionAddItem).ToString(), 
            typeof(ActionAddMonster).ToString(), 
            typeof(ActionAleaMessage).ToString(), 
            typeof(ActionExecuteMapObjectActions).ToString(), 
            typeof(ActionEXP).ToString(), 
            typeof(ActionFadeScreen).ToString(), 
            typeof(ActionHeal).ToString(), 
            typeof(ActionMessage).ToString(), 
            typeof(ActionMonsterBattle).ToString(), 
            typeof(ActionMove).ToString(), 
            typeof(ActionPlaySound).ToString(), 
            typeof(ActionRemoveItem).ToString(), 
            typeof(ActionSave).ToString(),
            typeof(ActionSetState).ToString(),
            typeof(ActionSetVariable).ToString(), 
            typeof(ActionTeleport).ToString(), 
            typeof(ActionTransform).ToString(), 
            typeof(ActionWait).ToString()
        };
    }

    public void OnGUI() {
        indent = 0;

        if (target == null) {
            Close();
            return;
        }

        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        // Map Object infos
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3));
        
        
        GUILayout.BeginHorizontal();
        GUILayout.Label(target.Sprite, InterfaceUtility.CenteredStyle, GUILayout.Width(70), GUILayout.Height(70));

        GUILayout.BeginVertical();
        GUILayout.Space(5);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name", GUILayout.Width(100));
        target.name = EditorGUILayout.TextField(target.name);
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Sprite", GUILayout.Width(100));
        string val = sprites[EditorGUILayout.Popup((target.sprite == null ? 0 : sprites.IndexOf(target.spritePath)), sprites.ToArray())];
        if (val != target.spritePath) {
            target.spritePath = val;
            target.sprite = null;
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Orientation", GUILayout.Width(100));
        target.orientation = (MapObject.Orientation)EditorGUILayout.EnumPopup(target.orientation);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Layer", GUILayout.Width(100));
        target.layer = (MapObject.Layer)EditorGUILayout.EnumPopup(target.layer);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed", GUILayout.Width(100));
        target.speed = (MapObject.MovementSpeed)EditorGUILayout.EnumPopup(target.speed);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Allow Pass Through", GUILayout.Width(100));
        target.allowPassThrough = EditorGUILayout.Toggle(target.allowPassThrough);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Fixed Orientation", GUILayout.Width(100));
        target.fixedOrientation = EditorGUILayout.Toggle(target.fixedOrientation);
        GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();

        // Map Object actions
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3));
        GUILayout.Space(5);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("ExecCondition", GUILayout.Width(170));
        target.execCondition = (MapObject.ExecutionCondition)EditorGUILayout.EnumPopup(target.execCondition);
        GUILayout.EndHorizontal();

        if (target.execCondition == MapObject.ExecutionCondition.Distance || target.execCondition == MapObject.ExecutionCondition.DistanceFace) {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Distance", GUILayout.Width(170));
            target.actionDistance = EditorGUILayout.IntField(target.actionDistance);
            GUILayout.EndHorizontal();
        }

        scrollPosList = GUILayout.BeginScrollView(scrollPosList);
        GUILayout.BeginVertical();
        for (int i = 0; i < target.actions.Count; ++i) {
            MapObjectAction a = target.actions[i];
            GUILayout.BeginHorizontal();

            GUI.enabled = (i != 0) && a.GetType() != typeof(ActionIf) && a.GetType() != typeof(ConditionElse) && a.GetType() != typeof(ConditionEnd);
            if (GUILayout.Button("↑", GUILayout.Width(20))) {
                target.actions.Insert(i - 1, a);
                target.actions.RemoveAt(i + 1);
                if (selectedElement == i)
                    selectedElement--;
                else if (selectedElement == i - 1)
                    selectedElement++;
                GUIUtility.ExitGUI();
                return;
            }
            GUI.enabled = (i != target.actions.Count - 1) && a.GetType() != typeof(ActionIf) && a.GetType() != typeof(ConditionElse) && a.GetType() != typeof(ConditionEnd);
            if (GUILayout.Button("↓", GUILayout.Width(20))) {
                target.actions.Insert(i + 2, a);
                target.actions.RemoveAt(i);
                if (selectedElement == i)
                    selectedElement++;
                else if (selectedElement == i + 1)
                    selectedElement--;
                GUIUtility.ExitGUI();
                return;
            }
            GUI.enabled = true;

            bool isCondition = a.GetType() == typeof(ConditionElse) || a.GetType() == typeof(ConditionEnd);

            for (int k = 0; k < indent - (isCondition ? 1 : 0); ++k)
                GUILayout.Space(10);

            if (isCondition) {
                if (GUILayout.Button(a.InLine(), GUILayout.MaxWidth(Screen.width / 3))) {
                    if (a.GetType() == typeof(ConditionElse)) selectedElement = target.actions.IndexOf(((ConditionElse)a).parent);
                    else if (a.GetType() == typeof(ConditionEnd)) selectedElement = target.actions.IndexOf(((ConditionEnd)a).parent);
                }

                if (a.GetType() == typeof(ConditionEnd))
                    indent--;
            } else {
                if (GUILayout.Button((a.waitForEnd ? "* " : "") + a.InLine(), i == selectedElement ? UnityActiveButton : UnityButton, GUILayout.MaxWidth(Screen.width / 3))) {
                    selectedElement = i;

                if (a.GetType() == typeof(ActionIf)) {
                    indent++;
            }

            if (!isCondition && GUILayout.Button("X", GUILayout.Width(30)) && EditorUtility.DisplayDialog("Delete", "Delete this action ? (if will delete all internal actions until its end", "Ok", "Cancel") {
                if (a.GetType() == typeof(ActionIf)) {
                    while (target.actions[i].GetType() == typeof(ConditionEnd) && ((ConditionEnd)target.actions[i]).parent == a) {
                        target.actions.RemoveAt(i);
                    }
                } else
                    target.actions.Remove(a);
                }
                if (selectedElement == i)
                    selectedElement = -1;
                if (selectedElement > i)
                    selectedElement--;
                GUIUtility.ExitGUI();
                return;
            }
            GUILayout.EndHorizontal();

        }

        int value = EditorGUILayout.Popup(0, actionTypes);
        if (value != 0) {
            MapObjectAction moa = System.Activator.CreateInstance(Types.GetType(actionTypes[value], "Assembly-CSharp")) as MapObjectAction;
            target.actions.Add(moa);
            selectedElement = target.actions.Count - 1;

            if (Types.GetType(actionTypes[value], "Assembly-CSharp") == typeof(ActionIf)) {
                target.actions.Add(new ConditionElse((ActionIf)moa));
                target.actions.Add(new ConditionEnd((ActionIf)moa));
            }
                

            if (Types.GetType(actionTypes[value], "Assembly-CSharp") == typeof(ActionMove))
                (target.actions[selectedElement] as ActionMove).targetId = target.mapObjectId;
        }
        
        GUILayout.EndVertical();
        GUILayout.EndScrollView();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        scrollPosEdit = GUILayout.BeginScrollView(scrollPosEdit);
        GUILayout.BeginVertical();
        if (selectedElement >= 0 && selectedElement < target.actions.Count) {
            EditDisplay(target.actions[selectedElement]);
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();

        if (GUILayout.Button("OK")) {
            Close();
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    public void OnDestroy() {
        isOpen = false;
    }

    public void EditDisplay(MapObjectAction _action) {
        _action.waitForEnd = EditorGUILayout.ToggleLeft("Wait For End", _action.waitForEnd);

        switch (_action.GetType().ToString()) {
            case "ActionIf":
                DisplayEditor((ActionIf)_action); break;
            case "ActionAddItem":
                DisplayEditor((ActionAddItem)_action); break;
            case "ActionAddMonster":
                DisplayEditor((ActionAddMonster)_action); break;
            case "ActionAleaMessage":
                DisplayEditor((ActionAleaMessage)_action); break;
            case "ActionExecuteMapObjectActions":
                DisplayEditor((ActionExecuteMapObjectActions)_action); break;
            case "ActionEXP":
                DisplayEditor((ActionEXP)_action); break;
            case "ActionFadeScreen":
                DisplayEditor((ActionFadeScreen)_action); break;
            case "ActionHeal":
                DisplayEditor((ActionHeal)_action); break;
            case "ActionMessage":
                DisplayEditor((ActionMessage)_action); break;
            case "ActionMonsterBattle":
                DisplayEditor((ActionMonsterBattle)_action); break;
            case "ActionMove":
                DisplayEditor((ActionMove)_action); break;
            case "ActionPlaySound":
                DisplayEditor((ActionPlaySound)_action); break;
            case "ActionRemoveItem":
                DisplayEditor((ActionRemoveItem)_action); break;
            case "ActionSave":
                DisplayEditor((ActionSave)_action); break;
            case "ActionSetState":
                DisplayEditor((ActionSetState)_action); break;
            case "ActionSetVariable":
                DisplayEditor((ActionSetVariable)_action); break;
            case "ActionTeleport":
                DisplayEditor((ActionTeleport)_action); break;
            case "ActionTransform":
                DisplayEditor((ActionTransform)_action); break;
            case "ActionWait":
                DisplayEditor((ActionWait)_action); break;
            default:
                GUILayout.Label("Unknown Action " + _action.GetType());
                return;
        }
    }


    private void DisplayEditor(ActionIf a) {
        a.conditionType = (ActionIf.ConditionType)EditorGUILayout.EnumPopup("Type", a.conditionType);


    } 
    private void DisplayEditor(ActionAddItem a) { 
        GUILayout.Label("TODO : Display a popup with all items");
    }
    private void DisplayEditor(ActionAddMonster a) {
        a.patternID = UtilityEditor.MonsterPatternField("MonsterPattern", a.patternID);

        a.lvl = EditorGUILayout.IntField("Level", a.lvl);
    }
    private void DisplayEditor(ActionAleaMessage a) {
        List<string> faces = SystemDatas.GetFaces();
        faces.Insert(0, "No Image");

        GUILayout.BeginHorizontal();
        int current = a.face == null ? 0 : faces.IndexOf(a.face.name + ".png");
        int choice = EditorGUILayout.Popup(current, faces.ToArray());
        if (choice != current) {
            if (choice == 0)
                a.face = null;
            else
                a.face = InterfaceUtility.GetTexture(Config.GetResourcePath(ActionMessage.IMAGE_FOLDER) + faces[choice]);
        }

        if (a.face != null)
            GUILayout.Label(a.face, GUILayout.Width(60), GUILayout.Height(60));
        GUILayout.EndHorizontal();

        if (a.face != null) a.faceOnRight = EditorGUILayout.Toggle("Face On Right", a.faceOnRight);
    }
    private void DisplayEditor(ActionExecuteMapObjectActions a) {
        a.mapObjectId = UtilityEditor.MapObjectField("Target", a.mapObjectId, false);
    }
    private void DisplayEditor(ActionEXP a) {
        a.targetMonster = Mathf.Clamp(EditorGUILayout.IntField("Monster", a.targetMonster), -1, 6);

        a.expValue = EditorGUILayout.IntField("Exp value", a.expValue);
    }
    private void DisplayEditor(ActionFadeScreen a) {
        a.color = EditorGUILayout.ColorField("Color", a.color);
        a.duration = EditorGUILayout.FloatField("Duration", a.duration);
    }
    private void DisplayEditor(ActionHeal a) {
        a.targetMonster = Mathf.Clamp(EditorGUILayout.IntField("Monster", a.targetMonster), -1, 6);

        a.healValue = EditorGUILayout.IntField("Heal value", a.healValue);
    }
    private void DisplayEditor(ActionMessage a) {
        List<string> faces = SystemDatas.GetFaces();
        faces.Insert(0, "No Image");

        GUILayout.BeginHorizontal();
        int current = a.face == null ? 0 : faces.IndexOf(a.face.name + ".png");
        int choice = EditorGUILayout.Popup(current, faces.ToArray());
        if (choice != current) {
            if (choice == 0)
                a.face = null;
            else
                a.face = InterfaceUtility.GetTexture(Config.GetResourcePath(ActionMessage.IMAGE_FOLDER) + faces[choice]);
        }

        if (a.face != null)
            GUILayout.Label(a.face, GUILayout.Width(60), GUILayout.Height(60));
        GUILayout.EndHorizontal();

        if (a.face != null) a.faceOnRight = EditorGUILayout.Toggle("Face On Right", a.faceOnRight);
        a.message = EditorGUILayout.TextField("Message", a.message);
        a.maxDuration = EditorGUILayout.FloatField("Max Duration", a.maxDuration);
    }
    private void DisplayEditor(ActionMonsterBattle a) {
        for (int i = 0; i < a.monsters.Count; i++) {
            GUILayout.BeginHorizontal();
            GUI.enabled = (i != 0);
            if (GUILayout.Button("↑", GUILayout.Width(20))) {
                a.monsters.Insert(i - 1, a.monsters[i]);
                a.monsters.RemoveAt(i + 1);
                GUIUtility.ExitGUI();
                return;
            }
            GUI.enabled = (i != a.monsters.Count - 1);
            if (GUILayout.Button("↓", GUILayout.Width(20))) {
                a.monsters.Insert(i + 2, a.monsters[i]);
                a.monsters.RemoveAt(i);
                GUIUtility.ExitGUI();
                return;
            }
            GUI.enabled = true;

            GUILayout.Label(i + ":", GUILayout.Width(22));
            a.monsters[i].patternId = UtilityEditor.MonsterPatternField("", a.monsters[i].patternId);
            GUILayout.Label("Lvl", GUILayout.Width(30));
            a.monsters[i].lvlMin = EditorGUILayout.IntField(a.monsters[i].lvlMin, GUILayout.Width(40));
            GUILayout.Label("-", GUILayout.Width(10));
            a.monsters[i].lvlMax = EditorGUILayout.IntField(a.monsters[i].lvlMax, GUILayout.Width(40));

            if (GUILayout.Button("X", GUILayout.Width(30)))
                a.monsters.RemoveAt(i);
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUI.enabled = a.monsters.Count < 6;
        if (GUILayout.Button("+"))
            a.monsters.Add(new ActionMonsterBattle.EncounterMonster());
        GUI.enabled = true;
        GUILayout.EndHorizontal();
    }
    private void DisplayEditor(ActionMove a) {
        int value = UtilityEditor.MapObjectField("Target", a.targetId, true);
        if (value != a.targetId) {
            a.targetId = value;
            a.target = World.Current.GetMapObjectById(a.targetId);
        }
        

        for (int i = 0; i < a.movements.Count; i++) {
            GUILayout.BeginHorizontal();
            GUI.enabled = (i != 0);
            if (GUILayout.Button("↑", GUILayout.Width(20))) {
                a.movements.Insert(i - 1, a.movements[i]);
                a.movements.RemoveAt(i + 1);
                GUIUtility.ExitGUI();
                return;
            }
            GUI.enabled = (i != a.movements.Count - 1);
            if (GUILayout.Button("↓", GUILayout.Width(20))) {
                a.movements.Insert(i + 2, a.movements[i]);
                a.movements.RemoveAt(i);
                GUIUtility.ExitGUI();
                return;
            }
            GUI.enabled = true;

            GUILayout.Label(i + ":", GUILayout.Width(22));
            a.movements[i] = (MapObject.PossibleMovement)EditorGUILayout.EnumPopup(a.movements[i]);
            if (GUILayout.Button("X", GUILayout.Width(30)))
                a.movements.RemoveAt(i);
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+"))
            a.movements.Add(MapObject.PossibleMovement.Forward);
        GUILayout.EndHorizontal();
    }
    private void DisplayEditor(ActionPlaySound a) {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Sound", GUILayout.Width(100));
        int index = sounds.IndexOf(a.soundPath);
        if (index < 0) { index = 0; }
        a.soundPath = sounds[EditorGUILayout.Popup(index, sounds.ToArray())];
        GUILayout.EndHorizontal();
        a.mode = (ActionPlaySound.SoundType)EditorGUILayout.EnumPopup("Mode", a.mode);
    }
    private void DisplayEditor(ActionRemoveItem a) {
        GUILayout.Label("TODO : Display a popup with all items");
    }
    private void DisplayEditor(ActionSave a) {
        
    }
    private void DisplayEditor(ActionSetState a) {
        GUILayout.BeginHorizontal();
        a.stateId = UtilityEditor.StateField("State", a.stateId);
        a.value = EditorGUILayout.Toggle(a.value);
        GUILayout.EndHorizontal();
    }
    private void DisplayEditor(ActionSetVariable a) {
        a.varId = UtilityEditor.VariableField("Variable", a.varId);

        a.setMode = (ActionSetVariable.SetMode)EditorGUILayout.EnumPopup("Set Mode", a.setMode);
        
        GUI.changed = false;
        a.mode = (ActionSetVariable.Mode)EditorGUILayout.EnumPopup("Mode", a.mode);
        if (GUI.changed) {
            a.value = -1;
            a.value2 = -1;
            a.linked = null;
        }

        switch (a.mode) {
            case ActionSetVariable.Mode.Random:
                GUILayout.BeginHorizontal();
                GUILayout.Label("Min", GUILayout.Width(50));
                a.value = EditorGUILayout.IntField(a.value);
                GUILayout.Label("Max", GUILayout.Width(50));
                a.value2 = EditorGUILayout.IntField(a.value2);
                GUILayout.EndHorizontal();
                break;
            case ActionSetVariable.Mode.Variable:
                a.value = UtilityEditor.VariableField("Source", a.value); break;
            case ActionSetVariable.Mode.MOPositionX:
            case ActionSetVariable.Mode.MOPositionY:
            case ActionSetVariable.Mode.MOOrientation:
                a.value = UtilityEditor.MapObjectField("MapObject", a.value, true); break;
            case ActionSetVariable.Mode.Gold:
            case ActionSetVariable.Mode.TeamCount:
            case ActionSetVariable.Mode.CollectionCount:
            case ActionSetVariable.Mode.EncouteredCount:
                break;
            case ActionSetVariable.Mode.MonsterLevel:
            case ActionSetVariable.Mode.MonsterPattern:
            case ActionSetVariable.Mode.MonsterExpRequired:
                a.value = Mathf.Clamp(EditorGUILayout.IntField("Monster", a.value), 0, 6); break;
            case ActionSetVariable.Mode.MonsterIndex:
                a.value = UtilityEditor.MonsterPatternField("Pattern", a.value); break;
        }
    }
    private void DisplayEditor(ActionTeleport a) {
        a.mapObjectId = UtilityEditor.MapObjectField("Target : ", a.mapObjectId, true);

        GUI.changed = false;
        a.mapID = UtilityEditor.MapField("Map", a.mapID);
        if (GUI.changed) {
            displayMap.Dispose();
            displayMap = null;
        }

        if (displayMap == null && Map.Exists(a.mapID)) {
            displayMap = new Map(a.mapID);
            displayMap.Load();
        }

        if (displayMap != null) {
            int width = Mathf.RoundToInt(Screen.width / 3f) - 10;
            int height = Mathf.RoundToInt(width * 9 / 16f);

            GUI.BeginGroup(GUILayoutUtility.GetRect(width, height));

            Vector2 resolution = new Vector2(width / (int)displayMap.Size.x, height / (int)displayMap.Size.y);

            // Map
            for (int layer = 0; layer < 3; layer++) {
                foreach (Map.Tile tile in displayMap.GetTiles(layer)) {
                    if (tile.Image != null) {
                        GUI.DrawTexture(new Rect(resolution.x * tile.mapCoords.x, resolution.y * tile.mapCoords.y, resolution.x, resolution.y), tile.Image);
                        if (GUI.Button(new Rect(resolution.x * tile.mapCoords.x, resolution.y * tile.mapCoords.y, resolution.x, resolution.y), "", InterfaceUtility.EmptyStyle)) {
                            a.arrival = tile.mapCoords;
                        }
                    }
                }
            }
            for (int i = 1; i < displayMap.Size.x; i++)
                EditorGUI.DrawRect(new Rect(i * resolution.x - 1, 0, 1, height), Color.black);
            for (int i = 1; i < displayMap.Size.y; i++)
                EditorGUI.DrawRect(new Rect(0, i * resolution.y - 1, width, 1), Color.black);

            foreach (MapObject mo in displayMap.mapObjects) {
                UtilityEditor.DrawEmptyRect(new Rect(resolution.x * mo.mapCoords.x, resolution.y * mo.mapCoords.y, resolution.x - 1, resolution.y - 1), Mathf.RoundToInt(resolution.x / 5f), Color.gray);
            }

            UtilityEditor.DrawEmptyRect(new Rect(resolution.x * a.arrival.x, resolution.y * a.arrival.y, resolution.x - 1, resolution.y - 1), Mathf.RoundToInt(resolution.x / 4f), Color.red);

            GUI.EndGroup();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Arrival", GUILayout.Width(80));
        GUILayout.Label("X", GUILayout.Width(20));
        a.arrival.x = Mathf.Clamp(EditorGUILayout.IntField((int)a.arrival.x), 0, displayMap.Size.x);
        GUILayout.Label("Y", GUILayout.Width(20));
        a.arrival.y = Mathf.Clamp(EditorGUILayout.IntField((int)a.arrival.y), 0, displayMap.Size.y);
        GUILayout.EndHorizontal();
        a.orientation = (MapObject.Orientation)EditorGUILayout.EnumPopup("Orientation", a.orientation);
    }
    private void DisplayEditor(ActionWait a) {
        a.duration = Mathf.Max(0, EditorGUILayout.FloatField("Time", a.duration));
    }
    private void DisplayEditor(ActionTransform a) {
        List<string> mapObjectList = new List<string>(){"Player"};
        foreach(MapObject mo in World.Current.currentMap.mapObjects){
            mapObjectList.Add(mo.mapObjectId + ": " + mo.name);
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Target", GUILayout.Width(100));
        int index = mapObjectList.FindIndex(S => S.StartsWith(a.mapObjectId+":"));
        if (index < 0) { index = 0; }
        string val = mapObjectList[EditorGUILayout.Popup(index, mapObjectList.ToArray())];
        if (val == "Player") { a.mapObjectId = -1; } else { a.mapObjectId = int.Parse(val.Substring(0, val.IndexOf(":")));  }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sprite", GUILayout.Width(100));
        val = sprites[EditorGUILayout.Popup(sprites.IndexOf(a.newSpritePath), sprites.ToArray())];
        if (val != a.newSpritePath) {
            a.newSpritePath = val;
        }
        GUILayout.EndHorizontal();
    }
}
