using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class VariableViewer : EditorWindow {
    public const int NB_COLUMNS = 2;

    public List<DBState> states = new List<DBState>();
    public Vector2 scrollpos_state;
    public List<DBVariable> variables = new List<DBVariable>();
    public Vector2 scrollpos_var;
    public List<DBMonsterPattern> patterns = new List<DBMonsterPattern>();
    public Vector2 scrollpos_monsterCollection;
    public List<DBMonster> monsters = new List<DBMonster>();
    public Vector2 scrollpos_monsterCaptured;

    private DBMonsterPattern selectedPattern = null;
    private DBMonster selectedMonster = null;

    [MenuItem("Tools/ElementsViewer")]
    public static void Init() {
        VariableViewer window = EditorWindow.GetWindow<VariableViewer>();
        window.minSize = new Vector2(500, 400);

        window.Show();

        InterfaceUtility.ClearAllCache();
    }

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


        GUILayout.BeginVertical(GUILayout.Width(Screen.width / NB_COLUMNS));
        GUILayout.Label("Patterns", InterfaceUtility.TitleStyle);
        scrollpos_var = GUILayout.BeginScrollView(scrollpos_var);
        
        foreach (DBMonsterPattern pattern in patterns) {
            GUILayout.BeginHorizontal();
            
            Color c = GUI.color;
            GUI.color = MonsterCollection.isAlreadyCaptured(pattern) ? Color.white : pattern.encountered ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.6f, 0.6f, 0.6f);
            GUILayout.Label(InterfaceUtility.IntString(pattern.ID, 3) + " : ", GUILayout.Width(40));
            if (GUILayout.Button(pattern.name)) {
                Select(pattern);
            }
            GUI.color = c;
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(GUILayout.Width(Screen.width / NB_COLUMNS));
        GUILayout.Label("Monsters" + (selectedPattern == null ? "" : " (" + selectedPattern.name + ")"), InterfaceUtility.TitleStyle);
        scrollpos_var = GUILayout.BeginScrollView(scrollpos_var);

        foreach (DBMonster monster in monsters) {
            GUILayout.BeginHorizontal();

            GUILayout.Label(InterfaceUtility.IntString(monster.ID, 3) + " : ", GUILayout.Width(40));
            if (GUILayout.Button(monster.nickName + " (lvl " + monster.lvl + ")")) {
                Select(monster);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.BeginVertical(GUILayout.Height(150));
        if (selectedMonster != null) {
            DBMonsterPattern pattern = DataBase.SelectById<DBMonsterPattern>(selectedMonster.patternId);
            GUILayout.BeginHorizontal();
            GUILayout.Label(pattern.battleSprite);
            GUILayout.BeginVertical();
            GUILayout.Label(selectedMonster.nickName);
            GUILayout.Label("STG : " + selectedMonster.stat_might);
            GUILayout.Label("RES : " + selectedMonster.stat_resistance);
            GUILayout.Label("SPD : " + selectedMonster.stat_speed);
            GUILayout.Label("LUK : " + selectedMonster.stat_luck);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (selectedMonster.attack1 != -1) {
                DBAttack attack = DataBase.SelectById<DBAttack>(selectedMonster.attack1);
                GUI.color = Utility.GetColorFromType(attack.type);
                GUILayout.Label(attack.name + " (" + attack.staminaCost + ") : " + attack.power + "(" + attack.accuracy + "%)");
            } else {
                GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
                GUILayout.Label("");
            }
            if (selectedMonster.attack2 != -1) {
                DBAttack attack = DataBase.SelectById<DBAttack>(selectedMonster.attack2);
                GUI.color = Utility.GetColorFromType(attack.type);
                GUILayout.Label(attack.name + " (" + attack.staminaCost + ") : " + attack.power + "(" + attack.accuracy + "%)");
            } else {
                GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
                GUILayout.Label("");
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (selectedMonster.attack3 != -1) {
                DBAttack attack = DataBase.SelectById<DBAttack>(selectedMonster.attack3);
                GUI.color = Utility.GetColorFromType(attack.type);
                GUILayout.Label(attack.name + " (" + attack.staminaCost + ") : " + attack.power + "(" + attack.accuracy + "%)");
            } else {
                GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
                GUILayout.Label("");
            }
            if (selectedMonster.attack4 != -1) {
                DBAttack attack = DataBase.SelectById<DBAttack>(selectedMonster.attack4);
                GUI.color = Utility.GetColorFromType(attack.type);
                GUILayout.Label(attack.name + " (" + attack.staminaCost + ") : " + attack.power + "(" + attack.accuracy + "%)");
            } else {
                GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.4f);
                GUILayout.Label("");
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    public void Refresh() {
        if (!DataBase.IsConnected) 
            DataBase.Connect(Application.dataPath + (Application.isPlaying ? "/current.sql" : "/database.sql"));
        
        states = DataBase.Select<DBState>();
        variables = DataBase.Select<DBVariable>();
        patterns = DataBase.Select<DBMonsterPattern>();
        Select(selectedPattern);
    }

    public void Select(DBMonsterPattern pattern) {
        if (pattern == null)
            monsters = DataBase.Select<DBMonster>();
        else
            monsters = DataBase.Select<DBMonster>("patternId=" + pattern.ID);
        selectedPattern = pattern;
        selectedMonster = null;
    }
    public void Select(DBMonster monster) {
        selectedMonster = monster;
    }
}
