using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapObjectKreator : EditorWindow {
    public static bool isOpen = false;
    
    private MapObject m_target = null;

    private int selectedElement = -1;

    private static List<string> sprites = new List<string>();
    private static List<string> sounds = new List<string>();
    private static string[] actionTypes;
    
    private Vector2 scrollPosList;
    private Vector2 scrollPosEdit;

    private static GUIStyle UnityButton;
    private static GUIStyle UnityActiveButton;


    public static void Open(MapObject mo) {
        if (mo == null)
            return;
        MapObjectKreator window = EditorWindow.GetWindow<MapObjectKreator>();
        window.minSize = new Vector2(1000, 500);
        window.maxSize = new Vector2(1000, 501);
        window.m_target = mo;
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
            typeof(ActionSetState).ToString(),
            typeof(ActionSetVariable).ToString(), 
            typeof(ActionTeleport).ToString(), 
            typeof(ActionTransform).ToString(), 
            typeof(ActionWait).ToString()
        };
    }

    public void OnGUI() {
        if (m_target == null) {
            Close();
            return;
        }

        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        // Map Object infos
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3));
        
        
        GUILayout.BeginHorizontal();
        GUILayout.Label(m_target.Sprite, InterfaceUtility.CenteredStyle, GUILayout.Width(70), GUILayout.Height(70));

        GUILayout.BeginVertical();
        GUILayout.Space(5);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name", GUILayout.Width(100));
        m_target.name = EditorGUILayout.TextField(m_target.name);
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Sprite", GUILayout.Width(100));
        string val = sprites[EditorGUILayout.Popup((m_target.sprite == null ? 0 : sprites.IndexOf(m_target.spritePath)), sprites.ToArray())];
        if (val != m_target.spritePath) {
            m_target.spritePath = val;
            m_target.sprite = null;
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("Orientation", GUILayout.Width(100));
        m_target.orientation = (MapObject.Orientation)EditorGUILayout.EnumPopup(m_target.orientation);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Layer", GUILayout.Width(100));
        m_target.layer = (MapObject.Layer)EditorGUILayout.EnumPopup(m_target.layer);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed", GUILayout.Width(100));
        m_target.speed = (MapObject.MovementSpeed)EditorGUILayout.EnumPopup(m_target.speed);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Allow Pass Through", GUILayout.Width(100));
        m_target.allowPassThrough = EditorGUILayout.Toggle(m_target.allowPassThrough);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Fixed Orientation", GUILayout.Width(100));
        m_target.fixedOrientation = EditorGUILayout.Toggle(m_target.fixedOrientation);
        GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();

        // Map Object actions
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3));
        GUILayout.Space(5);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("ExecCondition", GUILayout.Width(170));
        m_target.execCondition = (MapObject.ExecutionCondition)EditorGUILayout.EnumPopup(m_target.execCondition);
        GUILayout.EndHorizontal();

        if (m_target.execCondition == MapObject.ExecutionCondition.Distance || m_target.execCondition == MapObject.ExecutionCondition.DistanceFace) {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Distance", GUILayout.Width(170));
            m_target.actionDistance = EditorGUILayout.IntField(m_target.actionDistance);
            GUILayout.EndHorizontal();
        }

        scrollPosList = GUILayout.BeginScrollView(scrollPosList);
        GUILayout.BeginVertical();
        for (int i = 0; i < m_target.actions.Count; ++i) {
            MapObjectAction a = m_target.actions[i];
            GUILayout.BeginHorizontal();
            
            GUI.enabled = (i != 0);
            if (GUILayout.Button("↑", GUILayout.Width(20))) {
                m_target.actions.Insert(i - 1, a);
                m_target.actions.RemoveAt(i + 1);
                if (selectedElement == i)
                    selectedElement--;
                else if (selectedElement == i - 1)
                    selectedElement++;
                GUIUtility.ExitGUI();
                return;
            }
            GUI.enabled = (i != m_target.actions.Count - 1);
            if (GUILayout.Button("↓", GUILayout.Width(20))) {
                m_target.actions.Insert(i + 2, a);
                m_target.actions.RemoveAt(i);
                if (selectedElement == i)
                    selectedElement++;
                else if (selectedElement == i + 1)
                    selectedElement--;
                GUIUtility.ExitGUI();
                return;
            }
            GUI.enabled = true;

            if (GUILayout.Button((a.waitForEnd ? "* " : "") + a.InLine(), i == selectedElement ? UnityActiveButton : UnityButton, GUILayout.MaxWidth(Screen.width / 3))) {
                selectedElement = i;
            }
            if (GUILayout.Button("X", GUILayout.Width(30))) {
                m_target.actions.Remove(a);
                if (selectedElement == i)
                    selectedElement = -1;
                GUIUtility.ExitGUI();
                return;
            }
            GUILayout.EndHorizontal();

        }

        int value = EditorGUILayout.Popup(0, actionTypes);
        if (value != 0) {
            m_target.actions.Add(System.Activator.CreateInstance(Types.GetType(actionTypes[value], "Assembly-CSharp")) as MapObjectAction);
            selectedElement = m_target.actions.Count - 1;

            if (Types.GetType(actionTypes[value], "Assembly-CSharp") == typeof(ActionMove))
                (m_target.actions[selectedElement] as ActionMove).targetId = m_target.mapObjectId;
        }
        
        GUILayout.EndVertical();
        GUILayout.EndScrollView();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        scrollPosEdit = GUILayout.BeginScrollView(scrollPosEdit);
        GUILayout.BeginVertical();
        if (selectedElement >= 0 && selectedElement < m_target.actions.Count) {
            EditDisplay(m_target.actions[selectedElement]);
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

    private void DisplayEditor(ActionAddItem a) { 
        GUILayout.Label("TODO : Display a popup with all items");
    }
    private void DisplayEditor(ActionAddMonster a) { 
        GUILayout.Label("TODO : display a popup with all monsterpattern");
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
        GUILayout.Label("TODO : Display a popup with all MapObject");
    }
    private void DisplayEditor(ActionEXP a) {
        GUILayout.Label("TODO : Display a popup with all Battler and its Monsters");
    }
    private void DisplayEditor(ActionFadeScreen a) {
        a.color = EditorGUILayout.ColorField("Color", a.color);
        a.duration = EditorGUILayout.FloatField("Duration", a.duration);
    }
    private void DisplayEditor(ActionHeal a) {
        GUILayout.Label("TODO : Display a popup with all Battler and its Monsters");
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
        GUILayout.Label("TODO : Display a list of monsterPattern : cf DisplayEditor(ActionMove)");
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
    private void DisplayEditor(ActionSetVariable a) {
        GUILayout.Label("TODO : Display a popup with all Variables and modes");
    }
    private void DisplayEditor(ActionRemoveItem a) {
        GUILayout.Label("TODO : Display a popup with all items");
    }
    private void DisplayEditor(ActionSetState a) {
        GUILayout.Label("TODO : Display a popup with all States");
    }
    private void DisplayEditor(ActionTeleport a) {
        a.mapObjectId = UtilityEditor.MapObjectField("Target : ", a.mapObjectId, false);

        GUILayout.Label("TODO : Choisir une map à afficher, pouvoir choisir une position et une orientation");

        a.mapID = EditorGUILayout.IntField("Map", a.mapID);
        a.arrival = EditorGUILayout.Vector2Field("Arrival", a.arrival);
        a.orientation = (MapObject.Orientation)EditorGUILayout.EnumPopup(a.orientation);
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
