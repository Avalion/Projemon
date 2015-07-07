using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class VariableKreator : EditorWindow {
    public const int NB_COLUMNS = 2;

    public List<DBState> states = new List<DBState>();
    public Vector2 scrollpos_state;
    public List<DBVariable> variables = new List<DBVariable>();
    public Vector2 scrollpos_var;
    
    [MenuItem("Kreator/VariableKreator")]
    public static void Init() {
        VariableKreator window = EditorWindow.GetWindow<VariableKreator>();
        window.minSize = new Vector2(500, 400);

        window.Show();

        InterfaceUtility.ClearAllCache();
    }

    [MenuItem("Kreator/VariableKreator", true)]
    public static bool InitOK() { return !Application.isPlaying; }

    public void OnEnable() {
        Refresh();
    }

    public void OnGUI() {
        string str;
        bool boolval;
        int intval;

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(Screen.width / NB_COLUMNS));
        GUILayout.Label("States", InterfaceUtility.TitleStyle);
        scrollpos_state = GUILayout.BeginScrollView(scrollpos_state);

        foreach (DBState state in states) {
            GUILayout.BeginHorizontal();
            GUILayout.Label(InterfaceUtility.IntString(state.ID, 3) + " : ");
            str = GUILayout.TextField(state.name);
            if (str != state.name) {
                state.name = str;
                DataBaseEditorUtility.SetState(state.ID, str);
            }
            boolval = EditorGUILayout.Toggle(state.value);
            if (boolval != state.value) {
                state.value = boolval;
                DataBaseEditorUtility.SetState(state.ID, boolval);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(GUILayout.Width(Screen.width / NB_COLUMNS));
        GUILayout.Label("Variables", InterfaceUtility.TitleStyle);
        scrollpos_var = GUILayout.BeginScrollView(scrollpos_var);

        foreach (DBVariable variable in variables) {
            GUILayout.BeginHorizontal();
            GUILayout.Label(InterfaceUtility.IntString(variable.ID, 3) + " : ", GUILayout.Width(40));
            str = GUILayout.TextField(variable.name);
            if (str != variable.name) {
                variable.name = str;
                DataBaseEditorUtility.SetVariable(variable.ID, str);
            }
            intval = EditorGUILayout.IntField(variable.value);
            if (intval != variable.value) {
                variable.value = intval;
                DataBaseEditorUtility.SetVariable(variable.ID, intval);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    public void Refresh() {
        if (!DataBase.IsConnected) 
            DataBase.Connect(Application.dataPath + "/database.sql");
        
        states = DataBase.Select<DBState>();
        variables = DataBase.Select<DBVariable>();
    }
}
