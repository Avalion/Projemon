using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapObjectKreator : EditorWindow {
    public static bool isOpen = false;
    
    private MapObject m_target = null;

    private int selectedElement = -1;

    public static void Open(MapObject mo) {
        if (mo == null)
            return;
        MapObjectKreator window = EditorWindow.GetWindow<MapObjectKreator>();
        window.minSize = new Vector2(1000, 500);
        window.maxSize = new Vector2(1000, 501);
        window.m_target = mo;
        window.Show();

        isOpen = true;
    }

    Vector2 scrollPosList;
    Vector2 scrollPosEdit;

    string[] types = new string[] { 
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
    
    public void OnGUI() {
        if (m_target == null) {
            Close();
            return;
        }

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3));
        scrollPosList = GUILayout.BeginScrollView(scrollPosList);
        GUILayout.BeginVertical();
        for (int i = 0; i < m_target.actions.Count; ++i) {
            MapObjectAction a = m_target.actions[i];
            GUILayout.BeginHorizontal();
            if (GUILayout.Button((a.waitForEnd ? "*" : "") + a.InLine(), GUILayout.Width(Screen.width / 3 - 100))) {
                selectedElement = i;
            }
            if (GUILayout.Button("X")) {
                m_target.actions.Remove(a);
                if (selectedElement == i)
                    selectedElement = -1;
                GUIUtility.ExitGUI();
                return;
            }
            GUILayout.EndHorizontal();

        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();

        int value = EditorGUILayout.Popup(0, types);
        if (value != 0) {
            m_target.actions.Add(System.Activator.CreateInstance(Types.GetType(types[value], "Assembly-CSharp")) as MapObjectAction);
            selectedElement = m_target.actions.Count - 1;
        }
        GUILayout.EndVertical();

        scrollPosEdit = GUILayout.BeginScrollView(scrollPosEdit);
        GUILayout.BeginVertical();
        if (selectedElement > 0 && selectedElement < m_target.actions.Count) {
            EditDisplay(m_target.actions[selectedElement]);
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
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
                break;
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
        a.face = EditorGUILayout.ObjectField("Face", a.face, typeof(Texture2D), false) as Texture2D;
        a.faceOnRight = EditorGUILayout.Toggle("Face On Right", a.faceOnRight);
        a.message = EditorGUILayout.TextField("Message", a.message);
        a.maxDuration = EditorGUILayout.FloatField("Max Duration", a.maxDuration);
    }
    private void DisplayEditor(ActionMonsterBattle a) {
        GUILayout.Label("TODO : Display a list of monsterPattern : cf DisplayEditor(ActionMove)");
    }
    private void DisplayEditor(ActionMove a) {
        for (int i = 0; i < a.movements.Count; i++) {
            GUILayout.BeginHorizontal();
            a.movements[i] = (MapObject.PossibleMovement)EditorGUILayout.EnumPopup(i + ":", a.movements[i]);
            if (GUILayout.Button("X"))
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
        GUILayout.Label("TODO : propose list of files into BGM or BGS folder");
        //a.soundPath = EditorGUILayout.ObjectField("Sound", a.soundPath, typeof(AudioClip), false) as AudioClip;
        a.bgm = EditorGUILayout.Toggle("BGM", a.bgm);
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
        GUILayout.Label("TODO : Choisir une map, lister les mapObjects associés et pouvoir choisir une position et une orientation");
    }
    private void DisplayEditor(ActionWait a) {
        a.duration = Mathf.Max(0, EditorGUILayout.FloatField("Time", a.duration));
    }
    private void DisplayEditor(ActionTransform a) {
        GUILayout.Label("TODO : Display a popup with all MapObject");
    }
}
