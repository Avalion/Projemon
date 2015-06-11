using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor {
    
    private int _selectedMonster = -1;

    private bool[] foldouts = new bool[3];

    private GUIStyle _unknownStyle;
    private GUIStyle _encouteredStyle;
    private GUIStyle _capturedStyle;


    public void OnEnable() {
        _unknownStyle = new GUIStyle(InterfaceUtility.EmptyStyle);
        _unknownStyle.normal.textColor = new Color(0.4f, 0.4f, 0.4f);
        _encouteredStyle = new GUIStyle(InterfaceUtility.EmptyStyle);
        _encouteredStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f); ;
        _capturedStyle = new GUIStyle(InterfaceUtility.EmptyStyle);
        _capturedStyle.normal.textColor = new Color(1f, 1f, 1f);;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GUILayout.Space(5);


        if (!Application.isPlaying)
            return;

        EditorGUILayout.LabelField("Statistiques : ", InterfaceUtility.CenteredStyle);

        #region World
        foldouts[0] = EditorGUILayout.Foldout(foldouts[0], "World");
        if (foldouts[0]) {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Position");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Map " + World.Current.currentMap + " " + Player.Current.mapCoords);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Music ");
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("BGM");
            GUILayout.FlexibleSpace();
            GUILayout.Label(World.Current.BGM ? World.Current.BGM.name : "\"No Music\"");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("BGS");
            GUILayout.FlexibleSpace();
            GUILayout.Label(World.Current.BGS ? World.Current.BGS.name : "\"No Sound\"");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion


        #region Player
        foldouts[1] = EditorGUILayout.Foldout(foldouts[1], "Player");
        if (foldouts[1]) {
            GUIContent[] contents = new GUIContent[6];
            for (int i = 0; i < 6; ++i) {
                if (Player.Current.monsters.Count <= i)
                    contents[i] = new GUIContent("No monster");
                else {
                    Monster m = Player.Current.monsters[i];
                    contents[i] = new GUIContent(m.monsterName + (m.monsterName != m.monsterPattern.name ? " (" + m.monsterPattern.name + ")" : ""), Player.Current.monsters[i].miniSprite);
                }
            }

            _selectedMonster = GUILayout.SelectionGrid(_selectedMonster, contents, 2, GUILayout.Height(150));

            if (_selectedMonster != -1 && Player.Current.monsters.Count > _selectedMonster) {
                Monster m = Player.Current.monsters[_selectedMonster];

                GUILayout.BeginHorizontal();
                GUILayout.Label("PV");
                GUILayout.FlexibleSpace();
                GUILayout.Label(m.life + " / " + m.maxLife);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Stamina");
                GUILayout.FlexibleSpace();
                GUILayout.Label(m.stamina + " / " + m.maxStamina);
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Mgt", InterfaceUtility.CenteredStyle, GUILayout.Width((Screen.width - 30) / 4));
                GUILayout.Label("Res", InterfaceUtility.CenteredStyle, GUILayout.Width((Screen.width - 30) / 4));
                GUILayout.Label("Lck", InterfaceUtility.CenteredStyle, GUILayout.Width((Screen.width - 30) / 4));
                GUILayout.Label("Spd", InterfaceUtility.CenteredStyle, GUILayout.Width((Screen.width - 30) / 4));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label(m.stat_might.ToString(), InterfaceUtility.CenteredStyle, GUILayout.Width((Screen.width - 30) / 4));
                GUILayout.Label(m.stat_resistance.ToString(), InterfaceUtility.CenteredStyle, GUILayout.Width((Screen.width - 30) / 4));
                GUILayout.Label(m.stat_luck.ToString(), InterfaceUtility.CenteredStyle, GUILayout.Width((Screen.width - 30) / 4));
                GUILayout.Label(m.stat_speed.ToString(), InterfaceUtility.CenteredStyle, GUILayout.Width((Screen.width - 30) / 4));
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                foreach (DBAttack attack in m.attacks) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(attack.name, GUILayout.Width(120));
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(Monster.GetTypeIcon(attack.type), GUILayout.Height(25));
                    GUILayout.Label(attack.power + ":" + attack.accuracy + "% (" + attack.staminaCost + ")");
                    GUILayout.EndHorizontal();
                }

            }
        } else _selectedMonster = -1;
        #endregion


        #region Collection
        foldouts[2] = EditorGUILayout.Foldout(foldouts[2], "Collection");
        if (foldouts[2]) {
            List<DBMonsterPattern> list = DataBase.Select<DBMonsterPattern>();
            for (int i = 0; i < list.Count; ++i) {
                if (list[i] == null) {
                    Debug.LogError("UnexpectedBehaviour... Missing MonsterPattern.");
                    continue;
                }
                GUILayout.Label(InterfaceUtility.IntString(i + 1, 3) + ":" + list[i].name, MonsterCollection.isAlreadyCaptured(list[i]) ? _capturedStyle : MonsterCollection.isEncountered(list[i]) ? _encouteredStyle : _unknownStyle);
            }
        }
        #endregion
    }
}
